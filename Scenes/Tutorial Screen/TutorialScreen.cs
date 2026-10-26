using Godot;
using System;

public partial class TutorialScreen : Control
{
	[Export(PropertyHint.File, "*.tscn")]
	public string NextPath { get; set; } = string.Empty;


	public override void _Ready()
	{
		var continue_button = GetNode<Button>("%Continue");
		continue_button.Pressed += OnButtonPressed;
	}

	private void OnButtonPressed()
	{
		ScreenManager.Instance.SwitchScreenPath(NextPath, false);
	}
}
