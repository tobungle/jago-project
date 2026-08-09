using System.Text.Json.Serialization;
using System.Collections.Generic;
using Godot;

public struct ItemDef
{
    [JsonPropertyName("display_name")]
    public string DisplayName { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("craft_tags")]
    public string[] CraftTags { get; init; }

    [JsonPropertyName("value")]
    public int Value { get; init; }

    [JsonPropertyName("melee_damage")]
    public int MeleeDamage { get; init; }
}

public struct Item
{
    public string type;    // <- Points back to item type referred to in res://items.json :3
    public int quantity;
}

public partial class Inventory : Node
{
    List<Item> items = new();
    Globals globals;

    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");    // For item db access
    }

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }

    public void RemoveItemAt(int index)
    {
        items.RemoveAt(index);
    }
}
