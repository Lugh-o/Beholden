extends Enemy
class_name Drone

@onready var attackDelay: float = 1

func _ready():
	attackRange = 1.5
	damage = 1
	maxHealth = 3
	currentHealth = maxHealth
	attackTimer.wait_time = attackDelay
	speed = 10
