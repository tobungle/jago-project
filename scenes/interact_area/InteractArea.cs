using Godot;

public partial class InteractArea : Area3D
{
	enum InteractType
	{
		Normal,
		ItemPickup
	}
	[Export] InteractType interact_type;
	[Export] string default_prompt;
	public string interact_prompt;
	Globals globals;
	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		SetInteractPrompt();
	}

	void OnMouseEntered()
	{
		globals.hovered_interactable = this;
	}

	void OnMouseExited()
	{
		if (globals.hovered_interactable == this)
		{
			globals.hovered_interactable = null;
		}
	}

	void SetInteractPrompt()
	{
		switch (interact_type)
		{
			case InteractType.ItemPickup:
			interact_prompt = $"Pickup item [LMB]";
			break;

			default:
			if (default_prompt == "")
			{
				interact_prompt = $"Interact with {Name} [LMB]";
			}
			break;
		}
	}
}
