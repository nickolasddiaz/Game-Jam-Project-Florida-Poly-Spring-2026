using Godot;
using System;

public partial class ScreenManager : Node
{
	public static ScreenManager Instance { get; private set; }
	private Node _screenContainer;
	private Node _currentScreen;
	public override void _Ready()
	{
		Instance = this;
		_screenContainer = GetTree().CurrentScene;
	}


	public void SwitchScreen(PackedScene nextScreenPacked, bool deleteOld )
	{
		if (_currentScreen != null)
		{
			if (deleteOld)
			{
				_currentScreen.QueueFree();
			}
			else
			{
				var parent = _currentScreen.GetParent();
				if (parent != null)
				{
					parent.CallDeferred(Node.MethodName.RemoveChild, _currentScreen);
				}
			}
		}

		_currentScreen = null;
		if (nextScreenPacked != null)
		{
			_currentScreen = nextScreenPacked.Instantiate();
			(_screenContainer ?? this).AddChild(_currentScreen);
		}
	}

	public void SwitchScreenPath(string nextScreenPacked, bool deleteOld )
	{
		var scene = GD.Load<PackedScene>(nextScreenPacked);
        if (scene != null)
        {
            SwitchScreen(scene, deleteOld);
        }
        else
        {
            GD.PrintErr($"Failed to load screen:");
        }
	}

}
