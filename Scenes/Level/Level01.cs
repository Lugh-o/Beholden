using Godot;
using System;

public partial class Level01 : Node3D
{
	[Export] public Player player;
	[Export] public Node3D spawns;
	[Export] public NavigationRegion3D navigationRegion;

	public PackedScene drone = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/Drone/Drone.tscn");
	public PackedScene fat = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/Fat/Fat.tscn");
	public PackedScene[] mockArray;

	public override void _Ready()
	{
		mockArray = new PackedScene[2] { drone, fat };
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public static Node3D GetRandomChild(Node3D parentNode)
	{
		return (Node3D)parentNode.GetChild(GD.RandRange(0, parentNode.GetChildCount() - 1));
	}

	public void _OnMockSpawnTimerTimeout()
	{
		Vector3 spawnPoint = GetRandomChild(spawns).GlobalPosition;

		Enemy defaultInstance = GD.RandRange(0, 3) switch
		{
			0 => mockArray[0].Instantiate<Drone>(),
			_ => mockArray[1].Instantiate<Fat>(),
		};

		navigationRegion.AddChild(defaultInstance);
		defaultInstance.GlobalPosition = spawnPoint;
		defaultInstance.level = this;
		defaultInstance.player = player;
		defaultInstance.group = GD.RandRange(1, 5);
	}

	public void _onSurviveTimerTimeout()
	{
		player.ShowCongratulationsMenu();
	}

}
