extends CharacterBody3D
class_name Drop

@onready var player: Player
@onready var _lifetimeTimer: Timer
@onready var wasPicked: bool = false

@export var sprite: Sprite3D

func _ready():
    _lifetimeTimer = Timer.new()
    _lifetimeTimer.wait_time = 30
    _lifetimeTimer.one_shot = true
    _lifetimeTimer.connect("timeout", Callable(self, "_OnTimerTimeout"))
    add_child(_lifetimeTimer)
    _lifetimeTimer.start()

func _OnTimerTimeout() -> void:
    queue_free()

func _physics_process(delta):
    if (!is_on_floor()):
        velocity += get_gravity() * delta
    move_and_slide()
    if (wasPicked):
        velocity += global_position.direction_to(player.global_position) * 5

func GoToPlayer() -> void:
    wasPicked = true

func _OnScreenEntered() -> void:
    sprite.show()

func _OnScreenExited() -> void:
    sprite.hide()