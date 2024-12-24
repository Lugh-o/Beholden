extends CanvasLayer
class_name CongratulationsMenu

@onready var game = get_tree().get_first_node_in_group("game")
@onready var generic_menu_sfx = game.get_node_or_null("GenericMenuSFX") as AudioStreamPlayer

func _ready():
	hide()

func show_congratulations_menu():
	Globals.hasMenu = true
	Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	get_tree().paused = true
	show()

func _on_return_button_pressed():
	Globals.hasMenu = false
	generic_menu_sfx.play()
	game.call("load_main_menu")
