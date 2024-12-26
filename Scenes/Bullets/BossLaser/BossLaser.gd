extends CharacterBody3D
class_name BossLaser

@onready var speed: float = 30

@export var bulletRayCast: RayCast3D
@export var bulletMesh: MeshInstance3D
@export var bulletParticles: GPUParticles3D

func _physics_process(_delta) -> void:
	if (bulletRayCast.is_colliding()):
		var collider: Node3D = bulletRayCast.get_collider()
		if (collider != null):
			var parent: Node = collider.get_parent()
			if (parent is Player):
				parent.HandleHit(10)
		bulletRayCast.enabled = false
		bulletMesh.visible = false
		bulletParticles.emitting = true
	else:
		velocity = velocity.normalized() * speed
		move_and_slide()
	
func _onTimeoutTimerTimeout() -> void:
	queue_free()

func _onParticlesFinished() -> void:
	queue_free()
