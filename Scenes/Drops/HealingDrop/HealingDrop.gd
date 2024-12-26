extends Drop
class_name HealingDrop

func _onAreaEntered(body: Node3D):
	if (body is Player):
		player.HandleHealing(player.currentHealth * 0.1)
		queue_free()
