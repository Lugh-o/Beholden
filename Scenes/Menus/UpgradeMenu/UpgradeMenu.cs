using Godot;
using System;

public partial class UpgradeMenu : CanvasLayer
{
	[Export] HBoxContainer upgradeContainer;
	public PackedScene upgradeBox = ResourceLoader.Load<PackedScene>("res://Scenes/Menus/UpgradeMenu/UpgradeBox/UpgradeBox.tscn");
	[Export] public Player player;
	[Export] public Button rerollButton;

	public void ShowUpgradeMenu()
	{
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
			upgradeContainer.AddChild(upgradeBoxInstance);
			upgradeBoxInstance.player = player;
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
		generateBoxes();
	}
}
