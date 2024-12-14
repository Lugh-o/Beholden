using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UpgradeMenu : CanvasLayer
{
	[Export] HBoxContainer upgradeContainer;
	public PackedScene upgradeBox = ResourceLoader.Load<PackedScene>("res://Scenes/Menus/UpgradeMenu/UpgradeBox/UpgradeBox.tscn");
	[Export] public Player player;
	[Export] public Button rerollButton;
	[Export] public RichTextLabel rerollLabel;

	private Random random = new Random();

	public List<string> allUpgrades = new List<string>
	{
		"Speed Boost",
		"Double Jump",
		"Extra Health",
		"Faster Reload",
		"Magnetic Pull",
		"Piercing Bullets",
		"Shotgun Shells",
		"More Bullets"
	};

	private List<string> currentUpgrades = new List<string>();

	public void ShowUpgradeMenu()
	{
		currentUpgrades = allUpgrades.ToList();
		rerollLabel.Text = "[center][color=white][font_size=36]REROLL (1)";
		rerollButton.Disabled = false;
		generateBoxes();
		GetTree().Paused = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		Globals.HasMenu = true;
		Show();

	}

	public void HideUpgradeMenu()
	{
		GetTree().Paused = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Globals.HasMenu = false;
		Hide();
	}

	private void generateBoxes()
	{
		ClearBoxes();
		for (int i = 0; i < 3; i++)
		{
			UpgradeBox upgradeBoxInstance = upgradeBox.Instantiate<UpgradeBox>();

			var choosen = random.Next(currentUpgrades.Count);


			upgradeBoxInstance.upgradeName = currentUpgrades[choosen];
			upgradeContainer.AddChild(upgradeBoxInstance);
			upgradeBoxInstance.player = player;

			currentUpgrades.RemoveAt(choosen);
		}
	}

	private void ClearBoxes()
	{
		foreach (Control child in upgradeContainer.GetChildren())
		{
			upgradeContainer.RemoveChild(child);
		}
	}

	public void _onRerollButtonPressed()
	{
		rerollButton.Disabled = true;
		rerollLabel.Text = "[center][color=white][font_size=36]REROLL (0)";
		currentUpgrades = allUpgrades.ToList();
		generateBoxes();
	}
}
