extends Node
class_name Game

var currentScene: Node;
const MAIN_MENU: PackedScene = preload("res://Scenes/Menus/MainMenu/MainMenu.tscn")
const LEVEL_01: PackedScene = preload("res://Scenes/Level/Level01.tscn")
const SETTINGS_MENU: PackedScene = preload("res://Scenes/Menus/SettingsMenu/SettingsMenu.tscn")


func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_VISIBLE;
	LoadMainMenu();

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func LoadMainMenu() -> void:
	RemoveCurrentScene();
	var mainMenuInstance: Control = MAIN_MENU.instantiate();
	add_child(mainMenuInstance);
	currentScene = mainMenuInstance;

func LoadLevel1() -> void:
	RemoveCurrentScene();
	var level1Instance: Node3D = LEVEL_01.instantiate();
	add_child(level1Instance)
	currentScene = level1Instance;

func LoadSettings() -> void:
	RemoveCurrentScene();
	var settingsInstance: Control = SETTINGS_MENU.instantiate();
	add_child(settingsInstance);
	currentScene = settingsInstance;

func RemoveCurrentScene() -> void:	
	if (currentScene != null):
		remove_child(currentScene);
		currentScene = null;
