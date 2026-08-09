using System.Collections.Generic;
using System.ComponentModel;
using Godot;

public struct Item
{
    public string type;    // <- Points back to item type referred to in res://items.json :3
    public int quantity;
}

public partial class Inventory : Node
{
    [Signal] public delegate int ItemAddedEventHandler();
    [Signal] public delegate int ItemRemovedEventHandler();
    [Signal] public delegate int[] ItemQuantityChangedEventHandler();

    public readonly List<Item> items = new();
    Globals globals;

    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");    // For item db access
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < items.Count; i ++)
        {
            Item other_item = items[i];
            if (items[i].type == item.type)
            {
                other_item.quantity ++;
                items[i] = other_item;
                EmitSignal(SignalName.ItemQuantityChanged, i, items[i].quantity);
                return;
            }
        }
        items.Add(item);
        EmitSignal(SignalName.ItemAdded, items.Count);
    }

    public void DecrementItemAt(int index)
    {
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
