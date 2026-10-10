using Godot;

// Placeholder building for testing click-to-move
public partial class TestBuilding : Building
{
	public override void Interact(Player player)
	{
		GD.Print($"Player interacted with {Name}");
	}
}
