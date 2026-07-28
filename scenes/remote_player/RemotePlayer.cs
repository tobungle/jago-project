using Godot;
using System;

public partial class RemotePlayer : Node3D
{
	[Export] Node3D gfx_base;
	[Export] AnimationPlayer animator;
	[Export] Label3D player_label;
	Node network;
	int id;
	int animation_playing;
	Vector3 intended_position;
	public override void _Ready()
	{
		// When a remote player is spawned, their id is assigned as the node name
		id = Convert.ToInt32(Name);
		// Get network autload
		network = GetNode<Node>("/root/Network");
		// Connect the signal to a lambda which passes the argument to OnPlayerSync
		// Why cant i just do this directly? Dont fucking know Fuck you Eat shit
        network.Connect("on_player_synced", Callable.From((int player_id, Vector3 new_position, float new_y_rot, int new_animation_playing) => OnPlayerSync(player_id, new_position, new_y_rot, new_animation_playing)));
		SetPlayerLabel();
	}

    public override void _PhysicsProcess(double delta)
    {
		DoGraphics();
		GlobalPosition = GlobalPosition.Lerp(intended_position, 0.75f);
    }

	// Function that syncs player on client
	// Right now just syncs position
	void OnPlayerSync(int player_id, Vector3 new_position, float new_y_rot, int new_animation_playing)
	{
		if (player_id == id)
		{
			intended_position = new_position;
		}
		Vector3 rotation = gfx_base.RotationDegrees;
		rotation.Y = new_y_rot;
		gfx_base.RotationDegrees = rotation;
		animation_playing = new_animation_playing;
	}

	void SetPlayerLabel()
	{
		player_label.Text = (string) network.Call("get_player_steam_name", id);
	}

	void DoGraphics()
	{
		switch (animation_playing)
		{
			case (int) Player.PlayerAnimation.Idle:
			animator.Play("Humanoid Idle");
			break;
			case (int) Player.PlayerAnimation.Moving:
			animator.Play("Humanoid Run");
			break;
		}
	}
}
