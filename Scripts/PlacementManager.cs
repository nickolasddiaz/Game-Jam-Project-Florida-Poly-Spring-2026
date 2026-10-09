using Godot;
using System;
using System.Collections.Generic;

public partial class PlacementManager : Node2D
{
	//Focusing on Grid placement here.
	public float cellSize = 32;

	//Variables for the hovering actions.
	private Vector2I _hoveredCell;
	private bool _mouseInsideGrid;


	//This is just testing because I dont have access to like UI rightn ow...
	public Vector2I _buildingSize = new Vector2I(2, 1);
	private Area2D ValidPlacement;
	private bool _isMouseInside = false;

	public Vector2 RectAreaSize;

	[Export] public Color GridColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

	[Export] public PackedScene MySceneToSpawn { get; set; }

	public override void _Ready()
	{
		ValidPlacement = GetNode<Area2D>("%ValidPlacement");

		ValidPlacement.MouseEntered += OnMouseEntered;
		ValidPlacement.MouseExited += OnMouseExited;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!_isMouseInside)
		{
			return;
		}
		
		Vector2 mousePosition = GetLocalMousePosition();

		_hoveredCell = new Vector2I(Mathf.FloorToInt(mousePosition.X / cellSize), Mathf.FloorToInt(mousePosition.Y / cellSize));


		QueueRedraw();

		//Basically, divindg the mouse position by 32 tells you which cell it is in. 
		

		//With getting the local mouse position, it does it with the camera view, and then we force a visual update whenever that happens. with the redraw.

		//Then go back to draw after the grid lines loop(s).

	}

	//Basically this entire thing is checking input. We want to ONLY check left mouse input. If so, get mouse position, calculate, check occupancy.
	public override void _UnhandledInput(InputEvent @event)
	{
		//Basically ONLY LEFT CLICK dont react to anything else...
		if (@event is not InputEventMouseButton mouseEvent || mouseEvent.ButtonIndex != MouseButton.Left
			|| !mouseEvent.Pressed)
		{
			return;
		}

		//Then we need to read the mouse position at the time that we CLICK
		Vector2 mousePosition = GetLocalMousePosition();

		Vector2I clickedCell = new Vector2I(Mathf.FloorToInt(mousePosition.X / cellSize), Mathf.FloorToInt(mousePosition.Y / cellSize));


		if (MySceneToSpawn.Instantiate() is Node2D spawnedInstance)
		{
			// 2. Add it to the tree first (best practice in Godot 4)
			AddChild(spawnedInstance);

			// 3. Set its global position to your target coordinates
			spawnedInstance.GlobalPosition = mousePosition;
		}

	}

	//Draw function for a grid

	public override void _Draw()
	{
		if (!_isMouseInside)
		{
			return;
		}

		Vector2 cellPosition = new Vector2(_hoveredCell.X * cellSize, _hoveredCell.Y * cellSize);			
		Rect2 preview = new Rect2(cellPosition, new Vector2(cellSize, cellSize));

		//So now we are checking and providing more context to the player.
		//If its good, we will go blue, if not, and is obstructed, we go red.

		//Does something alredy occupy? placholer.
		// bool isOccupied = _occupiedCells.Contains(_hoveredCell);
		bool isOccupied = false;


		Color fillColor;
		Color outlineColor;

		if (isOccupied)
		{
			fillColor = new Color(1f, 0.2f, 0.2f, 0.4f);
			outlineColor = Colors.Red;
		}
		else
		{
			fillColor = new Color(0.2f, 0.8f, 1f, 0.4f);
			outlineColor = Colors.Cyan;
		}

		//Temporary transparent fill thing I odnt know honestl
		DrawRect(preview, fillColor);
		DrawRect(preview, outlineColor, false, 2f);
		
	}

	private void OnMouseEntered()
	{
		_isMouseInside = true;
		GD.Print("Mouse entered the Area2D!");
	}

	private void OnMouseExited()
	{
		_isMouseInside = false;
		GD.Print("Mouse left the Area2D.");
	}
}
