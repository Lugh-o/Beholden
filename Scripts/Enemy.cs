using Godot;
using System;

public partial class Enemy : Damageable
{
    [Export] public CharacterBody3D player;
    [Export] public float walkSpeed = 4.3f;
    [Export] public float attackRange = 5f;
    [Export] public NavigationAgent3D navigationAgent;


    public bool TargetInRange()
    {
        return GlobalPosition.DistanceTo(player.GlobalPosition) < attackRange;
    }

    public void HandleNavigation()
    {
        if (TargetInRange())
        {
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

}
