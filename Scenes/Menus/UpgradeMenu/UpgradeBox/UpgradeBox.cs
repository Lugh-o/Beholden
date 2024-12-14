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
        upgradeNameLabel.Text = upgradeName;
        switch (upgradeName)
		{
			case "Speed Boost":
				upgradeStatLabel.Text = "+25% Speed";
				break;
            case "Double Jump":
                upgradeStatLabel.Text = "+1 Extra jump";
                break;
            case "Extra Health":
                upgradeStatLabel.Text = "+50% HP";
                break;
            case "Faster Reload":
                upgradeStatLabel.Text = "+25% reload speed";
                break;
            case "Magnetic Pull":
                upgradeStatLabel.Text = "100% drop pickup area";
                break;
            case "Piercing Bullets":
                upgradeStatLabel.Text = "+1 Piercing";
                break;
            case "Shotgun Shells":
                upgradeStatLabel.Text = "+1 Bullet per shot";
                break;
            case "More Bullets":
                upgradeStatLabel.Text = "+3 Bullets in magazine";
                break;
            case "More Damage":
                upgradeStatLabel.Text = "+1 Damage per bullet";
                break;
            case "Faster Fire Rate":
                upgradeStatLabel.Text = "25% Faster fire rate";
                break;
        }
	}

	public void _onChooseButtonPressed()
	{
		player.UpgradeStat(upgradeName);
	}
}
