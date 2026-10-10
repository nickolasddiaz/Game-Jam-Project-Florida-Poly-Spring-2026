using Godot;
using System;

public partial class next_task_screen_box : ColorRect
{
	[Export(PropertyHint.File, "*.tscn")]
	private Sprite2D productPic;
	private Texture2D pendingTexture;
	private bool hasPendingTexture;
	private Label productLabel;
	private string pendingText;
	private bool hasPendingText;

	public override void _Ready()
	{
		productPic = GetNodeOrNull<Sprite2D>("%Sprite2D");
		if (productPic != null && hasPendingTexture)
			productPic.Texture = pendingTexture;

		productLabel = GetNodeOrNull<Label>("%Label");
		if (productLabel != null && hasPendingText)
			productLabel.Text = pendingText;

	}



	public void setTexture(Texture2D pic)
	{
		pendingTexture = pic;
		hasPendingTexture = true;
		productPic ??= GetNodeOrNull<Sprite2D>("%Sprite2D");
		if (productPic != null)
			productPic.Texture = pic;
	}

	public void setText(string text)
	{
		pendingText = text;
		hasPendingText = true;
		productLabel ??= GetNodeOrNull<Label>("%Label");
		if (productLabel != null)
			productLabel.Text = text;
	}


}
