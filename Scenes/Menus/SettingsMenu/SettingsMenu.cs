using Godot;
using System;

public partial class SettingsMenu : Control
{

	public Game game;
	public int sfxIndex = AudioServer.GetBusIndex("SFX");
	public int backgroundIndex = AudioServer.GetBusIndex("Background");

    private HSlider backgroundSlider;
    private HSlider sfxSlider;

	public override void _Ready()
	{
		game = GetTree().GetFirstNodeInGroup("game") as Game;
		backgroundSlider = GetNode<HSlider>("MarginContainer/HBoxContainer/VBoxContainer/VBoxContainer/VBoxContainer2/BackgroundMusic/VBoxContainer/HSlider");
		backgroundSlider.Value = AudioServer.GetBusVolumeDb(backgroundIndex);

		sfxSlider = GetNode<HSlider>("MarginContainer/HBoxContainer/VBoxContainer/VBoxContainer/VBoxContainer2/SFX/VBoxContainer/HSlider");
		sfxSlider.Value = AudioServer.GetBusVolumeDb(sfxIndex);

	}

	public void _onBackgroundSliderValueChanged(float value)
	{
		AudioServer.SetBusVolumeDb(backgroundIndex, value);
	}

	public void _onBackgroundMuteToggled(bool toggledOn)
	{
		AudioServer.SetBusMute(backgroundIndex, toggledOn);
	}

	public void _onSfxValueChanged(float value)
	{
		AudioServer.SetBusVolumeDb(sfxIndex, value);
	}

	public void _onSfxMuteToggled(bool toggledOn)
	{
		AudioServer.SetBusMute(sfxIndex, toggledOn);
	}

	public void _onFullscreenToggled(bool toggledOn)
	{
		if (toggledOn)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}

	}

	public void _onSavePressed() 
	{
		game.LoadMainMenu();
	}
}
