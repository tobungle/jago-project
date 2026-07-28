using System.Collections.Generic;
using Godot;

public partial class World : Node3D
{
	[Export] Vector3 player_start_position;
	[Export] PackedScene remote_player_packed;
	[Export] PackedScene player_packed;
	Node network;

	Dictionary<int, Player> spawned_players = new();
	Dictionary<int, WorldItem> spawned_world_items = new();
	int server_entity_counter = 0;
	public override void _Ready()
	{
		GD.Print("Setting up world...");
		network = GetNode<Node>("/root/Network");

		network.Connect("player_joined", Callable.From((int player_id) => SpawnPlayer(player_id, player_start_position)));
		network.Connect("player_left", Callable.From((int player_id) => RemovePlayer(player_id)));
		network.Connect("on_item_spawned", Callable.From((Godot.Collections.Dictionary<string, Variant> properties, int id) => SpawnItemLocal(properties, id)));
		network.Connect("on_item_despawned", Callable.From((int id) => DespawnItemLocal(id)));

		if (Multiplayer.IsServer())
		// Server setup
		{
			network.Connect("spawned_list_requested", Callable.From((int from) => OnSpawnedItemsRequested(from)));

			SpawnItemServer(new()
			{
				{"global_position", new Vector3(0f, 1f, -7f)}
			});
			SpawnItemServer(new()
			{
				{"global_position", new Vector3(0f, 1f, -8f)}
			});
			SpawnItemServer(new()
			{
				{"global_position", new Vector3(1f, 1f, -7f)}
			});
			SpawnItemServer(new()
			{
				{"global_position", new Vector3(2f, 1f, -7f)}
			});
		}
		else
		// Client setup
		{
			network.Connect("on_got_spawned_items", Callable.From((Godot.Collections.Dictionary<int, Vector3> spawned_items_list) => OnGotSpawnedItemsList(spawned_items_list)));
			network.RpcId(1, "request_spawned_list");
		}

		// Spawn a player for the local player
		SpawnPlayer(Multiplayer.GetUniqueId(), player_start_position);

		// Spawn a player for each connected lobby member
		foreach (int player_id in (Godot.Collections.Array<int>) network.Call("get_player_ids"))
		{
			SpawnPlayer(player_id, player_start_position);
		}
	}

	void SpawnPlayer(int id, Vector3 pos)
	{
		if (spawned_players.Keys.Contains(id))
		{
			GD.Print($"Not spawning player {id} because they've already been spawned.");
			return;
		}
		GD.Print($"Spawning player {id}...");
		Player inst = player_packed.Instantiate<Player>();
		inst.GlobalPosition = pos;
		inst.Name = id.ToString();
		inst.id = id;
		AddChild(inst, true);
		spawned_players[id] = inst;
	}

	void RemovePlayer(int id)
	{
		if (!spawned_players.Keys.Contains(id))
		{
			GD.Print($"Not removing player {id} because they've already been removed.");
			return;
		}
		GetNode(id.ToString()).QueueFree();
		spawned_players.Remove(id);
	}

	void SpawnItemServer(Godot.Collections.Dictionary<string, Variant> properties)
	{
		server_entity_counter ++;
		SpawnItemLocal(properties, server_entity_counter);
		network.Rpc("item_spawned", properties, server_entity_counter);
	}

	void SpawnItemLocal(Godot.Collections.Dictionary<string, Variant> properties, int id)
	{
		WorldItem inst = GD.Load<PackedScene>("res://scenes/world_item/WorldItem.tscn").Instantiate<WorldItem>();
		inst.Name = id.ToString();
		AddChild(inst, true);
		foreach (string property in properties.Keys)
		{
			inst.Set(property, properties[property]);
		}
		GD.Print($"Spawned thing {id}");
		inst.server_id = id;
		spawned_world_items[id] = inst;
		inst.PickedUp += () =>
		{
			DespawnItemServer(inst.server_id);
		};
	}

	void DespawnItemServer(int id)
	{
		network.Rpc("item_despawned", id);
		DespawnItemLocal(id);
	}
	void DespawnItemLocal(int id)
	{
		spawned_world_items[id].QueueFree();
		spawned_world_items.Remove(id);
		GD.Print($"Despawned thing {id}");
	}

	void OnSpawnedItemsRequested(int from)
	{
		GD.Print($"Player {from} requested spawned item list...");
		Godot.Collections.Dictionary<int, Vector3> spawned_items_list = new();
		foreach (int id in spawned_world_items.Keys)
		{
			WorldItem item = spawned_world_items[id];
			spawned_items_list[id] = item.GlobalPosition;
		}
		network.RpcId(from, "get_spawned_items", spawned_items_list);
		GD.Print($"Sent over spawned item list {spawned_items_list}");
	}

	void OnGotSpawnedItemsList(Godot.Collections.Dictionary<int, Vector3> spawned_items_list)
	{
		// This will need to include more properties later but uhhh fuck it for now just fuck it.
		foreach (int id in spawned_items_list.Keys)
		{
			SpawnItemLocal(new()
			{
				{"global_position", spawned_items_list[id]}
			},
			id);
		}
		GD.Print($"Got spawned items list {spawned_items_list}");
	}
}
