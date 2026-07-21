using Godot;

public partial class Player : CharacterBody3D
{
	float speed = 8.0f;
	float jump_force = 15.0f;
	float gravity = 0.5f;
	public override void _Ready()
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		DoMovement();
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
		var input = Input.GetVector("left", "right", "forward", "backward").Rotated(-Rotation.Y);
		Velocity = new Vector3(input.X * speed, y_vel, input.Y * speed);
		MoveAndSlide();
	}

}
