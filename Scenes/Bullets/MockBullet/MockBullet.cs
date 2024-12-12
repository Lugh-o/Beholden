using Godot;
using System;

public partial class MockBullet : Node3D
{
	[Export] public float speed = 30f;
	[Export] RayCast3D bulletRayCast;
	[Export] MeshInstance3D bulletMesh;
	[Export] GpuParticles3D bulletParticles;
	[Export] public MeshInstance3D mesh;

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
				if (collider is Player player)
				{
					player.HandleHit(1);
				}
			}
			bulletRayCast.Enabled = false;
			bulletMesh.Visible = false;
			bulletParticles.Emitting = true;
		}
		else
		{
			Vector3 speedVector = new(0, 0, -speed);
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



	public void _onScreenEntered()
	{
		mesh.Show();
	}

	public void _onScreenExited()
	{
		mesh.Hide();
	}

}


