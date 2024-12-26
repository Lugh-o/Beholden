extends Enemy
class_name Boss

@onready var attackDelay: float = 2

@export var animations: AnimationPlayer
@export var attackTimer: Timer
@export var longRoarSfx: AudioStreamPlayer3D
@export var attackSfx: AudioStreamPlayer3D

const BOSS_LASER: PackedScene = preload("res://Scenes/Bullets/BossLaser/BossLaser.tscn")

func _ready() -> void:
	attackRange = 50
	maxHealth = 300
	currentHealth = maxHealth
	attackTimer.wait_time = attackDelay
	speed = 20

func _process(_delta) -> void:
	# jogar isso no callback de take damage
	player.bossHp.value = currentHealth

func HandleAttack() -> void:
	if (attackTimer.is_stopped()):
		attackTimer.start()
		animations.play("attacking")
		attackSfx.play()

		var bossLaserInstance: BossLaser = BOSS_LASER.instantiate()
		bossLaserInstance.position = global_position

		# Calcular a direção pro jogador
		var directionToPlayer: Vector3 = (player.global_position + Vector3(randf_range(-1, 1), randf_range(-1, 1), randf_range(-1, 1)) - global_position).normalized()

		# Define a direção do laser
		bossLaserInstance.look_at_from_position(global_position, player.global_position, Vector3.UP)

		# Ajusta a velocidade e adiciona na cena
		bossLaserInstance.velocity = directionToPlayer * bossLaserInstance.speed
		add_sibling(bossLaserInstance)

func Die() -> void:
	animations.play("die")

func _onAnimationFinished(animationName: String) -> void:
	if (animationName == "attacking"):
		animations.play("walking")
	elif (animationName == "die"):
		player.ShowCongratulationsMenu()
		queue_free()
