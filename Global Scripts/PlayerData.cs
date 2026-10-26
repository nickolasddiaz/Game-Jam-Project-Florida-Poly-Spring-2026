using Godot;
using System;
using System.Collections.Generic;

public enum ProductType
{
	Barley, Cabbage, Grape,
	Wool, Milk,
	Bread, Wine, Ale,
}

public enum BuildingType
{
	Barley_Crop, Cabbage_Crop, Grape_Crop, 
	Sheep_Pen, Cow_Pen, 
	Bread_Oven, Distillery,
	Hut
}


public class BuildingData
{
	public int Cost;
	public Texture2D PlacementTexture;
	public Texture2D IconTexture;
	public PackedScene BuildingScene;

	public Vector2I Size;
	public BuildingData(string Path)
	{
		BuildingScene = GD.Load<PackedScene>(Path);

		if (BuildingScene is null)
		{
			throw new NullReferenceException($"Building's location is unknown: {Path}");
		}
		
		var instance = BuildingScene.Instantiate();
		if (instance is not Building building)
		{
			throw new Exception($"Scene root must inherit Building: {Path}");
		}

		Cost = building.Cost;
		PlacementTexture = building.pic;
		Size = (Vector2I)building.Size;
		building.Free();
	}
}

public partial class PlayerData : Node
{
    public Godot.Collections.Dictionary<ProductType, int> productAmounts = new Godot.Collections.Dictionary<ProductType, int>();
	public Godot.Collections.Dictionary<BuildingType, bool> isBuildingUnlockedDict = new Godot.Collections.Dictionary<BuildingType, bool>();
	public Godot.Collections.Dictionary<ProductType, Texture2D> ProductIcons = new() {};
	public Dictionary<BuildingType, BuildingData> Buildings = new() {};
	public int money = 5000; 
	public int day = 0;
	public bool PlacementEnabled = true;
	public PackedScene MySceneToSpawn;
	public int buildingcost = 0;
	public Vector2I selectionBuildingsize = new Vector2I(1, 1);
	public PlacementManager placementManager;
	public Node2D BuildingContainer;
    public EndOfDayScreen endOfDayScreen;
	public static PlayerData Instance { get; private set; } // setting PlayerData static
	public ProductScreen productScreen;
    public float time = 4; // seconds
	public bool startTimer = false;

	[Export]
    public SpriteFrames productIcons { get; set; }

	[Export]
	public SpriteFrames buildingIcons { get; set; }

		public override void _Ready()
	{
		Instance = this; // setting PlayerData static
        foreach (ProductType product in Enum.GetValues<ProductType>())
		{
			Texture2D frameTexture = productIcons.GetFrameTexture(product.ToString(), 0);

			if (frameTexture == null)
			{
				throw new NullReferenceException($"spr_productIcons.aseprite: A frame is null: {product}");
			}

			ProductIcons[product] = frameTexture;
			productAmounts[product] = 0;
		}

		foreach (BuildingType building in Enum.GetValues<BuildingType>())
		{
			//temporary
			isBuildingUnlockedDict[building] = true;
			Buildings[building] = new BuildingData($"res://Scenes/Buildings/{building}.tscn");
			Texture2D frameTexture = buildingIcons.GetFrameTexture(building.ToString(), 0);
			Buildings[building].IconTexture = frameTexture;
		}
	}

    public void newDay()
	{
		var today = DayInfo.Instance.Days[day];
		money += today.NumberOfMoneyGiven;
		time = today.Time;
		for (int i = 0; i <= day; i++) {
			isBuildingUnlockedDict[today.UnlockableBuilding] = true;
		}
		productScreen.setTimeText(formatTime());
		productScreen.updateProductAmounts();
	}

    public void placementToGameplayTransition()
    {
        PlacementEnabled = false;
        placementManager.SetPlacementEnabled(false);

        placementManager.GetNode<Player>("../Player")
            .SetControlEnabled(true);

        placementManager.Visible = false;
        placementManager.ProcessMode = ProcessModeEnum.Disabled;

        startTimer = true;
    }

    public void gameplayToEndofDay()
	{
		PlacementEnabled = true;
		placementManager.ProcessMode = ProcessModeEnum.Inherit;
		placementManager.Visible = true;
		startTimer = false;
		var endDayPath = "res://Scenes/end_of_day_screen.tscn";
		ScreenManager.Instance.SwitchScreenPath(endDayPath, false);
		endOfDayScreen.updateEndofDay();
	}

	public string formatTime()
	{
		TimeSpan elapsedTime = TimeSpan.FromSeconds(time);

        // Format as minute:second
        string formattedTime = string.Format("{0:D2}:{1:D2}", (int)elapsedTime.TotalMinutes, elapsedTime.Seconds);
		return formattedTime;
	}
	public override void _Process(double delta)
	{

		if (startTimer)
		{
			time -= (float)delta;
			productScreen.setTimeText(formatTime());
			if (time <= 0.0f)
			{
				startTimer = false;
				gameplayToEndofDay();
			}
        }
	}
}
