extends CanvasLayer
class_name PauseMenu

@onready var game = get_tree().get_first_node_in_group("game")
@onready var generic_menu_sfx = game.get_node_or_null("GenericMenuSFX") as AudioStreamPlayer

func _ready():
	hide()

func _process(_delta):
	if Input.is_action_just_pressed("pause") and not Globals.hasMenu:
		if not get_tree().paused:
			get_tree().paused = true
			Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
			show()
		else:
			get_tree().paused = false
			Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
			hide()

func _on_resume_button_pressed():
	generic_menu_sfx.play()
	get_tree().paused = false
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	hide()

func _on_return_button_pressed():
	generic_menu_sfx.play()
	game.call("load_main_menu")
