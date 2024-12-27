extends CanvasLayer
class_name CongratulationsMenu

@onready var game: Game = get_tree().get_first_node_in_group("game")
@onready var genericMenuSfx: AudioStreamPlayer = game.get_node_or_null("GenericMenuSFX")

func _ready():
	hide()

func ShowCongratulationsMenu():
	Globals.hasMenu = true
	Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	get_tree().paused = true
	show()

func _onReturnButtonPressed():
	Globals.hasMenu = false
	genericMenuSfx.play()
	game.LoadMainMenu()
