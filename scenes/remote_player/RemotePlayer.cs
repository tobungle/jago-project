using Godot;
using System;

public partial class RemotePlayer : Node3D
{
	[Export] Node3D gfx_base;
	[Export] AnimationPlayer animator;
	[Export] Label3D player_label;
	Node network;
	int id;
	Vector3 last_frame_position;
	public override void _Ready()
	{
		// When a remote player is spawned, their id is assigned as the node name
		id = Convert.ToInt32(Name);
		// Get network autload
		network = GetNode<Node>("/root/Network");
		// Connect the signal to a lambda which passes the argument to OnPlayerSync
		// Why cant i just do this directly? Dont fucking know Fuck you Eat shit
        network.Connect("on_player_synced", Callable.From((int player_id, Vector3 new_position, float new_y_rot) => OnPlayerSync(player_id, new_position, new_y_rot)));
		SetPlayerLabel();
	}

    public override void _PhysicsProcess(double delta)
    {
		DoGraphics();
        last_frame_position = GlobalPosition;
    }

	// Function that syncs player on client
	// Right now just syncs position
	void OnPlayerSync(int player_id, Vector3 new_position, float new_y_rot)
	{
		if (player_id == id)
		{
			GlobalPosition = new_position;
		}
		Vector3 rotation = gfx_base.RotationDegrees;
		rotation.Y = new_y_rot;
		gfx_base.RotationDegrees = rotation;
	}

	void SetPlayerLabel()
	{
		player_label.Text = (string) network.Call("get_player_steam_name", id);
	}

	void DoGraphics()
	{
		if (last_frame_position == GlobalPosition)
		{
			animator.Play("Humanoid Idle");
		}
		else
		{
			animator.Play("Humanoid Run");
		}
	}
}
