using Godot;
using System;

public partial class RaycastBullet : RayCast3D
{
	public override void _PhysicsProcess(double delta)
	{
		if (IsColliding())
		{
			Node3D collider = (Node3D)GetCollider();
			if (collider != null && collider.IsInGroup("enemy") && collider is Damageable damageable)
			{
				damageable.HandleHit(1);
				SetCollisionMaskValue(2, false);
			}
		}
	}

	public void _onTimeout()
	{
		QueueFree();
	}

}
