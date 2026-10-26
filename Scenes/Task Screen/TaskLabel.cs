using Godot;
using System;

public partial class TaskLabel : Control
{
	private Sprite2D sprite;
	private Label textlabel;
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("%Sprite2D");
		textlabel = GetNode<Label>("%Label");


	}

	public override void _Process(double delta)
	{
	}

	public void setImage(Texture2D image)
	{
		sprite.Texture = image;
	}

	public void setText(string text)
	{
		textlabel.Text = text;
	}
}
