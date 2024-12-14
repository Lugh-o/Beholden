using Godot;
using System;

public partial class Enemy : Damageable
{
	[Export] public float walkSpeed = 4.3f;
	[Export] public float attackRange = 5f;
	[Export] public NavigationAgent3D navigationAgent;
	[Export] public Level01 level;
	[Export] public Sprite3D sprite;

	[Export] public double healingDropRate = 0.3;
	[Export] public double ammoDropRate = 0.3;

	public PackedScene healingDrop = ResourceLoader.Load<PackedScene>("res://Scenes/Drops/Healing Drop/HealingDrop.tscn");
	public PackedScene ammoDrop = ResourceLoader.Load<PackedScene>("res://Scenes/Drops/AmmoDrop/AmmoDrop.tscn");

	public int navigationFrameThreshold = 40;
	public int currentNavigationFrame = 0;
	public int facingFrameThreshold = 20;
	public int currentFacingFrame = 0;
	[Export] public int group;

	public override void _PhysicsProcess(double delta)
	{
		HandleFacing();
		HandleMovement((float)delta);
		MoveAndSlide();
	}

	public bool TargetInRange()
	{
		return GlobalPosition.DistanceTo(player.GlobalPosition) < attackRange;
	}

	public void HandleMovement(float deltaFloat)
	{
		currentNavigationFrame++;
		if (currentNavigationFrame >= navigationFrameThreshold + group)
		{
			if (!IsOnFloor()) Velocity += GetGravity() * deltaFloat;

			currentNavigationFrame = 0;
			if (TargetInRange())
			{
				Velocity = Vector3.Zero;
				HandleAttack();
			}
			else
			{
				navigationAgent.TargetPosition = player.GlobalTransform.Origin;
				Vector3 nextNavigationPosition = navigationAgent.GetNextPathPosition();
				Velocity = (nextNavigationPosition - GlobalTransform.Origin).Normalized() * walkSpeed;
			}
			Vector3 playerPosition = new(player.GlobalPosition.X, GlobalPosition.Y, player.GlobalPosition.Z);
			LookAt(playerPosition, Vector3.Up);

		}
	}

	public void HandleFacing()
	{
		currentFacingFrame++;
		if (currentFacingFrame >= facingFrameThreshold)
		{
			Vector3 playerPosition = new(player.GlobalPosition.X, GlobalPosition.Y, player.GlobalPosition.Z);
			LookAt(playerPosition, Vector3.Up);
		}
	}

	public virtual void HandleAttack() { }

	public void DropHealing()
	{
		HealingDrop healingDropInstance = healingDrop.Instantiate<HealingDrop>();
		level.AddChild(healingDropInstance);
		healingDropInstance.GlobalPosition = GlobalPosition;
		healingDropInstance.player = player;
	}

	public void DropAmmo()
	{
		AmmoDrop ammoDropInstance = ammoDrop.Instantiate<AmmoDrop>();
		level.AddChild(ammoDropInstance);
		ammoDropInstance.GlobalPosition = GlobalPosition;
		ammoDropInstance.player = player;
	}

	public override void Die()
	{
		float rng = GD.Randf();
		if (rng <= healingDropRate)
		{
			DropHealing();
		}
		else if (rng < healingDropRate + ammoDropRate)
		{
			DropAmmo();
		}

		player.GainExperience(5);
		QueueFree();
	}

	public void _onScreenEntered()
	{
		sprite.Show();
	}

	public void _onScreenExited()
	{
		sprite.Hide();
	}


}
