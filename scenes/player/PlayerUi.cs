using Godot;

public partial class PlayerUi : CanvasLayer
{
	[Export] Camera3D player_camera;	// Needed for unprojecting postiions
	[Export] Vector2 hover_prompt_offset;
	[Export] Label hover_prompt;
	Globals globals;
	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
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
}
