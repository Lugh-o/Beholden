using Godot;
using System;

public partial class Game : Node
{

	public Node currentScene;
	public PackedScene mainMenu = ResourceLoader.Load<PackedScene>("res://Scenes/Menus/MainMenu/MainMenu.tscn");
	public PackedScene level1 = ResourceLoader.Load<PackedScene>("res://Scenes/Level/Level01.tscn");
	public PackedScene settings = ResourceLoader.Load<PackedScene>("res://Scenes/Menus/SettingsMenu/SettingsMenu.tscn");

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		LoadMainMenu();
	}

	public void LoadMainMenu()
	{
		RemoveCurrentScene();
		Control mainMenuInstance = mainMenu.Instantiate<Control>();
		AddChild(mainMenuInstance);
		currentScene = mainMenuInstance;
	}

	public void LoadLevel1()
	{
		RemoveCurrentScene();
		Node3D level1Instance = level1.Instantiate<Node3D>();
		AddChild(level1Instance);
		currentScene = level1Instance;
	}

	public void LoadSettings()
	{
		RemoveCurrentScene();
		Control settingsInstance = settings.Instantiate<Control>();
		AddChild(settingsInstance);
		currentScene = settingsInstance;
	}

	public void RemoveCurrentScene()
	{
		if (currentScene != null)
		{
			RemoveChild(currentScene);
			currentScene = null;
		}
	}
}
