extends Control
class_name MainMenu

@onready var game: Game = get_tree().get_first_node_in_group("game")
@onready var genericMenuSfx: AudioStreamPlayer = game.get_node_or_null("GenericMenuSFX")
@onready var startMenuSfx: AudioStreamPlayer = game.get_node_or_null("StartMenuSFX")

func _ready():
	get_tree().paused = false

func _onStartButtonPressed():
	startMenuSfx.play()
	game.call("LoadLevel1")

func _onSettingsButtonPressed():
	genericMenuSfx.play()
	game.call("LoadSettings")

func _onExitButtonPressed():
	get_tree().quit()
