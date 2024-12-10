using Godot;
using System;

public partial class Damageable : CharacterBody3D
{
    [Export] public Player player;
    [Export] public int MaxHealth;
    public int CurrentHealth { get; set; }

    public override void _Ready()
    {
    }

    public virtual void HandleHit(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die() { }
}
