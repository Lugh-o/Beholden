using Godot;
using System;

public partial class UpgradeBox : Control
{
	public Player player;
	[Export] private RichTextLabel upgradeNameLabel;
	[Export] private RichTextLabel upgradeStatLabel;

	public override void _Ready()
	{
		// Pegar titulo
		// Pegar Upgrade
	}

	public void _onChooseButtonPressed()
	{
		player.UpgradeStat();
	}
}
