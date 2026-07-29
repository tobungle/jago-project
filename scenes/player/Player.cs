using System;
using Godot;

public partial class Player : CharacterBody3D, Syncable
{
	public enum PlayerAnimation
	{
		Idle = 0,
		Moving = 1
	}
	string animation = "NONE";
	public enum OwnershipMode
	{
		Mine,
		Remote
	}
	[Export] AnimationTree anim_tree;
	[Export] AnimationPlayer animator;
	OwnershipMode ownership;
	Node network;
	int _server_id;
    public int server_id
	{
	get
		{
			return _server_id;
		}
	set
		{
			_server_id = value;
		}
	}

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
	float intended_angle;
	int animation_playing;
	
	public override void _Ready()
	{
		network = GetNode("/root/Network");
		DetermineOwnership();
		UpdateOwnership();
		ChangeAnimation("Idle");
		// QUICK FIX TO GET FUCKING SHITTY STATE MACHINE WORKING FUCK YOU
		// clean this UP TOMORROW IS MY CAPS LOCK EVEN FUCKING ON RAAGGGHH
		anim_tree.AnimationFinished += (StringName anim) =>
		{
			if (anim == new StringName("SwingAttack"))
			{
				AnimationNodeStateMachinePlayback state_machine = (AnimationNodeStateMachinePlayback) anim_tree.Get("parameters/playback");
				state_machine.Travel(input_vector == Vector2.Zero ? "Idle" : "Run");
			}
		};
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
		if (input_event.IsActionPressed("melee_atk"))
		{
			DoMeleeAttack();
		}
    }

	void DetermineOwnership()
	{
		int my_id = Multiplayer.GetUniqueId();
		if (my_id == server_id)
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
			string name = (string) network.Call("get_player_steam_name", server_id);
			GD.Print($"Got player name {name}, id {server_id}");
			name_label.Text = name == "" ? name : $"Player {server_id}";
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

		// idk what an arc tangent is  but internet tells me this is how you get angle to direction
		if (input_vector != Vector2.Zero)
		{
			float angle = Mathf.Atan2(-direction.X, -direction.Y);
			Vector3 g_rot = graphics_base.Rotation;
			g_rot.Y = Mathf.LerpAngle(graphics_base.Rotation.Y, angle, 0.25f);
			graphics_base.Rotation = g_rot;
		}


		if (input_vector == Vector2.Zero)
		{
			anim_tree.Set("parameters/RunSwingBlend/Blend2/blend_amount", 0f);
			ChangeAnimation("Idle");
		}
		else
		{
			anim_tree.Set("parameters/RunSwingBlend/Blend2/blend_amount", 1f);
			AnimationNodeStateMachinePlayback state_machine = (AnimationNodeStateMachinePlayback) anim_tree.Get("parameters/playback");
			StringName state = state_machine.GetCurrentNode();
			if (state != "RunSwingBlend")
			{
				ChangeAnimation("Run");
			}
		}
	}

	void DoMeleeAttack()
	{
		AnimationNodeStateMachinePlayback state_machine = (AnimationNodeStateMachinePlayback) anim_tree.Get("parameters/playback");
		state_machine.Travel("Idle");
		state_machine.Travel("RunSwingBlend");
	}

	// Update graphics for the remote player (player we do not own)
	void DoGraphicsRemote()
	{
		GlobalPosition = GlobalPosition.Lerp(intended__position, 0.75f);
		
		Vector3 g_rot = graphics_base.Rotation;
		g_rot.Y = Mathf.LerpAngle(graphics_base.Rotation.Y, intended_angle, 0.25f);
		graphics_base.Rotation = g_rot;
		
		switch (animation_playing)
		{
			case (int) PlayerAnimation.Idle:
			ChangeAnimation("Idle");
			break;
			case (int) PlayerAnimation.Moving:
			ChangeAnimation("Run");
			break;
		}
	}

	void Sync()
	{
		network.Call("_sync_my_player", GlobalPosition, graphics_base.Rotation.Y, input_vector == Vector2.Zero ? 0 : 1);
	}

	void RemoteSync(int player_id, Vector3 new_position, float new_y_rot, int new_animation_playing)
	{
		if (player_id == server_id)
		{
			intended__position = new_position;
			intended_angle = new_y_rot;
		}
		animation_playing = new_animation_playing;
	}

	// This is safe to call in _PhysicsProcess cos it only calls it once
	void ChangeAnimation(string new_anim)
	{
		if (new_anim == animation)
		{
			return;
		}
		AnimationNodeStateMachinePlayback state_machine = (AnimationNodeStateMachinePlayback) anim_tree.Get("parameters/playback");
		state_machine.Travel(new_anim);
		animation = new_anim;
	}

}
