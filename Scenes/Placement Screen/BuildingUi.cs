using Godot;
using System;

public partial class BuildingUi : Control
{

	private GridContainer inventoryContainer;


	[Export]
	private PackedScene BuildingUIButton;


	[Export(PropertyHint.File, "*.tscn")]
	public string NextPath { get; set; }



	[Export]
	public PlacementManager TargPlacement;


	public override void _Ready()
	{
		inventoryContainer = GetNode<GridContainer>("%GridContainer");
		GetNode<Button>("%Continue").Pressed += OnButtonPressed;
		TargPlacement = GetParent<PlacementManager>();

		foreach (Node child in inventoryContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var (buildingType, buildData) in PlayerData.Instance.Buildings) 
{
			// Fetch cost/texture from a lightweight data source instead of instantiating the whole scene tree
			int cost = buildData.Cost;
            string iconName = buildingType.ToString();

            Texture2D texture = buildData.IconTexture;


            BuildingUiButton sceneInstance = BuildingUIButton.Instantiate<BuildingUiButton>();
			sceneInstance.setText($"{cost}\n{buildingType}");
			sceneInstance.setTexture(texture);
			sceneInstance.setBuildingType(buildingType);
			inventoryContainer.AddChild(sceneInstance);

			//For disabling you know shop unlocks
			sceneInstance.GetNode<Button>("%Button").Disabled = !PlayerData.Instance.isBuildingUnlockedDict[buildingType];

			sceneInstance.GetNode<Button>("%Button").Pressed += () =>
			{
				TargPlacement.SelectBuilding(buildData.BuildingScene);
			};
		}

	}

	private void OnButtonPressed()
	{
		PlayerData.Instance.placementToGameplayTransition();
	}


}
