extends Enemy
class_name Drone

func _ready():
	damage = 1
	maxHealth = 3
	speed = 10
	attackDelay = 1
	set_physics_process(false)
	call_deferred("actorSetup")


