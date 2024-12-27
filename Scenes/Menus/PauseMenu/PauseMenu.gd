extends CanvasLayer
class_name PauseMenu

@onready var game: Game = get_tree().get_first_node_in_group("game")
@onready var genericMenuSfx: AudioStreamPlayer = game.get_node_or_null("GenericMenuSFX")

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

func _onResumeButtonPressed():
	genericMenuSfx.play()
	get_tree().paused = false
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	hide()

func _onReturnButtonPressed():
	genericMenuSfx.play()
	game.LoadMainMenu()
