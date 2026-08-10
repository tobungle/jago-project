using Godot;

public partial class PlayerUi : CanvasLayer
{
	[Export] Inventory player_inventory;
	[Export] Control inv_control;	// Parent of all inventory ui, used for visibility toggling
	[Export] VBoxContainer inv_container;	// Node that inventory listing are instanced under
	[Export] Camera3D player_camera;	// Needed for unprojecting postiions
	[Export] Vector2 hover_prompt_offset;
	[Export] Label hover_prompt;
	Globals globals;
	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
		inv_control.Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		// Delay all ui signals by 1 frame just incase items are added before this scene is ready
		// This can be removed l8r when um. When items are not initilialised in _ready anymore. im gay
		player_inventory.ItemAdded += OnInventoryItemAdded;
		player_inventory.ItemQuantityChanged += OnInventoryItemQuantityChanged;
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
		inv_control.Visible = !inv_control.Visible;
		if (inv_control.Visible)
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

	void OnInventoryItemAdded(int item_index)
	{
		Item item = player_inventory.items[item_index];
		ItemListing listing = GD.Load<PackedScene>("uid://bwlsjc6ii5si5").Instantiate<ItemListing>();
		inv_container.AddChild(listing);
		listing.SetItem(item);
		GD.Print($"PlayerUi.cs: ItemAdded {player_inventory.items[item_index].type} (x{item.quantity})");
	}
	void OnInventoryItemQuantityChanged(int index, int quantity)
	{
		ItemListing listing = inv_container.GetChild<ItemListing>(index);
		listing.SetItem(player_inventory.items[index]);
		GD.Print($"PlayerUi.cs: ItemQuantityChanged {player_inventory.items[index].type} -> {quantity}");
	}
}
