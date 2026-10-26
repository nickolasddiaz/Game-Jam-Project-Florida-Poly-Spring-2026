using Godot;

// One item slot sprite (16x16). Uses the item SpriteFrames from spr_items.aseprite,
// where each item is a one-frame animation: BARLEY, GRAPES, MILK, WOOL, BREAD, ALE, WINE.
// Derives from AnimationWrapperModule as Nico suggested.
public partial class ItemSprite : AnimationWrapperModule
{
	public override void _Ready()
	{
		Visible = false;
	}

	public void ShowItem(ProductType type)
	{
		string animation = AnimationFor(type);
		if (animation == null || !SpriteHelper.ShowFrame(this, animation))
		{
			// No sprite for this item (or frames not set up yet)
			Visible = false;
			return;
		}
		Visible = true;
	}

	public void HideItem()
	{
		Visible = false;
	}

	// ProductType -> animation (tag) name in spr_items.aseprite. There is no cabbage sprite yet.
	private static string AnimationFor(ProductType type)
	{
		switch (type)
		{
			case ProductType.Barley: return "BARLEY";
			case ProductType.Grape: return "GRAPES";
			case ProductType.Milk: return "MILK";
			case ProductType.Wool: return "WOOL";
			case ProductType.Bread: return "BREAD";
			case ProductType.Ale: return "ALE";
			case ProductType.Wine: return "WINE";
			default: return null;
		}
	}
}
