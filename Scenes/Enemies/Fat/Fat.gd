extends Enemy
class_name Fat

func _ready():
	maxHealth = 10
	damage = 2
	attackDelay = 2
	set_physics_process(false)
	call_deferred("actorSetup")