using Godot;

public partial class InteractionManager : Node3D
{
	[Export] Camera3D camera;	// Needed for camera raycast
	Globals globals;
	const float ray_length = 300f;
	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
	}

	public override void _Input(InputEvent input)
	{
		if (input.IsActionPressed("get_item"))
		{
			OnGetItemPressed();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		RaycastFromCamera();
	}

	void OnGetItemPressed()
	{
		globals.hovered_interactable?.Interact();
	}

	void RaycastFromCamera()
	{
		// Project ray from camera to infront of camera
		// Thank you random stack overflow user
		Vector2 mouse_pos = GetViewport().GetMousePosition();
		Vector3 origin = camera.ProjectRayOrigin(mouse_pos);
		var end = origin + camera.ProjectRayNormal(mouse_pos) * ray_length;

		// Do le raycast
		var query = PhysicsRayQueryParameters3D.Create(origin, end);
		query.CollideWithAreas = true;
		// Mask layer 4 (items) and layer 1 (world)
		query.CollisionMask = 0b_00001001;
		PhysicsDirectSpaceState3D space_state = GetWorld3D().DirectSpaceState;
		Godot.Collections.Dictionary result = space_state.IntersectRay(query);
		if (result.Count == 0)
		{
			return;
		}
		Node3D collider = (Node3D) result["collider"];
		if (collider is InteractArea interact_area)
		{
			globals.hovered_interactable = interact_area.interactable;
		}
		else
		{
			globals.hovered_interactable = null;
		}
	}
}
