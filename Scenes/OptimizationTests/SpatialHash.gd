class_name SpatialHash
extends Object

var cellSize: float
var hashMap: Dictionary = {}

func _init(_cellSize: float) -> void:
	self.cellSize = _cellSize

func clear() -> void:
	hashMap.clear()

func insert(position: Vector3, obj) -> void:
	var cell: Vector3 = getCell(position)
	if not hashMap.has(cell):
		hashMap[cell] = []
	hashMap[cell].append(obj)

func query(position: Vector3, radius: float) -> Array:
	var cell: Vector3 = getCell(position)
	var nearbyCells: Array = getNearbyCells(cell)
	var result: Array = []

	for nearbyCell in nearbyCells:
		if hashMap.has(nearbyCell):
			for obj in hashMap[nearbyCell]:
				if obj["transform"].origin.distance_to(position) <= radius:
					result.append(obj)
	return result

func getCell(position: Vector3) -> Vector3:
	return Vector3(
		floor(position.x / cellSize),
		floor(position.y / cellSize),
		floor(position.z / cellSize)
	)

func getNearbyCells(cell: Vector3) -> Array:
	var cells: Array = []
	for x in range(-1, 1):
		# for y in range(-1, 2):
		for z in range(-1, 1):
			cells.append(cell + Vector3(x, 0, z))
	return cells
