extends Enemy
class_name Fat

@onready var attackDelay: float = 2

func _ready():
	attackRange = 1.5
	maxHealth = 10
	damage = 2
	currentHealth = maxHealth
	attackTimer.wait_time = attackDelay
