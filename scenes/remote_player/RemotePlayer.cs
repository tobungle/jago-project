using Godot;

public partial class RemotePlayer : Node3D
{
	Node network;
	public override void _Ready()
	{
		network = GetNode<Node>("/root/Network");
		// Connect the signal to a lambda which passes the argument to OnPlayerSync
		// Why cant i just do this directly? Dont fucking know Fuck you Eat shit
        network.Connect("on_player_synced", Callable.From((Vector3 new_position) => OnPlayerSync(new_position)));
	}

	// Function that syncs player on client
	// Right now just syncs position
	void OnPlayerSync(Vector3 new_position)
	{
		GlobalPosition = new_position;
	}
}
