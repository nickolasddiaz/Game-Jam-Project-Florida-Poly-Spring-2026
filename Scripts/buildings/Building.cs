using Godot;

// Base class for every building (crops, pens, ovens, distillery...).
// Needs a CollisionShape2D child so the player can detect clicks on it.
// The Player handles all clicking and walking; the building only reacts in Interact().
public abstract partial class Building : Area2D
{
	// Size in tiles (hut 2x2, animal pen 2x1, crop 1x1...)
	[Export]
	public Vector2I Size { get; set; } = new Vector2I(1, 1);

	[Export]
	public int Cost { get; set; } = 100;

	// Called when the player arrives at this building
	public abstract void Interact(Player player);
}
