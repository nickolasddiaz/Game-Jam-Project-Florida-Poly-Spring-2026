using System.Collections.Generic;
using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public AnimationWrapperModule myAnim;

	[Export]
	public float Speed = 120.0f;

	// How close (in pixels) the player must get to a stop before it counts as arrived
	[Export]
	public float ArriveDistance = 4.0f;

	// A stop is either a building to interact with, or just a spot on the ground
	private readonly record struct Stop(Vector2 Position, Building Building);

	private readonly Queue<Stop> _queue = new Queue<Stop>();
	private Stop _target;
	private bool _hasTarget;

	public override void _Ready()
	{
		

		AddToGroup("player");
	}

	


	public void ClearQueue()
	{
		_queue.Clear();
		_hasTarget = false;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		//Debugging
		if (@event is InputEventMouseButton mouseClick && mouseClick.Pressed)
		{
			GD.Print("Player received click");
		}
		//end debugging

		if (@event.IsActionPressed("ui_cancel"))
		{
			ClearQueue();
			return;
		}

		if (@event is not InputEventMouseButton { Pressed: true } click)
		{
			return;
		}

		Vector2 mousePos = GetGlobalMousePosition();
		Building building = GetBuildingAt(mousePos);

		if (click.ButtonIndex == MouseButton.Left)
		{
			if (building != null)
			{
				// Left-click a building: add it to the queue
				_queue.Enqueue(new Stop(building.GlobalPosition, building));
			}
			else
			{
				// Left-click the ground: drop everything and walk there
				ClearQueue();
				_queue.Enqueue(new Stop(mousePos, null));
			}
		}
		else if (click.ButtonIndex == MouseButton.Right)
		{
			if (building != null)
			{
				// Right-click a building: queue it for interaction
				_queue.Enqueue(new Stop(building.GlobalPosition, building));
			}
			else
			{
				// Right-click the ground: cancel the queue
				ClearQueue();
			}
		}
	}

	// Finds a Building under the given world position (needs a CollisionShape2D)
	private Building GetBuildingAt(Vector2 worldPos)
	{
		var query = new PhysicsPointQueryParameters2D
		{
			Position = worldPos,
			CollideWithAreas = true,
			CollideWithBodies = false
		};

		var hits = GetWorld2D().DirectSpaceState.IntersectPoint(query);
		foreach (var hit in hits)
		{
			if (hit["collider"].AsGodotObject() is Building b)
			{
				return b;
			}
		}
		return null;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Grab the next valid stop from the queue
		while (!_hasTarget && _queue.Count > 0)
		{
			Stop next = _queue.Dequeue();
			// Skip buildings that were removed since they were queued
			if (next.Building != null && !IsInstanceValid(next.Building))
			{
				continue;
			}
			_target = next;
			_hasTarget = true;
		}

		Vector2 direction = Vector2.Zero;

		if (_hasTarget)
		{
			if (_target.Building != null && !IsInstanceValid(_target.Building))
			{
				_hasTarget = false;
			}
			else
			{
				Vector2 destination = _target.Building != null ? _target.Building.GlobalPosition : _target.Position;
				Vector2 toTarget = destination - GlobalPosition;
				float arrive = Mathf.Max(ArriveDistance, Speed * (float)delta);

				if (toTarget.Length() <= arrive)
				{
					Building arrived = _target.Building;
					_hasTarget = false;
					arrived?.Interact(this);
				}
				else
				{
					direction = toTarget.Normalized();
				}
			}
		}

		Velocity = direction * Speed;
		if (IsInstanceValid(myAnim))
		{
			myAnim.ChangeOnDir(direction);
		}
		MoveAndSlide();
	}
}
