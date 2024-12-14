using Godot;
using System;

public partial class RaycastBullet : RayCast3D
{
    public Player player;
    private int pierceAmmount;
    private int damage;
    private PackedScene bloodParticle = ResourceLoader.Load<PackedScene>("res://Particles/test.tscn");

    public override void _Ready()
    {
        pierceAmmount = player.piercing;
        damage = player.damage;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsColliding())
        {
            Node3D collider = (Node3D)GetCollider();
            if (collider != null && collider.IsInGroup("enemy") && collider is Damageable damageable)
            {
                damageable.HandleHit(damage);

                GpuParticles3D blood = bloodParticle.Instantiate() as GpuParticles3D;
                AddChild(blood);
                blood.GlobalPosition = GetCollisionPoint();
                blood.Emitting = true;

                if (pierceAmmount == 0)
                {
                    SetCollisionMaskValue(2, false);
                }
                else
                {
                    AddException(collider as CollisionObject3D);
                    pierceAmmount--;
                }
            }

            
        }
    }

    public void _onTimeout()
    {
        QueueFree();
    }
}
