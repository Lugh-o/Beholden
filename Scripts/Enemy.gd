extends Damageable
class_name Enemy

@onready var speed: float = 4.3
@onready var attackRange: float = 5
@onready var damage: float = 1
@onready var healingDropRate: float = 0.2
@onready var ammoDropRate: float = 0.2

@onready var navigationFrameThreshold: int = 40
@onready var currentNavigationFrame: int = 0
@onready var facingFrameThreshold: int = 20
@onready var currentFacingFrame: int = 0

@export var group: int
@export var navigationAgent: NavigationAgent3D
@export var level: Level01
@export var sprite: Sprite3D
@export var surviveTimer: Timer

const AMMO_DROP = preload("res://Scenes/Drops/AmmoDrop/AmmoDrop.tscn")
const HEALING_DROP = preload("res://Scenes/Drops/HealingDrop/HealingDrop.tscn")

func _physics_process(delta):
	HandleFacing()
	HandleMovement(delta)
	move_and_slide()

func HandleFacing() -> void:
	currentFacingFrame += 1
	if (currentFacingFrame >= facingFrameThreshold):
		var playerPosition: Vector3 = Vector3(player.global_position.x, global_position.y, player.global_position.z)
		look_at(playerPosition, Vector3.UP)

func HandleMovement(delta: float) -> void:
	currentNavigationFrame += 1
	if (currentNavigationFrame >= navigationFrameThreshold + group):
		if (!is_on_floor()):
			velocity += get_gravity() * delta
		currentNavigationFrame = 0
		if (global_position.distance_to(player.global_position) < attackRange):
			velocity = Vector3(randf_range(-2, 2), 0, 0)
			HandleAttack()
		else:
			navigationAgent.target_position = player.global_transform.origin
			var nextNavigationPosition: Vector3 = navigationAgent.get_next_path_position() + Vector3(randf_range(-1, 1), 0, 0)
			velocity = (nextNavigationPosition - global_transform.origin).normalized() * speed

func HandleAttack():
	pass

func DropHealing() -> void:
	var healingDropInstance: HealingDrop = HEALING_DROP.instantiate()
	level.add_child(healingDropInstance)
	healingDropInstance.global_position = global_position
	healingDropInstance.player = player

func DropAmmo() -> void:
	var ammoDropInstance: AmmoDrop = AMMO_DROP.instantiate()
	level.add_child(ammoDropInstance)
	ammoDropInstance.global_position = global_position
	ammoDropInstance.player = player

func Die() -> void:
	var rng: float = randf()
	if (rng <= healingDropRate):
		DropHealing()
	elif (rng < healingDropRate + ammoDropRate):
		DropAmmo()
	player.GainExperience(5)
	queue_free()

func _onScreenEntered() -> void:
	sprite.show()

func _onScreenExited() -> void:
	sprite.hide()

func HandleScaling() -> void:
	maxHealth *= 1 + (surviveTimer.time_left - surviveTimer.wait_time) * -0.01
	damage *= 1 + (surviveTimer.time_left - surviveTimer.wait_time) * -0.01
