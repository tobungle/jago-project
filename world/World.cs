using System.Collections.Generic;
using Godot;

public partial class World : Node3D
{
	[Export] PackedScene remote_player_packed;
	Node network;
	Dictionary<int, RemotePlayer> spawned_remote = new();
	public override void _Ready()
	{
		GD.Print("Setting up world...");
		network = GetNode<Node>("/root/Network");
		foreach (int player_id in (Godot.Collections.Array<int>) network.Call("get_player_ids"))
		{
			SpawnRemotePlayer(player_id);
		}
		network.Connect("player_joined", Callable.From((int player_id) => SpawnRemotePlayer(player_id)));
		network.Connect("player_left", Callable.From((int player_id) => RemoveRemotePlayer(player_id)));
	}

	void SpawnRemotePlayer(int id)
	{
		if (spawned_remote.Keys.Contains(id))
		{
			GD.Print($"Not spawning remote player {id} because they've already been spawned.");
			return;
		}
		GD.Print($"Spawning remote player {id}...");
		RemotePlayer inst = remote_player_packed.Instantiate<RemotePlayer>();
		inst.GlobalPosition = new Vector3(0f, 1f, 0f);
		inst.Name = id.ToString();
		AddChild(inst, true);
		spawned_remote[id] = inst;
	}

	void RemoveRemotePlayer(int id)
	{
		if (!spawned_remote.Keys.Contains(id))
		{
			GD.Print($"Not removing remote player {id} because they've already been removed.");
			return;
		}
		GetNode(id.ToString()).QueueFree();
		spawned_remote.Remove(id);
	}
}
