extends Enemy
class_name Boss

@export var longRoarSfx: AudioStreamPlayer3D
@export var attackSfx: AudioStreamPlayer3D

const BOSS_LASER: PackedScene = preload("res://Scenes/Bullets/BossLaser/BossLaser.tscn")

func _ready() -> void:
	attackRange = 50
	maxHealth = 300
	currentHealth = maxHealth
	speed = 20
	attackDelay = 2
	navigationAgent.target_desired_distance = 0.5 + attackRange + collisionShape.shape.radius
	currentHealth = maxHealth
	attackTimer.wait_time = attackDelay
	
func HandleAttack() -> void:
	if (attackTimer.is_stopped()):
		attackTimer.start()
		animations.play("attacking")
		attackSfx.play()

		var bossLaserInstance: BossLaser = BOSS_LASER.instantiate()
		bossLaserInstance.position = global_position
		add_sibling(bossLaserInstance)

		# Calcular a direção pro jogador
		var directionToPlayer: Vector3 = (player.global_position + Vector3(randf_range(-1, 1), randf_range(-1, 1), randf_range(-1, 1)) - global_position).normalized()

		# Define a direção do laser
		bossLaserInstance.look_at_from_position(global_position, player.global_position, Vector3.UP)

		# Ajusta a velocidade e adiciona na cena
		bossLaserInstance.velocity = directionToPlayer * bossLaserInstance.speed
		

func Die() -> void:
	animations.play("die")

func _onAnimationFinished(animationName: String) -> void:
	if (animationName == "attacking"):
		animations.play("walking")
	elif (animationName == "die"):
		player.ShowCongratulationsMenu()
		queue_free()

func HandleHit(damageTaken: float):
	currentHealth -= damageTaken
	player.bossHp.value = currentHealth
	if (currentHealth <= 0):
		Die()