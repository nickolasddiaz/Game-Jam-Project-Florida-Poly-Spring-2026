using Godot;
using System;

public partial class NextTaskScreen : Control
{
	[Export(PropertyHint.File, "*.tscn")]
	public string NextPath { get; set; } = string.Empty;

	public override void _Ready()
	{
		var continue_button = GetNode<Button>("%Continue");
		continue_button.Pressed += OnButtonPressed;

		var product_label = GetNode<Label>("%Product_Label");
		
		string text = "";
		foreach (var (product, amount) in PlayerData.Instance.productAmounts)
		{
			text += $"{product}: {amount} ";
		}
		product_label.Text = text;
	}

	private void OnButtonPressed()
	{
		ScreenManager.Instance.SwitchScreenPath(NextPath, false);
	}
}
