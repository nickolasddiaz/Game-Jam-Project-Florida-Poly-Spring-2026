using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Day
{
	[JsonPropertyName("unlockable_building")]
	public BuildingType UnlockableBuilding { get; set; }

	[JsonPropertyName("product_needs")]
	public Dictionary<ProductType, int> ProductNeeds { get; set; }

	[JsonPropertyName("time")]
	public float Time { get; set; }

	[JsonPropertyName("number_of_money")]
	public int NumberOfMoneyGiven { get; set; }

	public Day() { }

	public Day(BuildingType unlockableBuilding, Dictionary<ProductType, int> productNeeds, float time, int numberOfMoneyGiven)
	{
		UnlockableBuilding = unlockableBuilding;
		ProductNeeds = productNeeds;
		Time = time;
		NumberOfMoneyGiven = numberOfMoneyGiven;
	}
}

public partial class DayInfo : Node
{
	public static DayInfo Instance { get; private set; }

	[Export(PropertyHint.File, "*.json")]
	public string JsonDayFile { get; set; }
	public List<Day> Days;
	public override void _Ready()
	{
		Instance = this; // setting DayInfo static
		LoadDayInfo(JsonDayFile);
	}
	
	public int lastday()
	{
		return Days.Count;
	}
	private void LoadDayInfo(string filePath)
	{
		if (!FileAccess.FileExists(filePath))
		{
			throw new Exception($"Config file not found at: {filePath}");
		}

		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
		string jsonString = file.GetAsText();

		var options = new JsonSerializerOptions
		{
			Converters = { new JsonStringEnumConverter() },
			UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
			PropertyNameCaseInsensitive = false 
		};

		try
		{
			Days = JsonSerializer.Deserialize<List<Day>>(jsonString, options) 
					  ?? throw new JsonException("Deserialized result was null.");
					  
		}
		catch (JsonException ex)
		{
			throw new JsonException($"[CRITICAL] Failed to parse DayInfo JSON: {ex.Message}");
		}
	}
}
