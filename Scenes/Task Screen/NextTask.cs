using Godot;
using Godot.Collections;

public partial class NextTask : Control
{
	private GridContainer inventoryContainer;

	[Export]
	public PackedScene next_task_screen_box { get; set; }

	[Export(PropertyHint.File, "*.tscn")]
	public string NextPath { get; set; } = string.Empty;

	private readonly Dictionary<ProductType, NextTaskScreenBox> productItems = new();

	public override void _Ready()
	{
		inventoryContainer = GetNode<GridContainer>("%InventoryGridContainer");
		var continue_button = GetNode<Button>("%Continue");

		foreach (Node child in inventoryContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var (product, texture) in PlayerData.Instance.ProductIcons) 
		{
			var sceneInstance = (NextTaskScreenBox)next_task_screen_box.Instantiate();
			sceneInstance.setTexture(texture);
			int productCount = 0;
			if (DayInfo.Instance.Days[0].ProductNeeds.ContainsKey(product))
			{
				productCount = DayInfo.Instance.Days[0].ProductNeeds[product];
			} else
			{
				productCount = 0;
			}
			sceneInstance.setText($"{product}: {productCount}");
			productItems[product] = sceneInstance;
			inventoryContainer.AddChild(sceneInstance);
		}

		continue_button.Pressed += OnButtonPressed;

	}

	public void updateProductAmounts()
	{
		foreach ((ProductType product, NextTaskScreenBox sceneInstance) in productItems)
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
