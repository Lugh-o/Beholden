using Godot;
using System;

public partial class MeleeMock : Enemy
{
	[Export] public RayCast3D meleeRaycast;

	public override void _Ready()
	{
		attackRange = 0.9f;
		CurrentHealth = 3;
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleFacing();
		HandleMovement((float)delta);
		MoveAndSlide();
	}

	public override void HandleAttack()
	{
		if (meleeRaycast.IsColliding())
		{
			Node3D collider = (Node3D)meleeRaycast.GetCollider();

			if (collider != null && collider is Player player)
			{
				player.HandleHit(1);
			}
		}
	}

}
