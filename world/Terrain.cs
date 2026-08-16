using Godot;
using System;

public partial class Terrain : Node3D
{
	[Export] float meshsize = 5000f;
	[Export] int mesh_subdivisions = 250;
	[Export] float noise_freq = 0.01f;
	[Export] float terrain_height = 4f;
	[Export] float terrain_height_exponent = 4f;


	FastNoiseLite noise;
	MeshInstance3D terrainmesh;
	public override void _Ready()
	{
		Generate(1);
	}

	void Generate(int seed)
	{
		noise = new FastNoiseLite();
		noise.Seed = seed;
		noise.Frequency = noise_freq;
		terrainmesh?.QueueFree();
		terrainmesh = new MeshInstance3D();
		AddChild(terrainmesh);

		PlaneMesh base_mesh = new PlaneMesh();
		base_mesh.Size = new Vector2(meshsize, meshsize);
		base_mesh.SubdivideWidth = mesh_subdivisions;
		base_mesh.SubdivideDepth = mesh_subdivisions;

		Godot.Collections.Array base_arrays = base_mesh.GetMeshArrays();
		Godot.Collections.Array vertex_arrays = (Godot.Collections.Array) base_arrays[(int) Mesh.ArrayType.Vertex];
		// Convert PackedVector3Array to fucking ... um ... C# array
		Vector3[] vertices_ = new Vector3[vertex_arrays.Count];
		for (int i = 0; i < vertex_arrays.Count; i ++)
		{
			vertices_[i] = (Vector3) vertex_arrays[i];
		}

		for (int i = 0; i < vertex_arrays.Count; i ++)
		{
			Vector3 vertex = vertices_[i];
			float sample = GetY(vertex.X, vertex.Z);
			vertex.Y = sample;
			vertices_[i] = vertex;
		}
		base_arrays[(int) Mesh.ArrayType.Vertex] = vertices_;

		// FINALLY set up the array mesh

		ArrayMesh arrmesh = new ArrayMesh();
		arrmesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, base_arrays);
		terrainmesh.Mesh = arrmesh;
		terrainmesh.MaterialOverride = GD.Load<StandardMaterial3D>("res://assets/temp_materials/terrain_material.tres");
		terrainmesh.CreateTrimeshCollision();

	}

	float GetY(float x, float z)
	{
		return Mathf.Pow(noise.GetNoise2D(x, z) * terrain_height, terrain_height_exponent);
	}

}
