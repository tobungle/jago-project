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

	public override void _Process(double delta)
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
}
