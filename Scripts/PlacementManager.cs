using Godot;
using System.Collections.Generic;

public partial class PlacementManager : Node2D
{
    // Focusing on Grid placement here.
    public float cellSize = 32;

    // Variables for the hovering actions.
    private Vector2I _hoveredCell;
    private bool _mouseInsideGrid;

    // Grid occupation state (HashSet for O(1) cell lookup)
    private readonly HashSet<Vector2I> _occupiedCells = new HashSet<Vector2I>();

    // Building properties
    public Vector2I _buildingSize = new Vector2I(2, 2);
    private Area2D ValidPlacement;
    private bool _isMouseInside = false;

    public Vector2 RectAreaSize;

    [Export] public Color GridColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    [Export] public Color RightPlacement = new Color(0.2f, 0.8f, 0.2f, 0.5f);
    [Export] public Color WrongPlacement = new Color(1f, 0.2f, 0.2f, 0.5f);

    [Export] public PackedScene MySceneToSpawn { get; set; }

    public override void _Ready()
    {
        ValidPlacement = GetNode<Area2D>("%ValidPlacement");

        ValidPlacement.MouseEntered += OnMouseEntered;
        ValidPlacement.MouseExited += OnMouseExited;
    }

    public override void _Process(double delta)
    {
        if (!_isMouseInside)
        {
            return;
        }

        Vector2 mousePosition = GetLocalMousePosition();

        _hoveredCell = new Vector2I(
            Mathf.FloorToInt(mousePosition.X / cellSize),
            Mathf.FloorToInt(mousePosition.Y / cellSize)
        );

        QueueRedraw();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Only trigger on left click down while mouse is inside valid area
        if (!_isMouseInside || @event is not InputEventMouseButton mouseEvent
            || mouseEvent.ButtonIndex != MouseButton.Left || !mouseEvent.Pressed)
        {
            return;
        }

        Vector2 mousePosition = GetLocalMousePosition();
        Vector2I clickedCell = new Vector2I(
            Mathf.FloorToInt(mousePosition.X / cellSize),
            Mathf.FloorToInt(mousePosition.Y / cellSize)
        );

        // Check if any tile in the building's footprint is occupied
        if (IsAreaOccupied(clickedCell, _buildingSize))
        {
            GD.Print("Cannot place here: Space is occupied!");
            return;
        }

        if (MySceneToSpawn?.Instantiate() is Node2D spawnedInstance)
        {
            AddChild(spawnedInstance);

            // Fix: Centering building footprint accurately using both X and Y dimensions
            float offsetX = _buildingSize.X * cellSize / 2f;
            float offsetY = _buildingSize.Y * cellSize / 2f;
            Vector2 gridPosition = new Vector2(clickedCell.X * cellSize + offsetX, clickedCell.Y * cellSize + offsetY);
            spawnedInstance.Position = gridPosition;

            // Mark footprint tiles as occupied in hashset
            MarkAreaOccupied(clickedCell, _buildingSize);
        }
    }

    public override void _Draw()
    {
        // Don't draw when mouse is outside the Area2D
        if (!_isMouseInside)
        {
            return;
        }

        Vector2 cellPosition = new Vector2(_hoveredCell.X * cellSize, _hoveredCell.Y * cellSize);          
        Vector2 previewDimensions = new Vector2(_buildingSize.X * cellSize, _buildingSize.Y * cellSize);
        Rect2 preview = new Rect2(cellPosition, previewDimensions);

        // Check occupation status for preview coloring
        bool isOccupied = IsAreaOccupied(_hoveredCell, _buildingSize);

        // Green if free, Red if occupied
        Color outlineColor = !isOccupied ? RightPlacement : WrongPlacement;
        Color fillColor = new Color(outlineColor, 0.2f);

        DrawRect(preview, fillColor);
        DrawRect(preview, outlineColor, false, 2f);
    }

    /// <summary>
    /// Checks if any cell within the target building footprint is already occupied.
    /// </summary>
    public bool IsAreaOccupied(Vector2I originCell, Vector2I size)
    {
        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                Vector2I checkCell = originCell + new Vector2I(x, y);
                if (_occupiedCells.Contains(checkCell))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Registers all cells within the target building footprint as occupied.
    /// </summary>
    public void MarkAreaOccupied(Vector2I originCell, Vector2I size)
    {
        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                _occupiedCells.Add(originCell + new Vector2I(x, y));
            }
        }
    }

    /// <summary>
    /// Removes cells from the occupied set (useful when destroying buildings).
    /// </summary>
    public void ClearAreaOccupied(Vector2I originCell, Vector2I size)
    {
        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                _occupiedCells.Remove(originCell + new Vector2I(x, y));
            }
        }
    }

    private void OnMouseEntered()
    {
        _isMouseInside = true;
        QueueRedraw();
    }

    private void OnMouseExited()
    {
        _isMouseInside = false;
        QueueRedraw();
    }
}