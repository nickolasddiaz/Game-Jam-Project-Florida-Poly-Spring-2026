using Godot;
using System;
using Godot.Collections;

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
public partial class PlayerData : Node
{
	public static PlayerData Instance { get; private set; } // setting PlayerData static

	// look at NextTaskScreen to know how to loop through this
	public Dictionary<ProductType, int> productAmounts = new Dictionary<ProductType, int>();

	public Dictionary<BuildingType, bool> isBuildingUnlockedDict = new Dictionary<BuildingType, bool>();
	public int money = 0;
	public int day = 0;

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
	}

	public override void _Process(double delta)
	{
	}
}
