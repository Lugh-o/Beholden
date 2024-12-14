using Godot;
using System;

public partial class Damageable : CharacterBody3D
{
    [Export] public Player player;
    [Export] public double MaxHealth;
    public double CurrentHealth { get; set; }

    public override void _Ready()
    {
    }

    public virtual void HandleHit(double damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die() { }
}
