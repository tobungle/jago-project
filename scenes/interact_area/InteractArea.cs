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
	string interact_prompt;
	public override void _Ready()
	{
		SetInteractPrompt();
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
