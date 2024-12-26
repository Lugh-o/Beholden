extends RayCast3D
class_name RaycastBullet

@onready var player: Player
@onready var pierceAmount: int = player.piercing
@onready var damage: int = player.damage
const BLOOD_PARTICLE = preload("res://Particles/test.tscn")

func _physics_process(_delta):
	if (is_colliding()):
		var collider: Node3D = get_collider()
		if (collider != null && collider.is_in_group("enemy") && collider is Damageable):
			var damageableCollider: Damageable = collider
			damageableCollider.HandleHit(damage)
			var bloodParticleInstance: GPUParticles3D = BLOOD_PARTICLE.instantiate()
			add_child(bloodParticleInstance)
			bloodParticleInstance.global_position = get_collision_point()
			bloodParticleInstance.emmiting = true
			if (pierceAmount == 0):
				set_collision_mask_value(2, false)
			else:
				add_exception(collider)
				pierceAmount -= 1

func _onTimeout() -> void:
	queue_free()
    