using Godot;
using System;

public partial class BossLaser : CharacterBody3D
{
	[Export] public float speed = 40f;
	[Export] RayCast3D bulletRayCast;
	[Export] MeshInstance3D bulletMesh;
	[Export] GpuParticles3D bulletParticles;

	public override void _PhysicsProcess(double delta)
	{
		if (bulletRayCast.IsColliding())
		{
			Node3D collider = (Node3D)bulletRayCast.GetCollider();
			if (collider != null)
			{
				Node parent = collider.GetParent();
				if (parent is Player player)
				{
					player.HandleHit(10);
				}
			}
			bulletRayCast.Enabled = false;
			bulletMesh.Visible = false;
			bulletParticles.Emitting = true;
		}
		else
		{
			Velocity = Velocity.Normalized() * speed;
			MoveAndSlide();
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
