using Godot;

// Base class for every building (crops, pens, ovens, distillery...).
// Needs a CollisionShape2D child so the player can detect clicks on it.
// The Player handles all clicking and walking; the building only reacts when the player arrives.
public abstract partial class Building : Area2D
{
	// Size in tiles (hut 2x2, animal pen 2x1, crop 1x1...)
	[Export]
	public Vector2I Size { get; set; } = new Vector2I(1, 1);

	[Export]
	public int Cost { get; set; } = 100;

	// Player arrived after a LEFT-click on this building
	// (plant / harvest a crop, feed an animal pen or machine...)
	public abstract void Interact(Player player);

	// Player arrived after a RIGHT-click on this building.
	// By default it does the same as Interact. Production buildings override it to collect.
	public virtual void InteractSecondary(Player player)
	{
		Interact(player);
	}
}
