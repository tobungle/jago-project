using System.Collections.Generic;
using Godot;

public struct Item
{
    public string type;    // <- Points back to item type referred to in res://items.json :3
    public int quantity;
}

public partial class Inventory : Node
{
    [Signal] public delegate void ItemAddedEventHandler(int item_index);
    [Signal] public delegate void ItemRemovedEventHandler(int item_index);
    [Signal] public delegate void SpawnWorldItemEventHandler(int at);
    [Signal] public delegate void ItemQuantityChangedEventHandler(int index, int quantity);

    public readonly List<Item> items = new();
    Globals globals;

    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");    // For item db access

        // Add some items for testing :)

        AddItem(
            new Item()
            {
                type = "stick",
                quantity = 8
            }
        );

        AddItem(
            new Item()
            {
                type = "stick",
                quantity = 2
            }
        );

    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < items.Count; i ++)
        {
            Item other_item = items[i];
            if (other_item.type == item.type)
            {
                other_item.quantity += item.quantity;
                items[i] = other_item;
                EmitSignal(SignalName.ItemQuantityChanged, i, other_item.quantity);
                return;
            }
        }
        items.Add(item);
        EmitSignal(SignalName.ItemAdded, items.Count - 1);
    }

    public void DecrementItemAt(int index, bool spawn = false)
    {
        if (spawn)
        {
            EmitSignal(SignalName.SpawnWorldItem, index);
        }
        Item item = items[index];
        if (item.quantity - 1 <= 0)
        {
            items.RemoveAt(index);
            EmitSignal(SignalName.ItemRemoved, index);
            return;
        }
        item.quantity --;
        items[index] = item;
        EmitSignal(SignalName.ItemQuantityChanged, index, items[index].quantity);
    }
}
