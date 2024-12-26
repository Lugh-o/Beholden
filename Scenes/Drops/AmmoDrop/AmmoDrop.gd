extends Drop
class_name AmmoDrop

func _onAreaEntered(body: Node3D):
	if (body is Player):
		player.HandleAmmoRecover(20)
		queue_free()
