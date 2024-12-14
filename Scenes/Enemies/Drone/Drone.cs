using Godot;
using System;

public partial class Drone : Enemy
{
	[Export] private AnimationPlayer animations;
	[Export] private Timer attackTimer;
	[Export] private float attackDelay;

	public override void _Ready()
	{
		attackDelay = 1f;
		attackRange = 1.5f;
		damage = 1;
		MaxHealth = 3;
		CurrentHealth = MaxHealth;
		attackTimer.WaitTime = attackDelay;
		walkSpeed = 10f;
	}

	public override void HandleAttack()
	{
		if (attackTimer.IsStopped())
		{
			attackTimer.Start();
			animations.Play("attacking");
			player.HandleHit(damage);
		}
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
		animations.Play("die");
	}

	public void _onAnimationFinished(string name)
	{
		if (name == "attacking")
		{
			animations.Play("walking");
		}
		else if (name == "die")
		{
			QueueFree();
		}
	}
}
