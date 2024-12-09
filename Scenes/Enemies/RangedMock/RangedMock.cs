using Godot;
using System;


public partial class RangedMock : CharacterBody3D
{
	[Export] public CharacterBody3D player;
	[Export] public NavigationAgent3D navigationAgent;
	[Export] public float walkSpeed = 4.3f;
	[Export] public float attackRange = 2.5f;

	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		float deltaFloat = (float)delta;
		Velocity = Vector3.Zero;

		// Gravity
		if (!IsOnFloor()) Velocity += GetGravity() * deltaFloat;

		//Navigation 
		navigationAgent.TargetPosition = player.GlobalTransform.Origin;
		Vector3 nextNavigationPosition = navigationAgent.GetNextPathPosition();
		Velocity = (nextNavigationPosition - GlobalTransform.Origin).Normalized() * walkSpeed;

		// Facing
		Vector3 playerPosition = new Vector3(player.GlobalPosition.X, player.GlobalPosition.Y, player.GlobalPosition.Z);
		LookAt(playerPosition, Vector3.Up);

		MoveAndSlide();
	}

	public bool TargetInRange()
	{
		return GlobalPosition.DistanceTo(player.GlobalPosition) < attackRange;
	}
}
