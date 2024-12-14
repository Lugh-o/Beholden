using Godot;
using System;


public partial class MainMenu : Control
{

	private Game game;
	private AudioStreamPlayer genericMenuSfx;
	private AudioStreamPlayer startMenuSfx;

	public override void _Ready()
	{
		GetTree().Paused = false;
		game = GetTree().GetFirstNodeInGroup("game") as Game;
		genericMenuSfx = game.GetNodeOrNull("GenericMenuSFX") as AudioStreamPlayer;
		startMenuSfx = game.GetNodeOrNull("StartMenuSFX") as AudioStreamPlayer;
	}

	public void _onStartButtonPressed()
	{
		startMenuSfx.Play();
		game.LoadLevel1();
	}

	public void _onSettingsButtonPressed()
	{
		genericMenuSfx.Play();
		game.LoadSettings();
	}

	public void _onExitButtonPressed()
	{
		GetTree().Quit();
	}

}
