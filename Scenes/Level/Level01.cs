using Godot;
using System;

public partial class Level01 : Node3D
{
	[Export] public CharacterBody3D player;
	[Export] public Node3D spawns;
	[Export] public NavigationRegion3D navigationRegion;

	public PackedScene rangedMock = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/RangedMock/RangedMock.tscn");

	public PackedScene meleeMock = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/MeleeMock/MeleeMock.tscn");
	public PackedScene[] mockArray;
	

	public override void _Ready()
	{
		mockArray = new PackedScene[2] { meleeMock, rangedMock };
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}


	public static Node3D GetRandomChild(Node3D parentNode)
	{
		Random random = new Random();
		int randomId = random.Next(0, parentNode.GetChildCount());
		return (Node3D)parentNode.GetChild(randomId);
	}

	public void _OnMockSpawnTimerTimeout()
	{
		Vector3 spawnPoint = GetRandomChild(spawns).GlobalPosition;

		Random random = new Random();
		Enemy defaultInstance;
		switch (random.Next(0, 2))
		{
			case 0:
				defaultInstance = mockArray[0].Instantiate<MeleeMock>();
				break;
			case 1:
				defaultInstance = mockArray[1].Instantiate<RangedMock>();
				break;
			default:
				return;
		}

		navigationRegion.AddChild(defaultInstance);
		defaultInstance.GlobalPosition = spawnPoint;
		defaultInstance.player = player;

	}

}