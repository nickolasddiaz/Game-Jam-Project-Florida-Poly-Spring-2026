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
		// Draw vertical lines
		int cols = Mathf.CeilToInt(RectAreaSize.X / parent.cellSize);
		for (int x = 0; x <= cols - 1; x++)
		{
			float posX = x * parent.cellSize;
			DrawLine(new Vector2(posX, 0), new Vector2(posX, RectAreaSize.Y), parent.GridColor);
		}

		// Draw horizontal lines
		int rows = Mathf.CeilToInt(RectAreaSize.Y / parent.cellSize);
		for (int y = 0; y <= rows - 1; y++)
		{
			float posY = y * parent.cellSize;
			DrawLine(new Vector2(0, posY), new Vector2(RectAreaSize.X, posY), parent.GridColor);
		}
	}
}
