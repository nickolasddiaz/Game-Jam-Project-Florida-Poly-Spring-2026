using Godot;
using Godot.Collections;
using System;

public partial class next_task : Control
{
	private GridContainer inventoryContainer;
	private PackedScene sceneFile;

	[Export(PropertyHint.File, "*.tscn")]
	public string NextPath { get; set; } = string.Empty;

	private readonly Dictionary<ProductType, next_task_screen_box> productItems = new();

	public override void _Ready()
	{
		inventoryContainer = GetNode<GridContainer>("%InventoryGridContainer");
		sceneFile = GD.Load<PackedScene>("res://Scenes/task_scene/next_task_screen_box.tscn");

		foreach (Node child in inventoryContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var (product, texture) in PlayerData.Instance.ProductIcons) 
		{
			next_task_screen_box sceneInstance = (next_task_screen_box)sceneFile.Instantiate();
			sceneInstance.setTexture(texture);
			int productCount = 0;
			if (PlayerData.Instance.DayInfo[0].productNeeds.ContainsKey(product))
			{
				productCount = PlayerData.Instance.DayInfo[0].productNeeds[product];
			} else
			{
				productCount = 0;
			}
			sceneInstance.setText($"{product}: {productCount}");
			productItems[product] = sceneInstance;
			inventoryContainer.AddChild(sceneInstance);
		}

		var continue_button = GetNode<Button>("%Continue");
		continue_button.Pressed += OnButtonPressed;

	}

	public void updateProductAmounts()
	{
		foreach ((ProductType product, next_task_screen_box sceneInstance) in productItems)
		{
			sceneInstance.setText($"{product}: {PlayerData.Instance.productAmounts[product]}");
		}
	}

	private void OnButtonPressed()
	{
		ScreenManager.Instance.SwitchScreenPath(NextPath, false);
		foreach (ProductType product in PlayerData.Instance.ProductIcons.Keys)
		{
			if (!PlayerData.Instance.productAmounts.ContainsKey(product))
			{
				PlayerData.Instance.productAmounts[product] = 0;
			}
		}

		PlayerData.Instance.newDay();
		updateProductAmounts();
	}


}
