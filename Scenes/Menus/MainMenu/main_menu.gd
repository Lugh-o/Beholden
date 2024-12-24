extends Control
class_name MainMenu

@onready var game = get_tree().get_first_node_in_group("game")
@onready var generic_menu_sfx = game.get_node_or_null("GenericMenuSFX") as AudioStreamPlayer
@onready var start_menu_sfx = game.get_node_or_null("StartMenuSFX") as AudioStreamPlayer

func _ready():
	get_tree().paused = false

func _on_start_button_pressed():
	start_menu_sfx.play()
	game.call("load_level1")

func _on_settings_button_pressed():
	generic_menu_sfx.play()
	game.call("load_settings")

func _on_exit_button_pressed():
	get_tree().quit()
