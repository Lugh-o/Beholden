extends Damageable
class_name Enemy

@onready var speed: float = 4.3
@onready var attackRange: float = 0.5
@onready var damage: float = 1
@onready var healingDropRate: float = 0.2
@onready var ammoDropRate: float = 0.2
@onready var attackDelay: float

@export var navigationAgent: NavigationAgent3D
@export var level: Level01
@export var sprite: Sprite3D
@export var surviveTimer: Timer
@export var attackTimer: Timer
@export var animations: AnimationPlayer
@export var collisionShape: CollisionShape3D
@export var navTimer: Timer

const AMMO_DROP: PackedScene = preload("res://Scenes/Drops/AmmoDrop/AmmoDrop.tscn")
const HEALING_DROP: PackedScene = preload("res://Scenes/Drops/HealingDrop/HealingDrop.tscn")

func _physics_process(delta):
	HandleMovement(delta)
	move_and_slide()

func HandleMovement(delta: float) -> void:
	navigationAgent.target_position = player.global_transform.origin
	var nextNavigationPosition: Vector3 = navigationAgent.get_next_path_position()
	var direction: Vector3 = (nextNavigationPosition - global_transform.origin).normalized()
	var displacement: Vector3 = direction * speed
	if (!is_on_floor()):
		displacement += get_gravity() * delta
	velocity = displacement
	
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
	animations.play("die")

func _onScreenEntered() -> void:
	sprite.show()

func _onScreenExited() -> void:
	sprite.hide()

func HandleScaling() -> void:
	maxHealth *= 1 + (surviveTimer.time_left - surviveTimer.wait_time) * -0.01
	damage *= 1 + (surviveTimer.time_left - surviveTimer.wait_time) * -0.01

func HandleAttack() -> void:
	if (attackTimer.is_stopped()):
		attackTimer.start()
		animations.play("attacking")
		player.HandleHit(damage)

func _onAnimationFinished(animationName: String) -> void:
	if (animationName == "attacking"):
		animations.play("walking")
	elif (animationName == "die"):
		queue_free()

func _onTargetReached() -> void:
	HandleAttack()
