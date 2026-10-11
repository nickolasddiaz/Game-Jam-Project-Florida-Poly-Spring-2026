using Godot;
using Godot.Collections;
using System;

public partial class ProductScreen : Control
{
	private HBoxContainer hBoxContainer;
	private PackedScene sceneFile;
	private Label timeLabel;
	private readonly Dictionary<ProductType, ProductItem> productItems = new();

	public override void _Ready()
	{
		hBoxContainer = GetNode<HBoxContainer>("%HBoxContainer");
		sceneFile = GD.Load<PackedScene>("res://Scenes/Product Screen/product_item.tscn");

		foreach (var (product, texture) in PlayerData.Instance.ProductIcons) 
		{
			ProductItem sceneInstance = (ProductItem)sceneFile.Instantiate();
			sceneInstance.setTexture(texture);
			sceneInstance.setText(PlayerData.Instance.productAmounts[product].ToString());
			productItems[product] = sceneInstance;
			hBoxContainer.AddChild(sceneInstance);
		}

		timeLabel = GetNode<Label>("%Time");
		PlayerData.Instance.productScreen = this;

		setTimeText("9:99");
	}

	public void updateProductAmounts()
	{
		foreach (var (product, sceneInstance) in productItems)
		{
			sceneInstance.setText(PlayerData.Instance.productAmounts[product].ToString());
		}
	}

	public void setTimeText(string text)
	{
		timeLabel.Text = text;
	}



}
