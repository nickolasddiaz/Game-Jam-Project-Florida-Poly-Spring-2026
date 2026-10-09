using Godot;
using System;

public partial class EndOfDayScreen : Control
{
	[Export(PropertyHint.File, "*.tscn")]

	public string EndGamePath { get; set; } = string.Empty;
		
	[Export(PropertyHint.File, "*.tscn")]

	public string NextTaskPath { get; set; } = string.Empty;



	public override void _Ready()
	{
		var next_button = GetNode<Button>("%Continue");
		next_button.Pressed += NextButtonPressed;
	}

	private void NextButtonPressed()
	{
		ScreenManager.Instance.SwitchScreenPath(NextTaskPath, false);
	}
}
