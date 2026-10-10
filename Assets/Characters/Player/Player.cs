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

	// A stop is either a building to interact with, or just a spot on the ground.
	// Secondary = the building was right-clicked (e.g. collect instead of feed).
	private readonly record struct Stop(Vector2 Position, Building Building, bool Secondary);

	private readonly Queue<Stop> _queue = new Queue<Stop>();
	private Stop _target;
	private bool _hasTarget;
	private bool _controlEnabled = true;

	public override void _Ready()
	{
		AddToGroup("player");
	}

	// Turn player clicking on/off. Call SetControlEnabled(false) while the edit/placement
	// screen is open so clicks there don't also move the player, and true for gameplay.
	public void SetControlEnabled(bool enabled)
	{
		_controlEnabled = enabled;
		if (!enabled)
		{
			ClearQueue();
		}
	}

	public void ClearQueue()
	{
		_queue.Clear();
		_hasTarget = false;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!_controlEnabled)
		{
			return;
		}

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
				// Left-click a building: queue it (feed / plant / harvest)
				_queue.Enqueue(new Stop(building.GlobalPosition, building, false));
			}
			else
			{
				// Left-click the ground: drop everything and walk there
				ClearQueue();
				_queue.Enqueue(new Stop(mousePos, null, false));
			}
		}
		else if (click.ButtonIndex == MouseButton.Right)
		{
			if (building != null)
			{
				// Right-click a building: queue it for its secondary action (collect)
				_queue.Enqueue(new Stop(building.GlobalPosition, building, true));
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
					Stop arrived = _target;
					_hasTarget = false;

					if (arrived.Building != null)
					{
						if (arrived.Secondary)
						{
							arrived.Building.InteractSecondary(this);
						}
						else
						{
							arrived.Building.Interact(this);
						}
					}
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
