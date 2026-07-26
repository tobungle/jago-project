using Godot;

// All purpose global variable, put yer globals here
// 'Global state is bad' - statements dreamed up by the utterly deranged

public partial class Globals : Node
{
	public double world_item_spin_timer = 0.0;

	public override void _PhysicsProcess(double delta)
	{
		world_item_spin_timer += delta * WorldItem.spin_rate;
	}
}
