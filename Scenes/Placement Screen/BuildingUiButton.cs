using Godot;
using System;

public partial class BuildingUiButton : Control
{
	private Button productPic;
	private Texture2D pendingTexture;
	private bool hasPendingTexture;
	private Label productLabel;
	private string pendingText;
	private bool hasPendingText;
    private BuildingType building;

    public override void _Ready()
	{
		productPic = GetNodeOrNull<Button>("%Button");
		if (productPic != null && hasPendingTexture)
			productPic.Icon = pendingTexture;

		productPic.Pressed += onClick;

		productLabel = GetNodeOrNull<Label>("%Label");
		if (productLabel != null && hasPendingText)
			productLabel.Text = pendingText;

	}

	public void onClick()
	{
		PlayerData.Instance.PlacementEnabled = true;
		var buildings = PlayerData.Instance.Buildings[building];
		PlayerData.Instance.buildingcost = buildings.Cost;
		PlayerData.Instance.MySceneToSpawn = buildings.BuildingScene;
		PlayerData.Instance.selectionBuildingsize = buildings.Size;
	}

	public void setBuildingType(BuildingType building){
		this.building = building;
	}


	public void setTexture(Texture2D pic)
	{
		pendingTexture = pic;
		hasPendingTexture = true;
		productPic ??= GetNodeOrNull<Button>("%Button");
		if (productPic != null)
			productPic.Icon = pic;
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
