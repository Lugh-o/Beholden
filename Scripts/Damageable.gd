extends CharacterBody3D
class_name Damageable

@export var player: Player
@onready var maxHealth: float
@onready var currentHealth: float

func HandleHit(damage: float):
    currentHealth -= damage
    if (currentHealth <= 0):
        Die()

func Die():
    pass