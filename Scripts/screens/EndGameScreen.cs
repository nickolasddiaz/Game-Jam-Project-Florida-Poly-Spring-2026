using Godot;
using System;

public partial class EndGameScreen : Control
{
	[Export(PropertyHint.File, "*.tscn")]
	public string StartPath { get; set; } = string.Empty;

	public override void _Ready()
	{
		var next_button = GetNode<Button>("%Continue");
		next_button.Pressed += NextButtonPressed;
	}

	private void NextButtonPressed()
	{
		ScreenManager.Instance.SwitchScreenPath(StartPath, false);
	}
}
