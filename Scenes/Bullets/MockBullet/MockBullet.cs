using Godot;
using System;

public partial class MockBullet : Node3D
{
	[Export] public float speed = 40f;
	[Export] RayCast3D bulletRayCast; 
	[Export] MeshInstance3D bulletMesh;
	[Export] GpuParticles3D bulletParticles;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		Vector3 speedVector = new Vector3(0, 0, -speed);
		Position += Transform.Basis * speedVector * (float)delta;
		if (bulletRayCast.IsColliding())
		{
			bulletMesh.Visible = false;
			bulletParticles.Emitting = true;
			GetTree().CreateTimer(1).Timeout += () => {
				QueueFree();
			};
			
		}
	}

	public void _onTimerTimeout()
	{
		QueueFree();
	}

}


