using Godot;
using System.Collections.Generic;

public partial class PlacementManager : Node2D
{
    //So our validplacement uses this file to draw the grids.

    [Export]
    public Color GridColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    [Export]
    public PackedScene MySceneToSpawn {  get; set; }

    public float cellsize = 32;

    private TileMapLayer GridPlacement;

    private Vector2I hoveredcell;

    private bool mouseingrid;

    private string buildingnames = "Crop Plot";
        private Vector2I buildingsize = new Vector2I(1, 1);

    //So we need a dictionary because we need to know WHAT we are moving IF we are actually going to implement moving/deleting
    private Dictionary<Vector2I, Node2D> occupiedcells = new Dictionary<Vector2I, Node2D>();

    public override void _Ready()
    {
        GridPlacement = GetNode<TileMapLayer>("GridPlacement");
    }

    public void SelectBuilding(string buildingname, Vector2I footprint, PackedScene bulidingscene)
    {
        buildingnames = buildingname;
        buildingsize = footprint;
        MySceneToSpawn = bulidingscene;

        QueueRedraw();
    }

    public override void _Process(double delta)
    {
        //Finding the hovered cell and deciding whether to show the preview or not.
        hoveredcell = GetMouseCell();
        mouseingrid = IsCellInsideGrid(hoveredcell);

        QueueRedraw();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        //Basically ONLY accept MOUSE button inputs. Nothing more.

        if (@event is not InputEventMouseButton mouseEvent)
        {
            return;
        }

        //Now we move on to ONLY accepting LEFT button presses.
        if (mouseEvent.ButtonIndex != MouseButton.Left || !mouseEvent.Pressed)
        {
            return;
        }
        Vector2I clickedCell = GetMouseCell();

        //Says no to any clicks outside and modified to include occupancy for multiple slots.
        if (!CanPlaceBuilding(clickedCell))
        {
            return;
        }
        //Probably not necessary but making sure we assign as cene.
        if (MySceneToSpawn == null)
        {
            return;
        }
        //If something is already there then no. because dictionary says so.
        if (IsCellOccupied(clickedCell))
        {
            return;
        }
        //Create the actual building and spawn it at the center of the cells. 2x2 etc. so not like on click.
        Node2D building = MySceneToSpawn.Instantiate<Node2D>();

        //centering
        Vector2I cencell = clickedCell + buildingsize - new Vector2I(1, 1);

        building.Position = (GetCellCenter(clickedCell) + GetCellCenter(cencell)) / 2f;

        //Basically putting it down/making sure that the cell is actually recorded and saved.
        for (int x = 0; x < buildingsize.X; x++)
        {
            for (int y = 0; y < buildingsize.Y; y++)
            {
                Vector2I cell = clickedCell + new Vector2I(x, y);
                occupiedcells.Add(cell, building);
            }
        }
        AddChild(building);

        QueueRedraw();
        //debugging print we can remove this.
        GD.Print("Placed");
    }


    private bool IsCellOccupied(Vector2I cell)
    {
        //checking our dictionary with the saved thing that we did before
        return occupiedcells.ContainsKey(cell);

    }

    //Placing the building. THIS IS NOT THE PREVIEW. 
    private bool CanPlaceBuilding(Vector2I clickcell)
    {
        for (int x = 0; x < buildingsize.X; x++)
        {
            for (int y = 0;y < buildingsize.Y; y++)
            {
                Vector2I cell = clickcell + new Vector2I(x, y);

                //double check form aking we are in it and if something is in it.
                if (!IsCellInsideGrid(cell) || IsCellOccupied(cell))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsCellInsideGrid(Vector2I cell)
    {
        //so this is my old code...basically outlines the boundaries
        return cell.X >= 2 && cell.X < 16 && cell.Y >= 2 && cell.Y < 10;
    }

    private Vector2I GetMouseCell()
    {
        Vector2 mouseposition = GridPlacement.GetLocalMousePosition();

        return GridPlacement.LocalToMap(mouseposition);
    }

    private Vector2 GetCellCenter(Vector2I cell)
    {
        //Converting because we want to use tilemaps? Guess so.
        Vector2 gridposition = GridPlacement.MapToLocal(cell);
        Vector2 worldposition = GridPlacement.ToGlobal(gridposition);
        return ToLocal(worldposition);
    }


    public override void _Draw()
    {
        if (!mouseingrid)
        {
            return;
        }

        //Since the rectangles origin is 0,0 they are drawn top left. This means we need to divide from the axis.
        //Preview not artual building placement.

        Vector2 cellposition = GetCellCenter(hoveredcell) - new Vector2(cellsize / 2f, cellsize / 2f);

        //I dont know if we need this resize actually. It might double scale
        Vector2 size = new Vector2(buildingsize.X * cellsize, buildingsize.Y * cellsize);
        Rect2 preview = new Rect2(cellposition, size);

        Color outlinecolor = Colors.Cyan;

        //modified this originally because we aren't dealing in soley 1x1 vectros anyrmore.

        if (!CanPlaceBuilding(hoveredcell))
        {
            outlinecolor = Colors.Red;
        }

        Color fillcolor = outlinecolor;
        fillcolor.A = 0.4f;

        DrawRect(preview, fillcolor);
        DrawRect(preview, outlinecolor, false, 2f);

    }

}