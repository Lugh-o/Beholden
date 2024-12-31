extends Node3D
class_name Level01

@export var player: Player
@export var spawns: Node3D
@export var navigationRegion: NavigationRegion3D
@export var surviveTimer: Timer
@export var boss: Boss
@export var enemiesNode: Node

const DRONE: PackedScene = preload("res://Scenes/Enemies/Drone/Drone.tscn")
const FAT: PackedScene = preload("res://Scenes/Enemies/Fat/Fat.tscn")


func _ready() -> void:
	set_physics_process(false)
	call_deferred("actorSetup")

	boss.process_mode = Node.PROCESS_MODE_DISABLED
	player.bossHp.max_value = boss.maxHealth
	player.bossHp.value = boss.maxHealth

# func _physics_process(delta):
# 	for child in enemiesNode.get_children():
# 		child.HandleMovement(delta)

func actorSetup() -> void:
	await get_tree().physics_frame
	set_physics_process(true)

func _OnMockSpawnTimerTimeout() -> void:
	var rng: float = randf_range(0, 1)
	var enemyInstance: Enemy
	if (rng > 0.25):
		enemyInstance = FAT.instantiate()
	else:
		enemyInstance = DRONE.instantiate()

	enemiesNode.add_child(enemyInstance)
	enemyInstance.global_position = spawns.get_child(randi_range(0, spawns.get_child_count() - 1)).global_position
	enemyInstance.surviveTimer = surviveTimer
	enemyInstance.level = self
	enemyInstance.player = player
	enemyInstance.HandleScaling()

func _onSurviveTimerTimeout() -> void:
	boss.process_mode = Node.PROCESS_MODE_INHERIT
	boss.longRoarSfx.play()
	boss.show()
