using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	public Game game;
	private AudioStreamPlayer genericMenuSfx;

	public override void _Ready()
	{
		game = GetTree().GetFirstNodeInGroup("game") as Game;
		genericMenuSfx = game.GetNodeOrNull("GenericMenuSFX") as AudioStreamPlayer;
		Hide();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause") && !Globals.HasMenu)
		{
			if (GetTree().Paused == false)
			{
				GetTree().Paused = true;
				Input.MouseMode = Input.MouseModeEnum.Visible;
				Show();
			}
			else
			{
				GetTree().Paused = false;
				Input.MouseMode = Input.MouseModeEnum.Captured;
				Hide();
			}

		}
	}

	public void _onResumeButtonPressed()
	{
		genericMenuSfx.Play();
		GetTree().Paused = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Hide();
	}

	public void _onReturnButtonPressed()
	{
		genericMenuSfx.Play();
		game.LoadMainMenu();
	}
}
