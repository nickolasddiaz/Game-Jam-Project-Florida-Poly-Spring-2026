using Godot;
using System;
using System.Collections.Generic;

public enum ProductType
{
	barley, cabbage, grape,
	wool, milk,
	bread, wine, ale,
}

public enum BuildingType
{
	barley_crop, cabbage_crop, grape_crop, 
	sheep_pen, cow_pen, 
	bread_oven, distillary
}


public class Day
{
	 
	public BuildingType unlockable_building;
	public Godot.Collections.Dictionary<ProductType, int> productNeeds;
	public float time;
	public int numberOfMoneyGiven;
	public Day(BuildingType unlockable_building, Godot.Collections.Dictionary<ProductType, int> productNeeds, float time, int numberOfMoneyGiven)
    {
        this.unlockable_building = unlockable_building;
        this.productNeeds = productNeeds;
		this.time = time;
		this.numberOfMoneyGiven = numberOfMoneyGiven;
    }
}

public partial class PlayerData : Node
{
	public static PlayerData Instance { get; private set; } // setting PlayerData static

	// look at NextTaskScreen to know how to loop through this
	public Godot.Collections.Dictionary<ProductType, int> productAmounts = new Godot.Collections.Dictionary<ProductType, int>();

	public Godot.Collections.Dictionary<BuildingType, bool> isBuildingUnlockedDict = new Godot.Collections.Dictionary<BuildingType, bool>();
	public int money = 0;
	public int day = 0;

	public List<Day> DayInfo;

	public Godot.Collections.Dictionary<ProductType, Texture2D> ProductIcons = new()
    {
        { ProductType.barley, GD.Load<Texture2D>("res://Assets/placeholder.png") },
        { ProductType.cabbage, GD.Load<Texture2D>("res://Assets/placeholder.png") },
		{ ProductType.grape, GD.Load<Texture2D>("res://Assets/placeholder.png") },
		{ ProductType.wool, GD.Load<Texture2D>("res://Assets/placeholder.png") },
		{ ProductType.milk, GD.Load<Texture2D>("res://Assets/placeholder.png") },
		{ ProductType.bread, GD.Load<Texture2D>("res://Assets/placeholder.png") },
		{ ProductType.wine, GD.Load<Texture2D>("res://Assets/placeholder.png") },
		{ ProductType.ale, GD.Load<Texture2D>("res://Assets/placeholder.png") },
    };

	public override void _Ready()
	{
		Instance = this; // setting PlayerData static
		foreach (ProductType state in Enum.GetValues<ProductType>())
		{
			productAmounts[state] = 0;
		}

		foreach (BuildingType state in Enum.GetValues<BuildingType>())
		{
			isBuildingUnlockedDict[state] = false;
		}


		DayInfo = new List<Day> // 0-indexed
			{
				// Day 1
				new Day(
					BuildingType.barley_crop,
					new Godot.Collections.Dictionary<ProductType, int>
					{
						{ ProductType.barley, 10 }
					},
					200,
					300
				),

				// Day 2
				new Day(
					BuildingType.cabbage_crop,
					new Godot.Collections.Dictionary<ProductType, int>
					{
						{ ProductType.barley, 10 },
						{ ProductType.cabbage, 10 }
					},
					200,
					600
				),

				// Day 3
				new Day(
					BuildingType.sheep_pen,
					new Godot.Collections.Dictionary<ProductType, int>
					{
						{ ProductType.barley, 8 },
						{ ProductType.cabbage, 8 },
						{ ProductType.wool, 2 }
					},
					200,
					1000
				),

				// Day 4
				new Day(
					BuildingType.cow_pen,
					new Godot.Collections.Dictionary<ProductType, int>
					{
						{ ProductType.cabbage, 4 },
						{ ProductType.wool, 4 },
						{ ProductType.milk, 4 }
					},
					200,
					2000
				),

				// Day 5
				new Day(
					BuildingType.bread_oven,
					new Godot.Collections.Dictionary<ProductType, int>
					{
						{ ProductType.bread, 4 },
						{ ProductType.milk, 4 },
						{ ProductType.wool, 4 },
						{ ProductType.barley, 4 }
					},
					200,
					3000
				),

				// Day 6
				new Day(
					BuildingType.distillary,
					new Godot.Collections.Dictionary<ProductType, int>
					{
						{ ProductType.ale, 5 },
						{ ProductType.milk, 4 },
						{ ProductType.wool, 4 },
						{ ProductType.cabbage, 5 }
					},
					200,
					4000
				),

				// Day 7
				new Day(
					BuildingType.grape_crop,
					new Godot.Collections.Dictionary<ProductType, int>
					{
						{ ProductType.grape, 10 },
						{ ProductType.cabbage, 8 },
						{ ProductType.ale, 4 }
					},
					200,
					5000
				),

				new Day(
					BuildingType.barley_crop, // default
					new Godot.Collections.Dictionary<ProductType, int>
					{
						{ ProductType.barley, 10 },
						{ ProductType.cabbage, 10 },
						{ ProductType.grape, 10 },
						{ ProductType.wool, 10 },
						{ ProductType.milk, 10 },
						{ ProductType.bread, 10 },
						{ ProductType.ale, 10 },
						{ ProductType.wine, 10 }
					},
					200,
					0
				)
			};

	}

	public override void _Process(double delta)
	{
	}
}
