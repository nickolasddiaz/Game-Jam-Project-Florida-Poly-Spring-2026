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

    public bool PlacementEnabled = true;


    //this is more toggling for ui and the cyan color.
    [Export]

    public CanvasLayer PlacementUI;



    private string buildingnames = "Crop Plot";
        private Vector2I buildingsize = new Vector2I(1, 1);

    //Adding building cost because we are integrating
    private int buildingcost;



    //So we need a dictionary because we need to know WHAT we are moving IF we are actually going to implement moving/deleting
    private Dictionary<Vector2I, Node2D> occupiedcells = new Dictionary<Vector2I, Node2D>();

    public override void _Ready()
    {
        //fixing the bug where placement doesn't even work.
        GridPlacement = GetNode<TileMapLayer>("GridPlacement");

        //Select nothing becausef or some reason something is selected.
        MySceneToSpawn = null;

        //THIS IS CONTROLLING THE PLACEMENT ENABLING
        SetPlacementEnabled(true);
    }

    //toggling placement so that whenever we need to..buy mode...and whenever we need too...paly mode.
    public void SetPlacementEnabled(bool enabled)
    {
        //THIS IS WHAT PROGRAMMERS WILL USE TO TOGLLE PLACEMENT SYSTEM. placementManager.SetPlacementEnabled(true) editing. or false; gameplay.
        PlacementEnabled = enabled;

        if (PlacementUI != null)
        {
            PlacementUI.Visible = enabled;
        }
        //Update the preview when it changes
        QueueRedraw();
        GetNode<Area2D>("%ValidPlacement").QueueRedraw();
    }

    public void SelectBuilding(string buildingname, Vector2I footprint, PackedScene bulidingscene)
    {

        if (bulidingscene == null)
        {
            return;
        }
        //Reading the size from the
        Node2D instance = bulidingscene.Instantiate<Node2D>();

        if (instance is not Building building)
        {
            instance.Free();
            return;
        }


        //this was before I added Ms Code, so everything above is working with his building.cs 
        //changing variables to read placement properties from the building class.
        buildingnames = buildingname;
        buildingsize = building.Size;
        buildingcost = building.Cost;
        MySceneToSpawn = bulidingscene;

        //sinec we just need properties I already have dealings with placing
        instance.Free();

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
        //ignore placement and hide during gameplay
        if (!PlacementEnabled)
        {
            return;
        }



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

        if (PlayerData.Instance == null)
        {
            return; //because then the game cant run so.
        }

        //If player can't afford it dont place it.
        if (PlayerData.Instance.money < buildingcost)
        { //just debug can make it nice later.
            GD.Print($"Not enough money. Need {buildingcost}.");
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

        //Reducing money only after the building has been placed.
        PlayerData.Instance.money -= buildingcost;
        GD.Print($"Money remaining: {PlayerData.Instance.money}");

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
        //more hiding stuff ---and then added || because of the no select bug. Links to unhandled input which should rejectp lacement.
        if (!PlacementEnabled || MySceneToSpawn == null) {
            return;
        }

        //if not thogh
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