using Godot;
using Godot.Collections;

public partial class ProductScreen : Control
{
	private HBoxContainer hBoxContainer;

	[Export] 
	public PackedScene product_item { get; set; }
	private Label timeLabel;
	private readonly Dictionary<ProductType, ProductItem> productContainers = new();

	public override void _Ready()
	{
		PlayerData.Instance.productScreen = this;

		hBoxContainer = GetNode<HBoxContainer>("%HBoxContainer");
		timeLabel = GetNode<Label>("%Time");

		foreach (var (product, texture) in PlayerData.Instance.ProductIcons) 
		{
			ProductItem sceneInstance = (ProductItem)product_item.Instantiate();
			sceneInstance.setTexture(texture);
			sceneInstance.setText(PlayerData.Instance.productAmounts[product].ToString());
			productContainers[product] = sceneInstance;
			hBoxContainer.AddChild(sceneInstance);
		}

		setTimeText("9:99");
	}

	public void updateProductAmounts()
	{
		foreach (var (product, sceneInstance) in productContainers)
		{
			sceneInstance.setText(PlayerData.Instance.productAmounts[product].ToString());
		}
	}

	public void setTimeText(string text)
	{
		timeLabel.Text = text;
	}

}
