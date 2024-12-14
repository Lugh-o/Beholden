using Godot;
using System;

public partial class RaycastBullet : RayCast3D
{
	public Player player;
	private int pierceAmmount;

    public override void _Ready()
    {
		pierceAmmount = player.piercing;
    }
    public override void _PhysicsProcess(double delta)
	{
		if (IsColliding())
		{
			Node3D collider = (Node3D)GetCollider();
			if (collider != null && collider.IsInGroup("enemy") && collider is Damageable damageable)
			{
				damageable.HandleHit(1);
				if(pierceAmmount == 0) SetCollisionMaskValue(2, false);
				else
				{
					AddException(collider as CollisionObject3D);
					pierceAmmount--;
                }
			}
		}
	}

	public void _onTimeout()
	{
		QueueFree();
	}

}
