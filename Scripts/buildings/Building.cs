using Godot;
using System;

// Base class for every building (crops, pens, ovens, distillery...).
// Needs a CollisionShape2D child so the player can detect clicks on it.
public abstract partial class Building : Area2D
{
	// Shared properties that every building MUST define in the Inspector
	[Export] public int Cost { get; set; } = 100;
	[Export] public Vector2 Size { get; set; } = new Vector2(1, 1);
	[Export] public Texture2D pic { get; set; }

	public abstract void OnInteract();

	public override void _Ready()
	{
		// Shared initialization code can go here
	}

	public override void _Process(double delta)
	{
		// Shared frame processing can go here
	}

	public abstract void Interact(Player player);

	public virtual void InteractSecondary(Player player)
	{
		Interact(player);
	}
}
