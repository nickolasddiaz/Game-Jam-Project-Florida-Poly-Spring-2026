using Godot;

public enum CropState { Empty, Growing, Ready, Blighted }

// A 1x1 farm tile: interact to plant, wait for it to grow, interact again to harvest.
// Optional "push your luck" soil strain: harvesting again and again without letting the
// soil rest gives bigger yields, but with a growing chance of blight.
//
// Visuals: if the scene has an AnimatedSprite2D child named "BaseSprite" with SpriteFrames
// from spr_cropTile.aseprite, it is used (BASE, BARLEY, CABBAGE, GRAPE animations; frame 0 =
// growing, frame 1 = ready). Otherwise it draws simple colored placeholders.
public partial class CropTile : Building
{
	// Which product this tile grows (barley, cabbage, grape)
	[Export]
	public ProductType CropType { get; set; } = ProductType.barley;

	// Seconds from planting until ready
	[Export]
	public float GrowTime { get; set; } = 10.0f;

	// How many items a normal harvest gives
	[Export]
	public int Yield { get; set; } = 1;

	[ExportGroup("Push your luck: soil strain")]
	[Export]
	public bool SoilStrainEnabled { get; set; } = false;

	// Seconds the tile must sit empty for the strain to reset
	[Export]
	public float RestTime { get; set; } = 15.0f;

	// Seconds the tile is locked after blight
	[Export]
	public float BlightLockTime { get; set; } = 30.0f;

	// Index = consecutive harvests so far without resting
	private static readonly int[] YieldMultiplier = { 1, 2, 4, 6 };
	private static readonly float[] BlightChance = { 0.0f, 0.2f, 0.5f, 0.8f };

	public CropState State { get; private set; } = CropState.Empty;
	public int Strain { get; private set; }

	private AnimatedSprite2D _baseSprite;
	private bool _usingSprites;
	private float _timer;
	private float _restTimer;

	public override void _Ready()
	{
		_baseSprite = GetNodeOrNull<AnimatedSprite2D>("BaseSprite");
		_usingSprites = _baseSprite != null && _baseSprite.SpriteFrames != null;
		Refresh();
	}

	public override void Interact(Player player)
	{
		switch (State)
		{
			case CropState.Empty:
				Plant();
				break;
			case CropState.Ready:
				Harvest();
				break;
			case CropState.Growing:
				GD.Print($"{Name}: still growing ({GrowTime - _timer:0.0}s left)");
				break;
			case CropState.Blighted:
				GD.Print($"{Name}: blighted, locked for {BlightLockTime - _timer:0.0}s more");
				break;
		}
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;

		switch (State)
		{
			case CropState.Growing:
				_timer += dt;
				if (_timer >= GrowTime)
				{
					State = CropState.Ready;
					Refresh();
				}
				else if (!_usingSprites)
				{
					QueueRedraw(); // placeholder color fades as it grows
				}
				break;

			case CropState.Blighted:
				_timer += dt;
				if (_timer >= BlightLockTime)
				{
					State = CropState.Empty;
					Strain = 0;
					_restTimer = 0.0f;
					Refresh();
				}
				break;

			case CropState.Empty:
				if (SoilStrainEnabled && Strain > 0)
				{
					_restTimer += dt;
					if (_restTimer >= RestTime)
					{
						Strain = 0;
						Refresh();
					}
				}
				break;
		}
	}

	private void Plant()
	{
		_timer = 0.0f;
		_restTimer = 0.0f;
		State = CropState.Growing;
		Refresh();
	}

	private void Harvest()
	{
		int level = SoilStrainEnabled ? Mathf.Clamp(Strain, 0, 3) : 0;

		// Push your luck: the more strained the soil, the higher the blight chance
		if (SoilStrainEnabled && GD.Randf() < BlightChance[level])
		{
			State = CropState.Blighted;
			_timer = 0.0f;
			Strain = 0;
			GD.Print($"{Name}: BLIGHT! Crop lost, tile locked for {BlightLockTime}s");
			Refresh();
			return;
		}

		int amount = Yield * YieldMultiplier[level];
		if (PlayerData.Instance != null)
		{
			PlayerData.Instance.productAmounts[CropType] += amount;
			GD.Print($"Harvested {amount} {CropType}. Total: {PlayerData.Instance.productAmounts[CropType]}");
		}

		if (SoilStrainEnabled)
		{
			Strain = Mathf.Min(Strain + 1, 3);
		}

		State = CropState.Empty;
		_timer = 0.0f;
		_restTimer = 0.0f;
		Refresh();
	}

	// Updates whichever visuals are in use
	private void Refresh()
	{
		if (!_usingSprites)
		{
			QueueRedraw();
			return;
		}

		string crop = CropType.ToString().ToUpperInvariant(); // BARLEY / CABBAGE / GRAPE
		switch (State)
		{
			case CropState.Growing:
				SpriteHelper.ShowFrame(_baseSprite, crop, 0);
				break;
			case CropState.Ready:
				SpriteHelper.ShowFrame(_baseSprite, crop, 1);
				break;
			default:
				SpriteHelper.ShowFrame(_baseSprite, "BASE", 0);
				break;
		}

		// No blight sprite yet, so tint the bare soil purple
		_baseSprite.Modulate = State == CropState.Blighted ? new Color(0.6f, 0.3f, 0.7f) : Colors.White;
	}

	// Placeholder visuals: brown = empty, green shades = growing, gold = ready, purple = blighted.
	public override void _Draw()
	{
		if (_usingSprites)
		{
			return;
		}

		Vector2 full = new Vector2(Size.X * 32, Size.Y * 32);
		Rect2 rect = new Rect2(-full / 2, full);
		Color soil = new Color(0.45f, 0.30f, 0.15f);

		Color color;
		switch (State)
		{
			case CropState.Growing:
				float progress = GrowTime > 0 ? Mathf.Clamp(_timer / GrowTime, 0.0f, 1.0f) : 1.0f;
				color = soil.Lerp(new Color(0.2f, 0.7f, 0.2f), progress);
				break;
			case CropState.Ready:
				color = new Color(0.95f, 0.8f, 0.2f);
				break;
			case CropState.Blighted:
				color = new Color(0.35f, 0.15f, 0.4f);
				break;
			default:
				color = soil;
				break;
		}

		DrawRect(rect, color);
		DrawRect(rect, new Color(0, 0, 0, 0.5f), false, 1.0f);

		// Small label so you can tell the crops apart while testing
		DrawString(ThemeDB.FallbackFont, rect.Position + new Vector2(2, 10), CropType.ToString(),
			HorizontalAlignment.Left, rect.Size.X - 4, 8);

		// Strain pips along the bottom (one per consecutive harvest)
		for (int i = 0; i < Strain; i++)
		{
			DrawRect(new Rect2(rect.Position + new Vector2(2 + i * 8, rect.Size.Y - 6), new Vector2(6, 4)),
				new Color(0.9f, 0.2f, 0.2f));
		}
	}
}
