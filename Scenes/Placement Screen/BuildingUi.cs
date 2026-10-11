using Godot;
using System;

public partial class BuildingUi : Control
{

	private GridContainer inventoryContainer;
	private PackedScene sceneFile;

	[Export(PropertyHint.File, "*.tscn")]

	
	public string NextPath { get; set; } = string.Empty;



	[Export]
	public PlacementManager TargPlacement;


	public override void _Ready()
	{
		inventoryContainer = GetNode<GridContainer>("%GridContainer");
		TargPlacement = GetParent<PlacementManager>();

		sceneFile = GD.Load<PackedScene>("res://Scenes/Placement Screen/building_ui_button.tscn");

		foreach (Node child in inventoryContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var (buildingType, buildPacked) in PlayerData.Instance.Buildings) 
{
			// Fetch cost/texture from a lightweight data source instead of instantiating the whole scene tree
			int cost = buildPacked.Cost; 
			Texture2D texture = buildPacked.Texture;

			BuildingUiButton sceneInstance = sceneFile.Instantiate<BuildingUiButton>();
			sceneInstance.setText($"{cost}\n{buildingType}");
			sceneInstance.setTexture(texture);
			sceneInstance.setBuildingType(buildingType);
			inventoryContainer.AddChild(sceneInstance);

			//For disabling you knwo shop unlcoks
			sceneInstance.GetNode<Button>("%Button").Disabled = !PlayerData.Instance.isBuildingUnlockedDict[buildingType];

			sceneInstance.GetNode<Button>("%Button").Pressed += () =>
			{
				TargPlacement.SelectBuilding(buildingType.ToString(), (Vector2I)buildPacked.Size, buildPacked.BuildingScene);
			};
		}

		var continue_button = GetNode<Button>("%Continue");
		continue_button.Pressed += OnButtonPressed;

	}

	private void OnButtonPressed()
	{
		PlayerData.Instance.placementToGameplayTransition();
	}


}
