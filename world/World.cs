using Godot;

public partial class World : Node3D
{
	Node network;
	public override void _Ready()
	{
		network = GetNode<Node>("/root/Network");
		
	}

	public override void _Process(double delta)
	{
	}
}
