using Godot;
using System;

public partial class RemotePlayer : Node3D
{
	[Export] Label3D player_label;
	Node network;
	int id;
	public override void _Ready()
	{
		// When a remote player is spawned, their id is assigned as the node name
		id = Convert.ToInt32(Name);
		// Get network autload
		network = GetNode<Node>("/root/Network");
		// Connect the signal to a lambda which passes the argument to OnPlayerSync
		// Why cant i just do this directly? Dont fucking know Fuck you Eat shit
        network.Connect("on_player_synced", Callable.From((int player_id, Vector3 new_position) => OnPlayerSync(player_id, new_position)));
		SetPlayerLabel();
	}

	// Function that syncs player on client
	// Right now just syncs position
	void OnPlayerSync(int player_id, Vector3 new_position)
	{
		if (player_id == id)
		{
			GlobalPosition = new_position;
		}
	}

	void SetPlayerLabel()
	{
		player_label.Text = (string) network.Call("get_player_steam_name");
	}
}
