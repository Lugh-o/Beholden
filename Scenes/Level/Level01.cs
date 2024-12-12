using Godot;
using System;

public partial class Level01 : Node3D
{
	[Export] public Player player;
	[Export] public Node3D spawns;
	[Export] public NavigationRegion3D navigationRegion;

	public PackedScene rangedMock = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/RangedMock/RangedMock.tscn");
	public PackedScene meleeMock = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/MeleeMock/MeleeMock.tscn");
	public PackedScene[] mockArray;

	public int enemyCount = 0;
	
	public override void _Ready()
	{
		mockArray = new PackedScene[2] { meleeMock, rangedMock };
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public static Node3D GetRandomChild(Node3D parentNode)
	{
		return (Node3D)parentNode.GetChild(GD.RandRange(0, parentNode.GetChildCount() - 1));
	}

	public void _OnMockSpawnTimerTimeout()
	{
		Vector3 spawnPoint = GetRandomChild(spawns).GlobalPosition;

        Enemy defaultInstance = GD.RandRange(0, 1) switch
        {
          	0 => mockArray[1].Instantiate<RangedMock>(),
            _ => mockArray[0].Instantiate<MeleeMock>(),
        };

        navigationRegion.AddChild(defaultInstance);
		defaultInstance.GlobalPosition = spawnPoint;
		defaultInstance.level = this;
		defaultInstance.player = player;
		defaultInstance.group = GD.RandRange(1, 5);
		enemyCount++;
	}

	public void _onSurviveTimerTimeout()
	{
		GD.Print("Boss Spawnou");
	}

}