using Godot;
using System;

public partial class MeleeMock : Enemy
{
	[Export] public RayCast3D meleeRaycast;


	public override void _Ready()
	{
		attackRange = 0.9f;
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
			if (collider != null && collider.IsInGroup("player"))
			{
				Node3D parent = collider.GetParent<Node3D>();
				if (parent is Damageable damageable)
				{
					damageable.HandleHit(1);
				}
			}
		}
	}
}
