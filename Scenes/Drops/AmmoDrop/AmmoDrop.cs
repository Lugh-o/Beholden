using Godot;
using System;

public partial class AmmoDrop : Drop
{
	public void _onAreaEntered(Node3D body)
	{
		if (body is Player)
		{
			player.HandleAmmoRecover(20);
			QueueFree();
		}
	}
}
