extends CharacterBody3D
class_name Drop

@onready var player: Player
@onready var _lifetimeTimer: Timer
@onready var _facingFrameThreshold: int = 10
@onready var _currentFacingFrame: int = 0
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
    HandleFacing()
    if (!is_on_floor()):
        velocity += get_gravity() * delta
    move_and_slide()
    if (wasPicked):
        velocity += global_position.direction_to(player.global_position) * 5

func HandleFacing() -> void:
    _currentFacingFrame += 1
    if (_currentFacingFrame >= _facingFrameThreshold):
        var playerPosition: Vector3 = Vector3(player.global_position.x, global_position.y, player.global_position.z)
        look_at(playerPosition, Vector3.UP)

func GoToPlayer() -> void:
    wasPicked = true

func _OnScreenEntered() -> void:
    sprite.show()

func _OnScreenExited() -> void:
    sprite.hide()