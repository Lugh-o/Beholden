using Godot;
using System;

public partial class HealingDrop : Drop
{
	public void _onAreaEntered(Node3D body)
	{
		if (body is Player)
		{
			player.HandleHealing(player.CurrentHealth * 0.1);
			QueueFree();
		}
	}
}
