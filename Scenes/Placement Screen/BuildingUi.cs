using Godot;
using System;

public partial class BuildingUi : CanvasLayer
{

	//So now because for some reason we have a placement manager I need to connect these buttons

	[Export]
	public PlacementManager buttons { get; set; }

	[Export]
	public PackedScene Crop {  get; set; }
    [Export]
    public PackedScene Oven { get; set; }
    [Export]
    public PackedScene Hut { get; set; }



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		GetNode<Button>("BuildingScroll/BuildingButtons/Crop").Pressed += OnCropPressed;

        GetNode<Button>("BuildingScroll/BuildingButtons/Oven").Pressed += OnOvenPressed;

        GetNode<Button>("BuildingScroll/BuildingButtons/Hut").Pressed += OnHutPressed;
    }

	//Basically doing the footprinting here.
	private void OnCropPressed()
	{
		SelectBuilding("Crop Plot", new Vector2I(1, 1), Crop);
	}
    private void OnOvenPressed()
    {
        SelectBuilding("Oven", new Vector2I(2, 1), Oven);
    }
    private void OnHutPressed()
    {
        SelectBuilding("Hut", new Vector2I(2, 2), Hut);
    }

	private void SelectBuilding(string buildingname, Vector2I footprint, PackedScene bulidingscene)
	{
		if (buttons == null)
		{
			return;
		}

        if (bulidingscene == null)
        {
            return;
        }

        buttons.SelectBuilding(buildingname, footprint, bulidingscene);

	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
