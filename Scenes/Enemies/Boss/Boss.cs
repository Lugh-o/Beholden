using Godot;
using System;

public partial class Boss : Enemy
{
	[Export] private AnimationPlayer animations;
	[Export] private Timer attackTimer;
	[Export] private float attackDelay;

	public override void _Ready()
	{
		attackDelay = 2f;
		attackRange = 20f;
		CurrentHealth = 50;
		attackTimer.WaitTime = attackDelay;
		walkSpeed = 2f;
	}

	public override void HandleAttack()
	{
		if (attackTimer.IsStopped())
		{
			attackTimer.Start();
			animations.Play("attacking");
			//ataque
		}
	}

	public override void Die()
	{
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
			player.ShowCongratulationsMenu();
			QueueFree();
		}
	}
}
