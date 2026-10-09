using Godot;
using System;
using System.Collections.Generic;

public partial class PlacementManager : Node2D
{


	//Focusing on Grid placement here.
	private const int CellSize = 32;
	private const int Columns = 20;
	private const int Rows = 11;


	//hashset stores each cell ONCE, remembers grid cells where we placed preholder. This is used for the "clicking" and the occupied cells.
	//The hashset WILL NOT store the same cell twice. 
	private readonly HashSet<Vector2I> _occupiedCells = new();


	//Variables for the hovering actions.
	private Vector2I _hoveredCell;
	private bool _mouseInsideGrid;


	//This is just testing because I dont have access to like UI rightn ow...
	public Vector2I _buildingSize = new Vector2I(2, 1);









    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
		Vector2 mousePosition = GetLocalMousePosition();

		_hoveredCell = new Vector2I(Mathf.FloorToInt(mousePosition.X / CellSize), Mathf.FloorToInt(mousePosition.Y / CellSize));


		_mouseInsideGrid = _hoveredCell.X >= 0 && _hoveredCell.X < Columns && _hoveredCell.Y >= 0 && _hoveredCell.Y < Rows;


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

		Vector2I clickedCell = new Vector2I(Mathf.FloorToInt(mousePosition.X / CellSize), Mathf.FloorToInt(mousePosition.Y / CellSize));

		//I dont think I need this, but these are to ignore clicks that are outside of the grid...if somehow that happens?
		if (clickedCell.X < 0 || clickedCell.X >= Columns || clickedCell.Y < 0 || clickedCell.Y >= Rows)
		{
			return;
		}


		//Is the cell occupied? Then we need to return a no, so that we can prevent placing things in the same position
		if (_occupiedCells.Add(clickedCell))
		{
			QueueRedraw();
		}

    }

    //Draw function for a grid

    public override void _Draw()
    {
		int width = Columns * CellSize;
		int height = Rows * CellSize;

		//THIS IS JUST FOR REFERENCE SO I KNOW WHAT IT LOOKS LIKE
		Color gridColor = new Color(1f, 1f, 1f, 0.35f);

		//Lines for Vert
		for (int column = 0; column <= Columns; column++)
		{
			float x = column * CellSize;

			DrawLine( new Vector2(x, 0), new Vector2(x, height), gridColor);
		}

		//Lines for Horizontal

		for (int row = 0; row <= Rows; row++)
		{
			float y = row * CellSize;

			DrawLine(new Vector2(0, y), new Vector2(width, y), gridColor);
		}


		//So now before, draw what we have placed. this is what is actually there, orange stuff.
		foreach (Vector2I cell in _occupiedCells)
		{
			//Convert the coordinate into positions.
			Vector2 position = new Vector2(cell.X * CellSize, cell.Y * CellSize);

			Rect2 buildingRect = new Rect2(position, new Vector2(CellSize, CellSize));

			DrawRect(buildingRect, new Color(0.85f, 0.6f, 0.2f, 1f));
		}

		//basically we are hovering right so just hovering and thats the cyan blue color.
		if (_mouseInsideGrid)
		{
			Vector2 cellPosition = new Vector2(_hoveredCell.X * CellSize, _hoveredCell.Y * CellSize);

			
			Rect2 preview = new Rect2(cellPosition, new Vector2(CellSize, CellSize));

			//So now we are checking and providing more context to the player.
			//If its good, we will go blue, if not, and is obstructed, we go red.

			//Does something alredy occupy? placholer.
			bool isOccupied = _occupiedCells.Contains(_hoveredCell);

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

	
    }






	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	
}
