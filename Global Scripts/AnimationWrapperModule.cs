using Godot;
using System;

public partial class AnimationWrapperModule : AnimatedSprite2D
{
	private Vector2 lastdir;

	private bool ChangeAnimation(string anim){
		try{
			this.Play(anim);
			return true;
		}catch {
			GD.PushWarning("Animation: " + anim + "Does not exist!");
			return false;
		}
	}

	private void Freeze(){
		SpeedScale = 0;
	}

	private void Go(float s){
		SpeedScale = s;
	}

	public void ChangeOnDir(Vector2 dir){
		if(dir.Length() < .1){
			if(SpeedScale > .1){
				Frame = 0;
				Freeze();
			}
			return;
		}

		if(SpeedScale < .1f){
			Go(1.0f);
		}

		if(float.Abs(dir.X) > float.Abs(dir.Y)){
			if(dir.X < 0){
				ChangeAnimation("LEFT");
			}else{
				ChangeAnimation("RIGHT");
			}
		}else{
			if(dir.Y < 0){
				ChangeAnimation("UP");
			}else{
				ChangeAnimation("DOWN");
			}
		}
	}
}
