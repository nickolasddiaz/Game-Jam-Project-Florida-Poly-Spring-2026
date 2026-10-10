using System;
using System.Collections.Generic;
using Godot;

public enum ProductionState { Idle, Working }

// One production recipe. Inputs has one entry per item (max 3, matching the 3 feed slots).
public sealed class Recipe
{
	public ProductType Output;
	public float Time;
	public List<ProductType> Inputs;

	public Recipe(ProductType output, float time, params ProductType[] inputs)
	{
		Output = output;
		Time = time;
		Inputs = new List<ProductType>(inputs);
	}
}

// Animal pens, bread oven and distillery all use this one script. Set "Kind" in the Inspector.
//   LEFT-click  the building -> feed it (loads ingredients from the inventory and starts a cycle)
//   RIGHT-click the building -> collect finished products into the inventory
//
// Visuals (all optional, found by child node name):
//   BaseSprite            AnimatedSprite2D with the building's SpriteFrames
//   Feed1, Feed2, Feed3   ItemSprite nodes showing the ingredients being processed
//   Product1..Product3    ItemSprite nodes showing finished products waiting to be collected
// If BaseSprite has no SpriteFrames yet, simple colored placeholders are drawn instead.
public partial class ProductionBuilding : Building
{
	public const int MaxSlots = 3;

	[Export]
	public BuildingType Kind { get; set; } = BuildingType.sheep_pen;

	// Multiplies every recipe time (use to balance a single building)
	[Export]
	public float TimeMultiplier { get; set; } = 1.0f;

	public ProductionState State { get; private set; } = ProductionState.Idle;

	private Recipe[] _recipes = Array.Empty<Recipe>();
	private Recipe _current;
	private float _timer;
	private ProductType _outputType;
	private int _outputCount;

	private AnimatedSprite2D _baseSprite;
	private readonly List<ItemSprite> _feedSlots = new List<ItemSprite>();
	private readonly List<ItemSprite> _productSlots = new List<ItemSprite>();
	private bool _usingSprites;

	public override void _Ready()
	{
		_recipes = RecipesFor(Kind);
		if (_recipes.Length == 0)
		{
			GD.PushWarning($"{Name}: Kind {Kind} has no recipes (is it a production building?)");
		}

		_baseSprite = GetNodeOrNull<AnimatedSprite2D>("BaseSprite");
		_usingSprites = _baseSprite != null && _baseSprite.SpriteFrames != null;

		for (int i = 1; i <= MaxSlots; i++)
		{
			var feed = GetNodeOrNull<ItemSprite>($"Feed{i}");
			if (feed != null)
			{
				_feedSlots.Add(feed);
			}

			var product = GetNodeOrNull<ItemSprite>($"Product{i}");
			if (product != null)
			{
				_productSlots.Add(product);
			}
		}

		UpdateVisuals();
	}

	// ----- Recipes -----------------------------------------------------------
	// Wine is listed before ale: if the player holds grapes, wine is made.
	private static Recipe[] RecipesFor(BuildingType kind)
	{
		const ProductType barley = ProductType.barley;
		const ProductType grape = ProductType.grape;

		switch (kind)
		{
			case BuildingType.sheep_pen:
				return new[] { new Recipe(ProductType.wool, 8.0f, barley, barley, barley) };
			case BuildingType.cow_pen:
				return new[] { new Recipe(ProductType.milk, 8.0f, barley, barley, barley) };
			case BuildingType.bread_oven:
				return new[] { new Recipe(ProductType.bread, 10.0f, barley, barley, barley) };
			case BuildingType.distillary:
				return new[]
				{
					new Recipe(ProductType.wine, 15.0f, grape, grape, barley),
					new Recipe(ProductType.ale, 12.0f, barley, barley),
				};
			default:
				return Array.Empty<Recipe>();
		}
	}

	// ----- Interaction -------------------------------------------------------
	// LEFT-click: feed the building
	public override void Interact(Player player)
	{
		if (State == ProductionState.Working)
		{
			float left = _current.Time * TimeMultiplier - _timer;
			GD.Print($"{Name}: busy ({left:0.0}s left)");
		}
		else if (_outputCount >= MaxSlots)
		{
			GD.Print($"{Name}: product storage full, right-click to collect first");
		}
		else
		{
			Recipe recipe = PickAffordableRecipe();
			if (recipe != null)
			{
				StartRecipe(recipe);
			}
			else
			{
				GD.Print($"{Name}: not enough ingredients");
			}
		}

		UpdateVisuals();
	}

	// RIGHT-click: collect finished products
	public override void InteractSecondary(Player player)
	{
		if (!Collect())
		{
			GD.Print($"{Name}: nothing to collect");
		}
		UpdateVisuals();
	}

	private bool Collect()
	{
		if (_outputCount <= 0 || PlayerData.Instance == null)
		{
			return false;
		}

		PlayerData.Instance.productAmounts[_outputType] += _outputCount;
		GD.Print($"{Name}: collected {_outputCount} {_outputType}. Total: {PlayerData.Instance.productAmounts[_outputType]}");
		_outputCount = 0;
		return true;
	}

	private Recipe PickAffordableRecipe()
	{
		foreach (Recipe recipe in _recipes)
		{
			// Don't mix products: if something is waiting, only make more of the same
			if (_outputCount > 0 && recipe.Output != _outputType)
			{
				continue;
			}

			if (CanAfford(recipe))
			{
				return recipe;
			}
		}
		return null;
	}

	private static bool CanAfford(Recipe recipe)
	{
		if (PlayerData.Instance == null)
		{
			return false;
		}

		var needed = new Dictionary<ProductType, int>();
		foreach (ProductType item in recipe.Inputs)
		{
			needed[item] = needed.GetValueOrDefault(item) + 1;
		}

		foreach (var pair in needed)
		{
			PlayerData.Instance.productAmounts.TryGetValue(pair.Key, out int have);
			if (have < pair.Value)
			{
				return false;
			}
		}
		return true;
	}

	private void StartRecipe(Recipe recipe)
	{
		foreach (ProductType item in recipe.Inputs)
		{
			PlayerData.Instance.productAmounts[item] -= 1;
		}

		_current = recipe;
		_timer = 0.0f;
		State = ProductionState.Working;
		GD.Print($"{Name}: started making {recipe.Output} ({recipe.Time * TimeMultiplier:0.0}s)");
	}

	// ----- Production --------------------------------------------------------
	public override void _Process(double delta)
	{
		if (State != ProductionState.Working || _current == null)
		{
			return;
		}

		_timer += (float)delta;

		if (_timer >= _current.Time * TimeMultiplier)
		{
			_outputType = _current.Output;
			_outputCount = Math.Min(_outputCount + 1, MaxSlots);
			GD.Print($"{Name}: {_outputType} is ready");
			_current = null;
			_timer = 0.0f;
			State = ProductionState.Idle;
			UpdateVisuals();
			return;
		}

		if (!_usingSprites)
		{
			QueueRedraw(); // progress bar
		}
	}

	// ----- Visuals -----------------------------------------------------------
	private void UpdateVisuals()
	{
		if (!_usingSprites)
		{
			QueueRedraw();
			return;
		}

		SpriteHelper.PlayOrShow(_baseSprite, BaseAnimation());

		for (int i = 0; i < _feedSlots.Count; i++)
		{
			if (_current != null && i < _current.Inputs.Count)
			{
				_feedSlots[i].ShowItem(_current.Inputs[i]);
			}
			else
			{
				_feedSlots[i].HideItem();
			}
		}

		for (int i = 0; i < _productSlots.Count; i++)
		{
			if (i < _outputCount)
			{
				_productSlots[i].ShowItem(_outputType);
			}
			else
			{
				_productSlots[i].HideItem();
			}
		}
	}

	// Which animation (Aseprite tag) the building sprite should show right now.
	// Pen animations come from PenFrames.tres; oven/distillery tags from their .aseprite files.
	private string BaseAnimation()
	{
		bool working = State == ProductionState.Working;

		switch (Kind)
		{
			case BuildingType.sheep_pen:
				// Wool is in the product slot while it waits, so the sheep is shown sheared
				return _outputCount > 0 ? "SHEEP_NO_WOOL" : "SHEEP_WOOL";
			case BuildingType.cow_pen:
				return "COW";
			case BuildingType.bread_oven:
				return working ? "BAKING" : "EMPTY";
			case BuildingType.distillary:
				if (working)
				{
					return _current != null && _current.Output == ProductType.wine ? "WINE_BREWING" : "ALE_BREWING";
				}
				return "EMPTY";
			default:
				return "EMPTY";
		}
	}

	private static Color ColorFor(ProductType type)
	{
		return Color.FromHsv((int)type / 8.0f, 0.7f, 0.95f);
	}

	// Placeholder drawing: brown = idle, orange = working, small squares = items.
	public override void _Draw()
	{
		if (_usingSprites)
		{
			return;
		}

		Vector2 full = new Vector2(Size.X * 32, Size.Y * 32);
		Rect2 rect = new Rect2(-full / 2, full);

		Color body = State == ProductionState.Working ? new Color(0.85f, 0.55f, 0.2f) : new Color(0.55f, 0.4f, 0.25f);
		DrawRect(rect, body);
		DrawRect(rect, new Color(0, 0, 0, 0.5f), false, 1.0f);

		DrawString(ThemeDB.FallbackFont, rect.Position + new Vector2(2, rect.Size.Y / 2 + 4), Kind.ToString(),
			HorizontalAlignment.Left, rect.Size.X - 4, 8);

		// Progress bar along the bottom while working
		if (State == ProductionState.Working && _current != null)
		{
			float total = _current.Time * TimeMultiplier;
			float p = total > 0 ? Mathf.Clamp(_timer / total, 0.0f, 1.0f) : 1.0f;
			DrawRect(new Rect2(rect.Position + new Vector2(2, rect.Size.Y - 6), new Vector2((rect.Size.X - 4) * p, 4)),
				Colors.White);
		}

		// Feed items (ingredients being processed) top-left
		if (_current != null)
		{
			for (int i = 0; i < _current.Inputs.Count; i++)
			{
				DrawRect(new Rect2(rect.Position + new Vector2(2 + i * 10, 2), new Vector2(8, 8)), ColorFor(_current.Inputs[i]));
			}
		}

		// Finished products top-right
		for (int i = 0; i < _outputCount; i++)
		{
			DrawRect(new Rect2(new Vector2(rect.End.X - 10 - i * 10, rect.Position.Y + 2), new Vector2(8, 8)), ColorFor(_outputType));
		}
	}
}
