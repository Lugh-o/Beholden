using Godot;
using System;

public partial class MockBullet : Node3D
{
	[Export] public float speed = 30f;
	[Export] RayCast3D bulletRayCast;
	[Export] MeshInstance3D bulletMesh;
	[Export] GpuParticles3D bulletParticles;

	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (bulletRayCast.IsColliding())
		{
			Node3D collider = (Node3D)bulletRayCast.GetCollider();
			if (collider != null)
			{
				Node3D parent = collider.GetParent<Node3D>();
				if (parent is Damageable damageable)
				{
					damageable.HandleHit(1);
				}
			}
			bulletMesh.Visible = false;
			bulletParticles.Emitting = true;
		}
		else
		{
			Vector3 speedVector = new Vector3(0, 0, -speed);
			Position += Transform.Basis * speedVector * (float)delta;
		}
	}

	public void _onTimeoutTimerTimeout()
	{
		QueueFree();
	}

	public void _onParticlesFinished()
	{
		QueueFree();
	}
}


