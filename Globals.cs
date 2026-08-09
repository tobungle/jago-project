using Godot;

// This is an autoload. Access via GetNode<Globals>("/root/Globals")
// 'Global state is bad' - statements dreamed up by the utterly deranged

public partial class Globals : Node
{
	public double world_item_spin_timer = 0.0;
	public Interactable hovered_interactable;
	public Godot.Collections.Dictionary<Variant, Variant> item_data;

    public override void _Ready()
    {
		LoadItemData();
    }

	public override void _PhysicsProcess(double delta)
	{
		world_item_spin_timer += delta * WorldItem.spin_rate;
	}

	void LoadItemData()
	{
		Json json = GD.Load<Json>("res://items.json");
		string json_string = Json.Stringify(json.Data);
		item_data = (Godot.Collections.Dictionary<Variant, Variant>) GD.StrToVar(json_string);
	}
}
