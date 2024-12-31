extends Enemy
class_name Fat

func _ready():
	maxHealth = 10
	damage = 2
	attackDelay = 2
	navigationAgent.target_desired_distance = 0.5 + attackRange + collisionShape.shape.radius
	currentHealth = maxHealth
	attackTimer.wait_time = attackDelay