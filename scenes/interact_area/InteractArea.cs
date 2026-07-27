using Godot;

public partial class InteractArea : Area3D
{
	public Interactable interactable;
	public override void _Ready()
	{
		// This bit of code will break if parent is not an interactable
		Node parent = GetParent<Node>();
		interactable = parent as Interactable;
	}
}
