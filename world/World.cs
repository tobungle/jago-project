using Godot;

public partial class World : Node3D
{
	[Export] PackedScene remote_player_packed;
	Node network;
	public override void _Ready()
	{
		GD.Print("Setting up world...");
		network = GetNode<Node>("/root/Network");
		foreach (int player_id in (Godot.Collections.Array<int>) network.Call("get_player_ids"))
		{
			GD.Print($"Spawning remote player {player_id}...");
			SpawnRemotePlayer(player_id);
		}
	}

	void SpawnRemotePlayer(int id)
	{
		Node3D inst = remote_player_packed.Instantiate<Node3D>();
		inst.GlobalPosition = new Vector3(0f, 1f, 0f);
		inst.Name = id.ToString();
		AddChild(inst, true);
	}
}
