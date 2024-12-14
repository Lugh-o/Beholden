using Godot;
using System;

public partial class Boss : Enemy
{
	[Export] private AnimationPlayer animations;
	[Export] private Timer attackTimer;
	[Export] private float attackDelay;

	[Export] public AudioStreamPlayer3D longRoarSfx;
	[Export] public AudioStreamPlayer3D attackSfx;

	public PackedScene bossLaser = ResourceLoader.Load<PackedScene>("res://Scenes/Bullets/BossLaser/BossLaser.tscn");

	public override void _Ready()
	{
		attackDelay = 2f;
		attackRange = 50f;
		MaxHealth = 300;
		CurrentHealth = MaxHealth;
		attackTimer.WaitTime = attackDelay;
		walkSpeed = 20f;
	}

	public override void _Process(double delta)
	{
		player.bossHP.Value = CurrentHealth;

	}

	public override void HandleAttack()
	{
		if (attackTimer.IsStopped())
		{
			attackTimer.Start();
			animations.Play("attacking");
			attackSfx.Play();

			BossLaser bossLaserInstance = bossLaser.Instantiate<BossLaser>();
			bossLaserInstance.Position = GlobalPosition;

			// Calcular a direção para o jogador
			Vector3 directionToPlayer = (player.GlobalPosition + new Vector3(GD.RandRange(-1, 1), GD.RandRange(-1, 1), GD.RandRange(-1, 1)) - GlobalPosition).Normalized();

			// Definir a direção do laser
			bossLaserInstance.LookAtFromPosition(GlobalPosition, player.GlobalPosition, Vector3.Up);

			// Ajustar a velocidade e adicionar à cena
			bossLaserInstance.Velocity = directionToPlayer * bossLaserInstance.speed;
			GetParent().AddChild(bossLaserInstance);
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
