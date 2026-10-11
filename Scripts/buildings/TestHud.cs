using System;
using System.Text;
using Godot;

// Test-only helper: gives starting items and shows the inventory on screen.
// Delete it (and its node in PlayerTest) when the real HUD exists.
public partial class TestHud : CanvasLayer
{
	[Export]
	public int StartingAmount { get; set; } = 10;

	private Label _label;

	public override void _Ready()
	{
		_label = new Label();
		_label.Position = new Vector2(8, 8);
		AddChild(_label);

		if (PlayerData.Instance != null)
		{
			foreach (ProductType type in Enum.GetValues<ProductType>())
			{
				PlayerData.Instance.productAmounts[type] = StartingAmount;
			}
		}
	}

	public override void _Process(double delta)
	{
		if (PlayerData.Instance == null)
		{
			return;
		}

		var sb = new StringBuilder("Inventory\n");
		foreach (ProductType type in Enum.GetValues<ProductType>())
		{
			sb.AppendLine($"{type}: {PlayerData.Instance.productAmounts[type]}");
		}
		_label.Text = sb.ToString();
	}
}
