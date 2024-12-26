using Godot;
using System;
using System.Collections.Generic;

public partial class UpgradeBox : Control
{
	public Player player;
	[Export] private RichTextLabel upgradeNameLabel;
	[Export] private RichTextLabel upgradeStatLabel;

	public string upgradeName;

	public override void _Ready()
	{
		upgradeNameLabel.Text = $"[center][color=white][font_size=30]{upgradeName}";
		switch (upgradeName)
		{
			case "Speed Boost":
				upgradeStatLabel.Text = "[center][color=white][font_size=30]+25% Speed";
				break;
			case "Double Jump":
				upgradeStatLabel.Text = "[center][color=white][font_size=30]+1 Extra jump";
				break;
			case "Extra Health":
				upgradeStatLabel.Text = "[center][color=white][font_size=30]+50% HP";
				break;
			case "Faster Reload":
				upgradeStatLabel.Text = "[center][color=white][font_size=30]+25% reload speed";
				break;
			case "Magnetic Pull":
				upgradeStatLabel.Text = "[center][color=white][font_size=30]+100% drop pickup area";
				break;
			case "Piercing Bullets":
				upgradeStatLabel.Text = "[center][color=white][font_size=30]+1 Piercing";
				break;
			case "Shotgun Shells":
				upgradeStatLabel.Text = "[center][color=white][font_size=30]+1 Bullet per shot";
				break;
			case "More Bullets":
				upgradeStatLabel.Text = "[center][color=white][font_size=30]+3 Bullets in magazine";
				break;
			case "More Damage":
				upgradeStatLabel.Text = "[center][color=white][font_size=30]+1 Damage per bullet";
				break;
			case "Faster Fire Rate":
				upgradeStatLabel.Text = "[center][color=white][font_size=30]25% Faster fire rate";
				break;
		}
	}

	public void _onChooseButtonPressed()
	{
		player.UpgradeStat(upgradeName);
	}
}
