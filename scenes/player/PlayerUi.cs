using Godot;

public partial class PlayerUi : CanvasLayer
{
	[Export] GridContainer inv_grid;
	[Export] Camera3D player_camera;	// Needed for unprojecting postiions
	[Export] Vector2 hover_prompt_offset;
	[Export] Label hover_prompt;
	Globals globals;
	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
		inv_grid.Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    public override void _Input(InputEvent input)
    {
        if (input.IsActionPressed("toggle_inv"))
		{
			ToggleInv();
		}
    }

	public override void _PhysicsProcess(double delta)
	{
		// Delay this by one frame else it throws disposed object errors
		// I have no idea why and cant be bothered to find out why
		CallDeferred("UpdateHoverPrompt");
	}

	void ToggleInv()
	{
		inv_grid.Visible = !inv_grid.Visible;
		if (inv_grid.Visible)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	void UpdateHoverPrompt()
	{
		if (globals.hovered_interactable == null)
		{
			hover_prompt.Text = "";
		}
		else
		{
			Node3D interactable_node = globals.hovered_interactable as Node3D;
			hover_prompt.Position = player_camera.UnprojectPosition(interactable_node.GlobalPosition) + hover_prompt_offset;
			hover_prompt.Text = globals.hovered_interactable.GetInteractPrompt();
		}
	}
}
