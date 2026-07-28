using System;
using Godot;

public partial class Player : CharacterBody3D
{
	public enum PlayerAnimation
	{
		Idle = 0,
		Moving = 1
	}
	public enum OwnershipMode
	{
		Mine,
		Remote
	}
	[Export] AnimationPlayer animator;
	OwnershipMode ownership;
	Node network;
	public int id;

	// Local player vars
	[Export] InteractionManager interaction;
	[Export] PlayerUi player_ui;
	[Export] Label3D name_label;
	[Export] Node3D camera_base;	// Node that the camera is attached to
	[Export] Node3D graphics_base;	// Node that graphics are attached to
	[Export] Camera3D camera;
	float speed = 8.0f;
	float jump_force = 15.0f;
	float gravity = 0.5f;
	float cam_rot_sensitivity = 0.5f;
	Vector2 input_vector;
	Vector2 mouse_relative;
	

	// Remote player vars
	Vector3 intended__position;
	int animation_playing;
	
	public override void _Ready()
	{
		network = GetNode("/root/Network");
		DetermineOwnership();
		UpdateOwnership();
	}

    public override void _Input(InputEvent input_event)
    {
		if (ownership == OwnershipMode.Remote)
		{
			return;
		}
		// Get mouse relative from _Input callback
        if (input_event is InputEventMouseMotion mm)
		{
			mouse_relative = mm.Relative;
		}
    }

	void DetermineOwnership()
	{
		int my_id = Multiplayer.GetUniqueId();
		GD.Print($"Determining player ownership. My id: {my_id}	Player id: {id}");
		if (my_id == id)
		{
			ownership = OwnershipMode.Mine;
		}
		else
		{
			ownership = OwnershipMode.Remote;
		}
	}

	void UpdateOwnership()
	{
		if (ownership == OwnershipMode.Mine)
		{
			camera.Current = true;
			interaction.ProcessMode = ProcessModeEnum.Inherit;
			player_ui.ProcessMode = ProcessModeEnum.Inherit;
			player_ui.Visible = true;
		}
		else
		{
			camera.Current = false;
			interaction.ProcessMode = ProcessModeEnum.Disabled;
			player_ui.ProcessMode = ProcessModeEnum.Disabled;
			player_ui.Visible = false;
			network.Connect("on_player_synced", Callable.From((int player_id, Vector3 new_position, float new_y_rot, int new_animation_playing) => RemoteSync(player_id, new_position, new_y_rot, new_animation_playing)));
		}
		UpdateName();
	}

	void UpdateName()
	{
		if (ownership == OwnershipMode.Mine)
		{
			name_label.Text = (string) network.Get("persona_name");
		}
		else
		{
			string name = (string) network.Call("get_player_steam_name", id);
			GD.Print($"Got player name {name}, id {id}");
			name_label.Text = name == "" ? name : $"Player {id}";
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (ownership == OwnershipMode.Remote)
		{
			DoGraphicsRemote();
			return;
		}
		DoCamera();
		DoMovement();
		Sync();
		DoGraphics();
		mouse_relative = Vector2.Zero;
	}

	void DoCamera()
	{
		if (Input.MouseMode == Input.MouseModeEnum.Visible)
		{
			return;
		}
		// Modify camera rotation based on mouse relative
		Vector3 cam_rot = camera_base.RotationDegrees;
		cam_rot.Y -= mouse_relative.X * cam_rot_sensitivity;
		cam_rot.X -= mouse_relative.Y * cam_rot_sensitivity;
		// Clamp mouse angle to prevent going upside down
		cam_rot.X = Math.Clamp(cam_rot.X, -70f, 30f);
		camera_base.RotationDegrees = cam_rot;
	}
	
	void DoMovement()
	{
		float y_vel = Velocity.Y;
		if (!IsOnFloor())
		{
			y_vel -= gravity;
		}
		else
		{
			y_vel = 0f;
			if (Input.IsActionJustPressed("ui_accept"))
			{
				y_vel = jump_force;
			}
		}
		input_vector = Input.GetVector("left", "right", "forward", "backward");
		Vector2 direction = input_vector.Rotated(-camera_base.Rotation.Y);
		Velocity = new Vector3(direction.X * speed, y_vel, direction.Y * speed);
		MoveAndSlide();
	}

	// Update the player graphcs (rotation, animations etc.)
	void DoGraphics()
	{
		Vector2 direction = input_vector.Rotated(-camera_base.Rotation.Y);
		if (GlobalPosition + new Vector3(direction.X, 0f, direction.Y) != GlobalPosition)
		{
			graphics_base.LookAt(GlobalPosition + new Vector3(direction.X, 0f, direction.Y));
		}
		if (input_vector == Vector2.Zero)
		{
			animator.Play("Humanoid Idle");
		}
		else
		{
			animator.Play("Humanoid Run");
		}
	}

	// Update graphics for the remote player (player we do not own)
	void DoGraphicsRemote()
	{
		GlobalPosition = GlobalPosition.Lerp(intended__position, 0.75f);
		switch (animation_playing)
		{
			case (int) PlayerAnimation.Idle:
			animator.Play("Humanoid Idle");
			break;
			case (int) PlayerAnimation.Moving:
			animator.Play("Humanoid Run");
			break;
		}
	}

	void Sync()
	{
		network.Call("_sync_my_player", GlobalPosition, graphics_base.RotationDegrees.Y, input_vector == Vector2.Zero ? 0 : 1);
	}

	void RemoteSync(int player_id, Vector3 new_position, float new_y_rot, int new_animation_playing)
	{
		if (player_id == id)
		{
			intended__position = new_position;
		}
		Vector3 rotation = graphics_base.RotationDegrees;
		rotation.Y = new_y_rot;
		graphics_base.RotationDegrees = rotation;
		animation_playing = new_animation_playing;
	}

}
