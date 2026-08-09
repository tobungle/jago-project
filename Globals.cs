using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text.Json;
using Godot;

// This is an autoload. Access via GetNode<Globals>("/root/Globals")
// 'Global state is bad' - statements dreamed up by the utterly deranged

public partial class Globals : Node
{
	public struct ItemDef
	{
		[JsonPropertyName("display_name")]
		public string display_name { get; set; }

		[JsonPropertyName("description")]
		public string description { get; set; }

		[JsonPropertyName("craft_tags")]
		public string[] craft_tags { get; set; }

		// If u just deserialize it to a raw number it breaks. Why? Don't know. So in items.json save numbers as strings, ie. "3" instead of 3.
		[JsonPropertyName("value")]
		[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
		public int value { get; set; }

		[JsonPropertyName("melee_damage")]
		[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
		public int melee_damage { get; set; }
	}
	public Dictionary<string, ItemDef> item_defs;
	public double world_item_spin_timer = 0.0;
	public Interactable hovered_interactable;

    public override void _Ready()
    {
		LoadItemData();
		GD.Print(item_defs["stone"].description);
    }

	public override void _PhysicsProcess(double delta)
	{
		world_item_spin_timer += delta * WorldItem.spin_rate;
	}

	void LoadItemData()
	{
		Json json = GD.Load<Json>("res://items.json");
		string json_string = Json.Stringify(json.Data);
		item_defs = JsonSerializer.Deserialize<Dictionary<string, ItemDef>>(json_string);
	}
}
