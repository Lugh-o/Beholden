extends Control
class_name SettingsMenu

@onready var game: Game = get_tree().get_first_node_in_group("game")
@onready var genericMenuSfx: AudioStreamPlayer = game.get_node_or_null("GenericMenuSFX")

@onready var sfxIndex: int = AudioServer.get_bus_index("SFX")
@onready var backgroundIndex: int = AudioServer.get_bus_index("Background")

@onready var backgroundSlider: HSlider = $MarginContainer/HBoxContainer/CenterContainer/VBoxContainer/VBoxContainer/VBoxContainer2/BackgroundMusic/VBoxContainer/HSlider
@onready var sfxSlider: HSlider = $MarginContainer/HBoxContainer/CenterContainer/VBoxContainer/VBoxContainer/VBoxContainer2/SFX/VBoxContainer/HSlider

func _ready():
	backgroundSlider.value = AudioServer.get_bus_volume_db(backgroundIndex)
	sfxSlider.value = AudioServer.get_bus_volume_db(sfxIndex)

func _onBackgroundSliderValueChanged(value: float) -> void:
	genericMenuSfx.play()
	AudioServer.set_bus_volume_db(backgroundIndex, value)

func _onBackgroundMuteToggled(toggled_on: bool) -> void:
	genericMenuSfx.play()
	AudioServer.set_bus_mute(backgroundIndex, toggled_on)

func _onSfxValueChanged(value: float) -> void:
	genericMenuSfx.play()
	AudioServer.set_bus_volume_db(sfxIndex, value)

func _onSfxMuteToggled(toggled_on: bool) -> void:
	AudioServer.set_bus_mute(sfxIndex, toggled_on)

func _onFullscreenToggled(toggled_on: bool) -> void:
	genericMenuSfx.play()
	if toggled_on:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
	else:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)

func _onSavePressed() -> void:
	genericMenuSfx.play()
	game.LoadMainMenu()
