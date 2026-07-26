using Godot;

public partial class PlayerUi : CanvasLayer
{
	[Export] Camera3D player_camera;	// Needed for unprojecting postiions
	[Export] Vector2 hover_prompt_offset;
	[Export] Label hover_prompt;
	Globals globals;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (globals.hovered_interactable == null)
		{
			hover_prompt.Text = "";
		}
		else
		{
			hover_prompt.Position = player_camera.UnprojectPosition(globals.hovered_interactable.GlobalPosition) + hover_prompt_offset;
			hover_prompt.Text = globals.hovered_interactable.interact_prompt;
		}
	}
}
