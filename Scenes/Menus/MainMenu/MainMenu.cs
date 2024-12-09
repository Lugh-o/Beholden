using Godot;
using System;


public partial class MainMenu : Control
{

	public Game game;

	public override void _Ready()
	{
		GetTree().Paused = false;
		game = GetTree().GetFirstNodeInGroup("game") as Game;
	}

	public void _onStartButtonPressed()
	{
		game.LoadLevel1();
	}

	public void _onSettingsButtonPressed()
	{
		game.LoadSettings();
	}
	
	public void _onExitButtonPressed()
	{
		GetTree().Quit();
	}

}
