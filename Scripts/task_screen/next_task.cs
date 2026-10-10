using Godot;
using System;

public partial class next_task : Control
{
	private GridContainer inventoryContainer;
	private PackedScene sceneFile;

	[Export(PropertyHint.File, "*.tscn")]
	public string NextPath { get; set; } = string.Empty;


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
			sceneInstance.setText($"{product}: {PlayerData.Instance.productAmounts[product]}");
			inventoryContainer.AddChild(sceneInstance);
		}

		var continue_button = GetNode<Button>("%Continue");
		continue_button.Pressed += OnButtonPressed;

	}

		private void OnButtonPressed()
	{
		ScreenManager.Instance.SwitchScreenPath(NextPath, false);
	}


}
