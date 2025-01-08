extends Node3D

@onready var objectShape: CapsuleShape3D = CapsuleShape3D.new()
@onready var capsuleMesh: CapsuleMesh = CapsuleMesh.new()

@onready var objectArray: Array = []
const objectDiameter: float = 1
const objectHeight: float = 2
const speed: float = 10
const gravity: float = 9.8

@onready var rng = RandomNumberGenerator.new()
const targetPosition: Vector3 = Vector3(0, 1, 0)

@onready var spawnFrameCounter: int = 0
const spawnFrameThreshold: int = 6

@export var label: Label
@onready var objectCount: int = 0

@onready var spatialHash: SpatialHash = SpatialHash.new(0.7)

@export var navigationRegion: NavigationRegion3D
@onready var navigationMap: RID = navigationRegion.get_navigation_map()
const targetDistance: int = 1

func _ready() -> void:
	rng.set_seed(123456789)
	self.call_deferred("setup")

func setup() -> void:
	await get_tree().physics_frame

func _process(_delta) -> void:
	label.text = "FPS: %d\nObject Count: %d" % [floor(Engine.get_frames_per_second()), objectCount]

func _physics_process(delta):
	spatialHash.clear()
	for object in objectArray:
		spatialHash.insert(object["transform"].origin, object)

	for object in objectArray:
		var baseTransform: Transform3D = object["transform"]
		var displacement: Vector3 = Vector3.ZERO

		if baseTransform.origin.y - (objectHeight / 2) > 0:
			# Falling
			displacement = Vector3(0, -gravity * delta, 0)
		else:
			var path: Array = getNavigationPath(baseTransform.origin)
			var distance: float = (path[-1] - path[0]).length()
			if distance >= targetDistance:
				var nextPoint: Vector3 = path[1] # Index 1 is the first step
				displacement = (nextPoint - baseTransform.origin).normalized() * speed * delta
			else:
				killObject(object)
				continue

		var nearbyObjects: Array = spatialHash.query(baseTransform.origin, objectDiameter)
		for nearby in nearbyObjects:
			if object == nearby:
				continue

			var collisionVector: Vector3 = baseTransform.origin - nearby["transform"].origin
			collisionVector.y = 0
			var distance: float = collisionVector.length()

			if distance < objectDiameter:
				# Colliding
				displacement = collisionVector.normalized() * (objectDiameter - distance)

		baseTransform.origin += displacement
		object["transform"] = baseTransform

		PhysicsServer3D.body_set_state(object["physicsBody"], PhysicsServer3D.BODY_STATE_TRANSFORM, baseTransform)
		RenderingServer.instance_set_transform(object["mesh"], baseTransform)

	spawnFrameCounter += 1
	if spawnFrameCounter >= spawnFrameThreshold:
		spawnObject()
		spawnFrameCounter = 0

func getNavigationPath(agentPosition: Vector3) -> Array:
	var path: Array = NavigationServer3D.map_get_path(
				navigationMap,
				agentPosition,
				targetPosition,
				true
			)
	return path

func spawnObject() -> void:
	var objectRid: RID = PhysicsServer3D.body_create()
	PhysicsServer3D.body_set_space(objectRid, get_world_3d().space)
	PhysicsServer3D.body_set_mode(objectRid, PhysicsServer3D.BODY_MODE_KINEMATIC)
	PhysicsServer3D.body_add_shape(objectRid, objectShape)
	PhysicsServer3D.body_set_collision_layer(objectRid, 2)

	var spawnPointParent: Node = get_node_or_null("Spawns")
	var index: int = rng.randi_range(0, spawnPointParent.get_child_count() - 1)
	var spawnPoint: Node = spawnPointParent.get_child(index)
	var spawnPosition: Vector3 = spawnPoint.global_position + Vector3(rng.randf_range(-1, 1), 0, rng.randf_range(-1, 1))
	var baseTransform: Transform3D = Transform3D(Basis.IDENTITY, spawnPosition)

	PhysicsServer3D.body_set_shape_transform(objectRid, 0, Transform3D(Basis.IDENTITY, Vector3.ZERO))
	PhysicsServer3D.body_set_state(objectRid, PhysicsServer3D.BODY_STATE_TRANSFORM, baseTransform)

	var meshRid: RID = RenderingServer.instance_create2(capsuleMesh, get_world_3d().scenario)
	RenderingServer.instance_set_transform(meshRid, baseTransform)

	objectArray.append({
		"transform": baseTransform,
		"mesh": meshRid,
		"physicsBody": objectRid,
	})
	objectCount += 1

func killObject(object: Dictionary) -> void:
	objectArray.erase(object)
	PhysicsServer3D.free_rid(object["physicsBody"])
	RenderingServer.free_rid(object["mesh"])
	objectCount -= 1
