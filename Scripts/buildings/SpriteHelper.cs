using System;
using Godot;

// Small helpers for AnimatedSprite2D nodes whose SpriteFrames come from the Aseprite Wizard
// (animation names are the Aseprite tag names, e.g. "SHEEP_WOOL", "BAKING").
public static class SpriteHelper
{
	// Finds an animation ignoring case, so a tag typo like "WOOl" still matches "WOOL".
	// Returns null if the sprite has no frames or no such animation.
	public static string FindAnimation(AnimatedSprite2D sprite, string wanted)
	{
		if (sprite == null || sprite.SpriteFrames == null || wanted == null)
		{
			return null;
		}

		foreach (string name in sprite.SpriteFrames.GetAnimationNames())
		{
			if (string.Equals(name, wanted, StringComparison.OrdinalIgnoreCase))
			{
				return name;
			}
		}
		return null;
	}

	// Shows one frame of an animation without playing it.
	public static bool ShowFrame(AnimatedSprite2D sprite, string wanted, int frame = 0)
	{
		string name = FindAnimation(sprite, wanted);
		if (name == null)
		{
			return false;
		}

		int count = sprite.SpriteFrames.GetFrameCount(name);
		sprite.Animation = name;
		sprite.Frame = Mathf.Clamp(frame, 0, Math.Max(count - 1, 0));
		sprite.Pause();
		return true;
	}

	// Plays an animation if it has several frames, otherwise just shows its single frame.
	// Does nothing if that animation is already playing (so it doesn't restart every update).
	public static bool PlayOrShow(AnimatedSprite2D sprite, string wanted)
	{
		string name = FindAnimation(sprite, wanted);
		if (name == null)
		{
			return false;
		}

		if (sprite.SpriteFrames.GetFrameCount(name) > 1)
		{
			if (sprite.Animation.ToString() != name || !sprite.IsPlaying())
			{
				sprite.Play(name);
			}
		}
		else
		{
			ShowFrame(sprite, name, 0);
		}
		return true;
	}
}
