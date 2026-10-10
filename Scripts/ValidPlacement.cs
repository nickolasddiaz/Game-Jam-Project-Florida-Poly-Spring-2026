using Godot;

public partial class ValidPlacement : Area2D
{

	private Vector2 RectAreaSize;
	private PlacementManager parent;

	public override void _Ready()
	{
		var collisionShapeNode = GetNode<CollisionShape2D>("CollisionShape2D");

		// Check if the shape is a RectangleShape2D
		if (collisionShapeNode.Shape is RectangleShape2D rectShape)
		{
			RectAreaSize = rectShape.Size;
		}

		QueueRedraw();

		parent = GetParent() as PlacementManager;
	}

	public override void _Draw()
	{
		//hiding when gaming
		if (!parent.PlacementEnabled)
		{
			return;
		}

		//Doing our vertical lines
		for (int x =2; x <= 16; x++)
		{
			DrawLine(new Vector2(x * parent.cellsize, 2 * parent.cellsize), new Vector2(x * parent.cellsize, 10 * parent.cellsize), parent.GridColor);
		}


		//Horizontal line drawing
		for (int y =2; y <= 10; y++)
		{
			DrawLine(new Vector2(2 * parent.cellsize, y *parent.cellsize), new Vector2(16 * parent.cellsize, y * parent.cellsize), parent.GridColor);
		}
	}
}
