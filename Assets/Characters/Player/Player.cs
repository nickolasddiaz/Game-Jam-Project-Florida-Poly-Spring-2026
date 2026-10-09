using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public AnimationWrapperModule myAnim;

	public const float Speed = 300.0f;
	//public const float JumpVelocity = -400.0f;

	public override void _PhysicsProcess(double delta)
	{

		/* Top down means we don't need these
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}
		*/

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ac_left", "ac_right", "ac_up", "ac_down");
		Velocity = new Vector2(direction.X, direction.Y)*Speed;
		if(IsInstanceValid(myAnim)){
			myAnim.ChangeOnDir(direction);
		}
		MoveAndSlide();
	}
}
