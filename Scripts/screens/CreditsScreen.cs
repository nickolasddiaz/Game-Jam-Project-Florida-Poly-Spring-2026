using Godot;
using System;

public partial class CreditsScreen : Control
{
	[Export(PropertyHint.File, "*.tscn")]
	public string StartPath { get; set; } = string.Empty;

	public override void _Ready()
	{
		var credits_button = GetNode<Button>("%Back");
		credits_button.Pressed += BackOnButtonPressed;
	}

	private void BackOnButtonPressed()
	{
		ScreenManager.Instance.SwitchScreenPath(StartPath, false);
	}
}
