using Godot;
using System;


public partial class RangedMock : Enemy
{
	// Bullets
	[Export] public RayCast3D weaponGunBarrel;
	public PackedScene MockBullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullets/MockBullet/MockBullet.tscn");
	public MockBullet MockBulletInstance;
	[Export] public Timer shotDelay;

	public override void _Ready()
	{
		attackRange = 10f;
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
		if (shotDelay.IsStopped())
		{
			shotDelay.Start();
			MockBulletInstance = MockBullet.Instantiate<MockBullet>();
			MockBulletInstance.Position = weaponGunBarrel.GlobalPosition;

			Transform3D currentTransform = MockBulletInstance.Transform;
			currentTransform.Basis = weaponGunBarrel.GlobalTransform.Basis;
			MockBulletInstance.Transform = currentTransform;
			MockBulletInstance.speed = 10f;
			GetParent().AddChild(MockBulletInstance);
		}
	}
}
