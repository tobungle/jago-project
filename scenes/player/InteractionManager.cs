using Godot;
using System;

public partial class InteractionManager : Node
{
	Globals globals;
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

	void OnGetItemPressed()
	{
		globals.hovered_interactable?.Interact();
	}
}
