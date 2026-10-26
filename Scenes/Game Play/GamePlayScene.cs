using Godot;
using System;

public partial class GamePlayScene : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		PlayerData.Instance.BuildingContainer = GetNode<Node2D>("%BuildingContainer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
