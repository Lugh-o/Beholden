using Godot;
using System;

public partial class Enemy : Damageable
{
    [Export] public float walkSpeed = 4.3f;
    [Export] public float attackRange = 5f;
    [Export] public NavigationAgent3D navigationAgent;
    [Export] public Node3D level;
    [Export] public double healingDropRate = 0.3;
    [Export] public double ammoDropRate = 0.3;

    public PackedScene healingDrop = ResourceLoader.Load<PackedScene>("res://Scenes/Drops/Healing Drop/HealingDrop.tscn");
    public PackedScene ammoDrop = ResourceLoader.Load<PackedScene>("res://Scenes/Drops/AmmoDrop/AmmoDrop.tscn");

    public bool TargetInRange()
    {
        return GlobalPosition.DistanceTo(player.GlobalPosition) < attackRange;
    }

    public void HandleNavigation()
    {
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
    }

    public void HandleFacing()
    {
        Vector3 playerPosition = new Vector3(player.GlobalPosition.X, GlobalPosition.Y, player.GlobalPosition.Z);
        LookAt(playerPosition, Vector3.Up);
    }

    public virtual void HandleAttack() { }

    public void HandleGravity(float deltaFloat)
    {
        if (!IsOnFloor()) Velocity += GetGravity() * deltaFloat;
    }

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
        } else if (healingDropRate < rng && rng < healingDropRate + ammoDropRate)
        {
           DropAmmo(); 
        }
        
        player.GainExperience(5);
        QueueFree();
    }

}
