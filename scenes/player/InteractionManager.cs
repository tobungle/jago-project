using Godot;

public partial class InteractionManager : Node3D
{
	[Export] Camera3D camera;	// Needed for camera raycast
	Globals globals;
	const float ray_length = 300f;
	bool get_item_held = false;
	double get_item_held_time;
	const double pickup_time = 0.1;
	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
	}

	public override void _Input(InputEvent input)
	{
		if (input.IsActionPressed("get_item"))
		{
			get_item_held = true;
		}
		if (input.IsActionReleased("get_item"))
		{
			get_item_held = false;
			ItemHeldReleased();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (get_item_held)
		{
			get_item_held_time += delta;
		}
		else
		{
			get_item_held_time = 0.0;
		}
		RaycastFromCamera();
	}

	void ItemHeldReleased()
	{
		if (get_item_held_time <= pickup_time)
		{
			OnGetItemPressed();
		}
	}

	void OnGetItemPressed()
	{
		globals.hovered_interactable?.Interact();
		// Check if is queued for deletion after interaction and handle
		Node3D interactable_node = globals.hovered_interactable as Node3D;
		if (interactable_node == null || interactable_node.IsQueuedForDeletion())
		{
			globals.hovered_interactable = null;
		}
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

	// DO NOT USE THIS
	void TryDragHoveredItem()
	{
		if (globals.hovered_interactable != null)
		{
			float distance = 9.0f;
			Vector3 camera_forward = -camera.GlobalTransform.Basis.Z;
			Vector3 position_in_front = camera.GlobalPosition + camera_forward * distance;
			
			Node3D interactable_node = globals.hovered_interactable as Node3D;
			interactable_node.GlobalPosition = position_in_front;
		}
	}
}
