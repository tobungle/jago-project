using Godot;

public partial class ItemListing : Control
{
	[Signal] public delegate void DropOnePressedEventHandler(int at);
	[Signal] public delegate void DropAllPressedEventHandler(int at);
	[Export] Label item_name_label;
	[Export] Button drop_1_btn;
	[Export] Button drop_all_btn;
	Globals globals;

    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");
		drop_1_btn.Pressed += DropOne;
		drop_all_btn.Pressed += DropAll;
    }

	public void SetItem(Item item)
	{
		string quant_string = "";
		if (item.quantity > 1)
		{
			quant_string = $" x{item.quantity}";
		}
		item_name_label.Text = $"{globals.item_defs[item.type].display_name}{quant_string}";
	}

	void DropOne()
	{
		EmitSignal(SignalName.DropOnePressed, GetIndex());
	}

	void DropAll()
	{
		EmitSignal(SignalName.DropAllPressed, GetIndex());
	}
}
