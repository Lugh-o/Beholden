using Godot;
using System;

public partial class CongratulationsMenu : CanvasLayer
{
	public Game game;
	private AudioStreamPlayer genericMenuSfx;

	public override void _Ready()
	{
		game = GetTree().GetFirstNodeInGroup("game") as Game;
		genericMenuSfx = game.GetNodeOrNull("GenericMenuSFX") as AudioStreamPlayer;
		Hide();
	}

	public void ShowCongratulationsMenu()
	{
		Globals.HasMenu = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GetTree().Paused = true;
		Show();
	}

	public void _onReturnButtonPressed()
	{
		Globals.HasMenu = false;
		genericMenuSfx.Play();
		game.LoadMainMenu();
	}
}
