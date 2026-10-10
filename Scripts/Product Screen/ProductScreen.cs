using Godot;
using System;

public partial class ProductScreen : Control
{
	private HBoxContainer hBoxContainer;
	private PackedScene sceneFile;
	private Label timeLabel;

	public override void _Ready()
	{
		hBoxContainer = GetNode<HBoxContainer>("%HBoxContainer");
		sceneFile = GD.Load<PackedScene>("res://Scenes/Product Screen/product_item.tscn");

		foreach (var (product, texture) in PlayerData.Instance.ProductIcons) 
		{
			ProductItem sceneInstance = (ProductItem)sceneFile.Instantiate();
			sceneInstance.setTexture(texture);
			sceneInstance.setText(PlayerData.Instance.productAmounts[product].ToString());
			hBoxContainer.AddChild(sceneInstance);
		}

		timeLabel = GetNode<Label>("%Time");

		setTimeText("9:99");

	}

	public void setTimeText(string text)
	{
		timeLabel.Text = text;
	}



}
