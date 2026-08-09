using Godot;

public partial class ItemListing : Control
{
	[Export] Label item_name_label;
	[Export] Button drop_1_btn;
	[Export] Button drop_all_btn;
	Globals globals;

    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");
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
}
