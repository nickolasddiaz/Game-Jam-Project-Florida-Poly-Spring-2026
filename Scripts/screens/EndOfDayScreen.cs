using Godot;
using System;
using System.Linq;

public partial class EndOfDayScreen : Control
{
    private Button next_button;
    private Label title;
    private Label label;

    [Export(PropertyHint.File, "*.tscn")]

	public string EndGamePath { get; set; } = string.Empty;
		
	[Export(PropertyHint.File, "*.tscn")]
	public string NextTaskPath { get; set; } = string.Empty;

	public override void _Ready()
	{
		next_button = GetNode<Button>("%Continue");
		next_button.Pressed += NextButtonPressed;

		title = GetNode<Label>("%Title");
		label = GetNode<Label>("%Label");

		PlayerData.Instance.endOfDayScreen = this;
	}

	public void updateEndofDay()
	{
		title.Text = $"End of day results:{PlayerData.Instance.day + 1}";
		bool passed = PlayerData.Instance.productAmounts.Values.All(value => value == 0);


		if (passed)
		{
			if(PlayerData.Instance.day == PlayerData.Instance.lastDay)
			{
				title.Text = "Congrats you beat the Game";
			}

			label.Text = "You passed this day: going to the next day";
			PlayerData.Instance.day += 1;
		}
		else
		{
			label.Text = "You failed this dat: restarting the day";
		}
	}

	private void NextButtonPressed()
	{

		if(PlayerData.Instance.day == PlayerData.Instance.lastDay)
		{
			ScreenManager.Instance.SwitchScreenPath(EndGamePath, false);
		}
		else
		{
			ScreenManager.Instance.SwitchScreenPath(NextTaskPath, false);
		}
		
	}
}
