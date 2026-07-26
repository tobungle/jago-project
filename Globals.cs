using Godot;

// This is an autoload. Access via GetNode<Globals>("/root/Globals")
// 'Global state is bad' - statements dreamed up by the utterly deranged

public partial class Globals : Node
{
	public double world_item_spin_timer = 0.0;
	public Interactable hovered_interactable;

	public override void _PhysicsProcess(double delta)
	{
		world_item_spin_timer += delta * WorldItem.spin_rate;
	}
}
