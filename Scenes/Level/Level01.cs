using Godot;
using System;

public partial class Level01 : Node3D
{
	[Export] public CharacterBody3D player;
	[Export] public Node3D spawns;
	[Export] public NavigationRegion3D navigationRegion;

	public PackedScene rangedMock = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/RangedMock/RangedMock.tscn");
	public RangedMock rangedMockInstance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public static Node3D GetRandomChild(Node3D parentNode)
	{
		Random random = new Random();
		int randomId = random.Next(0, parentNode.GetChildCount());
		return (Node3D)parentNode.GetChild(randomId);
	}

	public void _OnRangedMockSpawnTimerTimeout()
	{
		Vector3 spawnPoint = GetRandomChild(spawns).GlobalPosition;
		rangedMockInstance = rangedMock.Instantiate<RangedMock>();
		navigationRegion.AddChild(rangedMockInstance);

		rangedMockInstance.GlobalPosition = spawnPoint;
		rangedMockInstance.player = player;

	}

}