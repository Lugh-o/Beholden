using Godot;
using System;

public partial class MeleeMock : Enemy
{
	[Export] public RayCast3D meleeRaycast;

	public override void _Ready()
	{
		attackRange = 0.9f;
		CurrentHealth = MaxHealth;
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleGravity((float)delta);
		HandleFacing();
		HandleNavigation();
		MoveAndSlide();
	}

	public override void HandleAttack()
	{
		if (meleeRaycast.IsColliding())
		{
			Node3D collider = (Node3D)meleeRaycast.GetCollider();

			if (collider != null && collider.IsInGroup("player") && collider is Damageable damageable)
			{
				damageable.HandleHit(1);
			}
		}
	}

}
