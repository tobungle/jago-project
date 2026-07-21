using System;
using Godot;

public partial class Player : CharacterBody3D
{
	Node network;
	[Export] Label3D name_label;
	[Export] Node3D camera_base;	// Node that the camera is attached to
	[Export] Node3D graphics_base;	// Node that graphics are attached to
	float speed = 8.0f;
	float jump_force = 15.0f;
	float gravity = 0.5f;
	float cam_rot_sensitivity = 0.5f;
	Vector2 input_vector;
	Vector2 mouse_relative;	// Where is mouse, relative to last frame. Basically how much it's moving
	public override void _Ready()
	{
		// Get reference to the Network node
		network = GetNode("/root/Network");
		// Set the player's in-game name to their Steam name
		// Because network is a GDscript class, properties need to be retrieved with the Get() method
		name_label.Text = (string)network.Get("persona_name");
	}

    public override void _Input(InputEvent input_event)
    {
		// Get mouse relative from _Input callback
        if (input_event is InputEventMouseMotion mm)
		{
			mouse_relative = mm.Relative;
		}
    }

	public override void _PhysicsProcess(double delta)
	{
		DoCamera();
		DoMovement();
		DoGraphics();
		// Reset the mouse movement at the end of each frame. Without this, camera will just keep moving based on last input
		// - regardless if the player is moving their mouse or not. I'm gay
		mouse_relative = Vector2.Zero;
	}

	void DoCamera()
	{
		bool rotating = Input.IsActionPressed("rotate_camera");
		// Update mouse visibility & lock based on whether we're rotating or not
		if (rotating)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		// Escape from function if not holding down rotate button or if the mouse isn't moving
		if (!rotating || mouse_relative == Vector2.Zero)
		{
			return;
		}
		// Modify camera rotation based on mouse relative
		Vector3 cam_rot = camera_base.RotationDegrees;
		cam_rot.Y -= mouse_relative.X * cam_rot_sensitivity;
		cam_rot.X -= mouse_relative.Y * cam_rot_sensitivity;
		// Clamp mouse angle to prevent going upside down
		cam_rot.X = Math.Clamp(cam_rot.X, -50f, 30f);
		camera_base.RotationDegrees = cam_rot;
	}
	
	void DoMovement()
	{
		// Handle y velocity
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
		// Get input vector from mouse keys then modify based on camera rotation
		input_vector = Input.GetVector("left", "right", "forward", "backward");
		Vector2 direction = input_vector.Rotated(-camera_base.Rotation.Y);
		// Apply velocity
		Velocity = new Vector3(direction.X * speed, y_vel, direction.Y * speed);
		// Tell engine to do movement shit based on velocity
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
		
	}

}
