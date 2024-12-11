using Godot;
using System;

public partial class Drop : CharacterBody3D
{
	public Player player;

	public override void _PhysicsProcess(double delta)
	{
		HandleFacing();
		if (!IsOnFloor()) Velocity += GetGravity() * (float)delta;
		MoveAndSlide();
	}

	public void HandleFacing()
	{
		Vector3 playerPosition = new Vector3(player.GlobalPosition.X, GlobalPosition.Y, player.GlobalPosition.Z);
		LookAt(playerPosition, Vector3.Up);
	}

}
