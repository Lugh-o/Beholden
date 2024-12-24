extends Control
class_name SettingsMenu

@onready var game = get_tree().get_first_node_in_group("game")
@onready var generic_menu_sfx = game.get_node_or_null("GenericMenuSFX") as AudioStreamPlayer

var sfx_index = AudioServer.get_bus_index("SFX")
var background_index = AudioServer.get_bus_index("Background")

@onready var background_slider = $MarginContainer/HBoxContainer/CenterContainer/VBoxContainer/VBoxContainer/VBoxContainer2/BackgroundMusic/VBoxContainer/HSlider
@onready var sfx_slider = $MarginContainer/HBoxContainer/CenterContainer/VBoxContainer/VBoxContainer/VBoxContainer2/SFX/VBoxContainer/HSlider

func _ready():
	background_slider.value = AudioServer.get_bus_volume_db(background_index)
	sfx_slider.value = AudioServer.get_bus_volume_db(sfx_index)

func _on_background_slider_value_changed(value: float) -> void:
	generic_menu_sfx.play()
	AudioServer.set_bus_volume_db(background_index, value)

func _on_background_mute_toggled(toggled_on: bool) -> void:
	generic_menu_sfx.play()
	AudioServer.set_bus_mute(background_index, toggled_on)

func _on_sfx_value_changed(value: float) -> void:
	generic_menu_sfx.play()
	AudioServer.set_bus_volume_db(sfx_index, value)

func _on_sfx_mute_toggled(toggled_on: bool) -> void:
	AudioServer.set_bus_mute(sfx_index, toggled_on)

func _on_fullscreen_toggled(toggled_on: bool) -> void:
	generic_menu_sfx.play()
	if toggled_on:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
	else:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)

func _on_save_pressed() -> void:
	generic_menu_sfx.play()
	game.call("load_main_menu")
