using Godot;

public partial class InteractArea : Area3D
{
	public Interactable interactable;
	Globals globals;
	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;

		// This bit of code will break if parent is not an interactable
		Node parent = GetParent<Node>();
		// Cleanup if parent is deleted
		parent.TreeExiting += () =>
		{
			if (globals.hovered_interactable == parent)
			{
				globals.hovered_interactable = null;
			}
		};
		interactable = parent as Interactable;
	}

	void OnMouseEntered()
	{
		globals.hovered_interactable = interactable;
	}

	void OnMouseExited()
	{
		if (globals.hovered_interactable == interactable)
		{
			globals.hovered_interactable = null;
		}
	}
}
