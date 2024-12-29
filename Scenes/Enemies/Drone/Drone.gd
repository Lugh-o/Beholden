extends Enemy
class_name Drone

func _ready():
	damage = 1
	maxHealth = 3
	speed = 10
	attackDelay = 1
	navigationAgent.target_desired_distance = 0.5 + attackRange + collisionShape.shape.radius
