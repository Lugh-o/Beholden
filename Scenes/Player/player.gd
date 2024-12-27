extends CharacterBody3D
class_name Player

# Game
@onready var level01: Node3D = self.get_parent()
@onready var surviveTimer: Timer = level01.get_node_or_null("SurviveTimer")

# Movement
@onready var speed: float = 10
@onready var jumpVelocity: float = 4.5
@onready var airborneSensitivity: int = 3

# SFX
@onready var sfxNode: Node = get_node_or_null("SFX");
@onready var takingDamageSfx: AudioStreamPlayer = sfxNode.get_node_or_null("TakingDamageSFX");
@onready var levelUpSfx: AudioStreamPlayer = sfxNode.get_node_or_null("LevelUpSFX");
@onready var slideSfx: AudioStreamPlayer = sfxNode.get_node_or_null("SlideSFX");
@onready var dieSfx: AudioStreamPlayer = sfxNode.get_node_or_null("DieSFX");
@onready var shotSfx: AudioStreamPlayer = sfxNode.get_node_or_null("ShotSFX");
@onready var noAmmoShotSfx: AudioStreamPlayer = sfxNode.get_node_or_null("NoAmmoShotSFX");
@onready var reloadSfx: AudioStreamPlayer = sfxNode.get_node_or_null("ReloadSFX");
@onready var hpPickupSfx: AudioStreamPlayer = sfxNode.get_node_or_null("HealingPickupSFX");
@onready var ammoPickupSfx: AudioStreamPlayer = sfxNode.get_node_or_null("AmmoPickupSFX");

# Mouse variables
@onready var mouseSensitivity: float = 0.005

# Damage
@export var invulnerabilityTimer: Timer

@onready var maxHealth: float = 15
@onready var currentHealth: float = maxHealth

# Slide
@onready var collisionShape: CollisionShape3D = self.get_node_or_null("CollisionShape3D")
@onready var isSliding: bool = false
@onready var slideSpeed: float = 60
@onready var friction: float = 0

# Pickup magnet
@export var collisionShapeMagnet: CollisionShape3D

# Double Jump
@onready var canDoubleJump: bool = true
@onready var doubleJumpEnabled: bool = false

# Shots
@export var shotDelayTimer: Timer
@export var reloadTimer: Timer

@onready var shotDelay: float = 0.15
@onready var reloadTime: float = 2.2
@onready var bulletReserve: int = 40
@onready var bulletsInMagazine: int = 6
@onready var magazineSize = 13
@onready var isReloading: bool = false
@onready var isAuto: bool = false
@onready var piercing: int = 0
@onready var damage: int = 1
@onready var shotsFired: int = 1
const raycastBullet = preload("res://Scenes/Bullets/RaycastBullet/RaycastBullet.tscn")

# HUD
@export var fpsCounter: RichTextLabel
@export var crossHair: TextureRect
@export var hpBar: TextureProgressBar
@export var xpBar: TextureProgressBar
@export var reloadBar: TextureProgressBar
@export var bossHp: TextureProgressBar
@export var magLabel: RichTextLabel
@export var timerLabel: RichTextLabel
@export var reloadContainer: HBoxContainer
@export var weaponAnimationPlayer: AnimationPlayer
@export var hurtPlayer: AnimationPlayer
@export var gameOverMenu: GameOverMenu
@export var upgradeMenu: UpgradeMenu
@export var congratulationsMenu: CongratulationsMenu
@export var gun1: TextureRect
@export var gun2: TextureRect
@export var gun3: TextureRect
@export var gun4: TextureRect
@export var gun5: TextureRect

# Crosshair
@onready var currentSeparation: float = 0
@onready var normalSeparation: float = 0.26
@onready var currentRadius: float = 0
@onready var normalRadius: float = 0.09

# Camera
@export var camera: Camera3D
@export var cameraController: Node3D

@onready var tiltLowerLimit = deg_to_rad(-80)
@onready var tiltUpperLimit = deg_to_rad(80)
@onready var isAiming: bool = false
@onready var baseFov: float = 75
@onready var fovChange: float = 1.5

# Shake
@onready var randomStrength: float = 0.01
@onready var shakeFade: float = 12
@onready var shakeStrength: float = 0

# Headbob
@onready var headbobTime: float = 0
@onready var headbobAmplitude: float = 0.08
@onready var headbobFrequency: float = 2.0

# Leveling system
@onready var level: int = 1
@onready var experience: float = 0
@onready var experienceTotal: float = 0
@onready var experienceRequired: float = 0

func _input(event) -> void:
	if (event is InputEventMouseMotion):
		HandleCameraRotation(event);

func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

	currentHealth = maxHealth;
	shotsFired = 1;
	hpBar.value = maxHealth;
	hpBar.max_value = maxHealth;

	shotDelayTimer.wait_time = shotDelay;
	reloadTimer.wait_time = reloadTime;
	reloadBar.max_value = reloadTime;

	bulletReserve = 40;
	experienceRequired = GetRequiredExperience(level + 1);
	xpBar.max_value = experienceRequired;
	(crossHair.material as ShaderMaterial).set_shader_parameter("normal", true);

	magLabel.text = "[font_size=90][center]%d/%d" % [bulletsInMagazine, bulletReserve]

func HandleCameraRotation(mouseMotion: InputEventMouseMotion) -> void:
	self.rotate_y(-mouseMotion.relative.x * mouseSensitivity)
	cameraController.rotate_x(-mouseMotion.relative.y * mouseSensitivity)
	
	var cameraRotation: Vector3 = cameraController.rotation
	cameraRotation.x = clamp(cameraRotation.x, tiltLowerLimit, tiltUpperLimit)
	cameraController.rotation = cameraRotation

func GetRequiredExperience(playerLevel: int) -> int:
	return round(pow(playerLevel, 1.8) + playerLevel * 4)

func _process(_delta) -> void:
	if (surviveTimer.time_left > 0):
		var timeLeft: float = surviveTimer.time_left
		var minutes: int = floor(timeLeft / 60)
		var seconds: int = int(timeLeft) % 60
		timerLabel.text = "[font_size=90][center]%02d:%02d" % [minutes, seconds];
	else:
		timerLabel.hide()
		bossHp.show()
	reloadBar.value = reloadTimer.time_left
	fpsCounter.text = "[right][font_size=54] %d FPS" % [floor(Engine.get_frames_per_second())]

func _physics_process(delta) -> void:
	var velocityTemp: Vector3 = velocity

	# Gravity
	if (!is_on_floor()): velocityTemp += get_gravity() * delta

	# Handle Jump
	if (Input.is_action_just_pressed("jump")):
		if (is_on_floor()):
			velocityTemp.y = jumpVelocity
		elif (canDoubleJump):
			velocityTemp.y = jumpVelocity;
			canDoubleJump = false;

	# Handle Double Jump
	if (is_on_floor()): canDoubleJump = doubleJumpEnabled

	# Reload on Input
	if (Input.is_action_just_pressed("reload")): HandleReload()

	# Aim on Input
	if (Input.is_action_pressed("aim")):
		isAiming = true;
	else:
		isAiming = false;

	# Handle Slide Kinda
	if (Input.is_action_just_pressed("slide") && is_on_floor() && !isSliding):
		slideSfx.play()
		isSliding = true
	if (Input.is_action_just_released("slide")):
		slideSfx.stop()
		isSliding = false
	if (!isSliding):
		slideSfx.stop()
		HandleWalking(delta, velocityTemp)
		HandleHeadbob(delta, velocityTemp)
		
		collisionShape.scale = Vector3(1, 1, 1)
		collisionShape.position = Vector3(collisionShape.position.x, 0, collisionShape.position.z)
		cameraController.position = Vector3(cameraController.position.x, 0, cameraController.position.z)
		friction = 1
	else:
		velocity *= friction;
		collisionShape.scale = Vector3(0.2, 0.2, 0.2)
		collisionShape.position = Vector3(collisionShape.position.x, -0.8, collisionShape.position.z)
		cameraController.position = Vector3(cameraController.position.x, -0.5, cameraController.position.z)
		if (friction >= 0): friction -= 0.01 * delta

	HandleFov(delta, velocityTemp)
	HandleShooting()
	HandleShake(delta)
	HandleCrosshair(delta)
	move_and_slide()

func HandleReload() -> void:
	if (bulletReserve == 0 || bulletsInMagazine == magazineSize):
		return
	elif (!isReloading && bulletReserve > 0):
		reloadContainer.show()
		reloadSfx.play()
		isReloading = true
		reloadTimer.start()
		magLabel.text = "[font_size=90][center]%d/%d" % [bulletsInMagazine, bulletReserve]

func HandleWalking(delta: float, inputVelocity: Vector3) -> void:
	var inputDir: Vector2 = Input.get_vector("moveLeft", "moveRight", "moveForward", "moveBackwards")
	var direction: Vector3 = (transform.basis * Vector3(inputDir.x, 0, inputDir.y)).normalized()
	if (is_on_floor()):
		if (direction != Vector3.ZERO):
			inputVelocity.x = direction.x * speed
			inputVelocity.z = direction.z * speed
		else:
			inputVelocity.x = lerp(inputVelocity.x, direction.x * speed, delta * 7)
			inputVelocity.z = lerp(inputVelocity.z, direction.z * speed, delta * 7)
	else: # lose control if not on floor
		inputVelocity.x = lerp(inputVelocity.x, direction.x * speed, delta * airborneSensitivity)
		inputVelocity.z = lerp(inputVelocity.z, direction.z * speed, delta * airborneSensitivity)
	velocity = inputVelocity

func HandleHeadbob(delta: float, inputVelocity: Vector3) -> void:
	if (is_on_floor()):
		headbobTime += delta * inputVelocity.length()
	# ===== Isso foi feito de armengue pro C#, da pra melhorar======
	var cameraTransform: Transform3D = camera.transform
	var cameraPosition: Vector3 = Vector3.ZERO
	cameraPosition.y = sin(headbobTime * headbobFrequency) * headbobAmplitude
	cameraPosition.x = cos(headbobTime * headbobFrequency / 2) * headbobAmplitude
	cameraTransform.origin = cameraPosition
	camera.transform = cameraTransform
	# ==============================================================

func HandleFov(delta: float, inputVelocity: Vector3) -> void:
	var velocityClamped: float = clamp(inputVelocity.length(), 0.5, speed * 2)
	var targetFov: float = baseFov + fovChange * velocityClamped
	if (isAiming):
		targetFov -= 50
		mouseSensitivity = 0.003
	else:
		mouseSensitivity = 0.005
	camera.fov = lerp(camera.fov, targetFov, delta * 8)

func HandleShooting() -> void:
	var shot: bool
	var crosshairMaterial: ShaderMaterial = crossHair.material
	if (isAuto):
		shot = Input.is_action_pressed("shoot")
	else:
		shot = Input.is_action_just_pressed("shoot")

	if (shot && shotDelayTimer.is_stopped() && !isReloading):
		if (bulletsInMagazine == 0 && bulletReserve == 0):
			noAmmoShotSfx.play()
			return
		shotSfx.play()
		shotDelayTimer.start()
		weaponAnimationPlayer.play("shoot")

		# Apply shake
		shakeStrength = randomStrength
		for i in range(shotsFired):
			magLabel.text = "[font_size=90][center]%d/%d" % [bulletsInMagazine, bulletReserve]
			if (bulletsInMagazine > 0):
				var dispersionRadius: float = crosshairMaterial.get_shader_parameter("radius")
				if (shotsFired == 1):
					dispersionRadius = 0
				bulletsInMagazine -= 1

				var shootPosition = camera.global_position
				shootPosition.y -= 0.3
				var adjustedDirection = GetRandomPointInCircle(-camera.get_camera_transform().basis.z, dispersionRadius).normalized()

				var raycastInstance: RayCast3D = raycastBullet.instantiate()
				raycastInstance.player = self
				self.add_sibling(raycastInstance)
				raycastInstance.global_position = shootPosition
				raycastInstance.target_position = adjustedDirection * 200
				crosshairMaterial.set_shader_parameter("size", 0.3)
				crosshairMaterial.set_shader_parameter("radius", 0.2)
				if (bulletsInMagazine <= 0):
					HandleReload()
				magLabel.text = "[font_size=90][center]%d/%d" % [bulletsInMagazine, bulletReserve]

func GetRandomPointInCircle(direction: Vector3, radius: float) -> Vector3:
	var angle: float = randf_range(0, TAU)
	var randomRadius: float = randf_range(0, radius)
	var offsetX: float = cos(angle) * randomRadius
	var offsetY: float = (sin(angle) * randomRadius) - 0.02

	var right: Vector3 = direction.cross(Vector3.UP).normalized()
	var up: Vector3 = direction.cross(right).normalized()

	return direction + (right * offsetX) + (up * offsetY)

func HandleShake(delta: float) -> void:
	if (shakeStrength > 0):
		shakeStrength = lerp(shakeStrength, 0.0, shakeFade * delta)
		camera.rotation = Vector3(randf_range(-shakeStrength, shakeStrength), randf_range(-shakeStrength, shakeStrength), randf_range(-shakeStrength, shakeStrength))

func HandleCrosshair(delta: float) -> void:
	var crosshairMaterial: ShaderMaterial = crossHair.material

	# Normal Crosshair
	currentSeparation = crosshairMaterial.get_shader_parameter("size")
	currentSeparation = lerp(currentSeparation, normalSeparation, delta * 5)
	crosshairMaterial.set_shader_parameter("size", currentSeparation)

	# Shotgun Crosshair
	currentRadius = crosshairMaterial.get_shader_parameter("size")
	currentRadius = lerp(currentRadius, normalRadius, delta * 5)
	crosshairMaterial.set_shader_parameter("size", currentRadius)

func _onReloadTimerTimeout() -> void:
	isReloading = false
	reloadContainer.hide()
	var bulletsNeeded: int = magazineSize - bulletsInMagazine
	bulletReserve -= bulletsNeeded
	if (bulletReserve >= 0):
		bulletsInMagazine += bulletsNeeded
	else:
		bulletsInMagazine += bulletsNeeded + bulletReserve
		bulletReserve = 0
	magLabel.text = "[font_size=90][center]%d/%d" % [bulletsInMagazine, bulletReserve]

func OnArea3DEntered(area: Area3D) -> void:
	if (area.get_parent().is_in_group("drops")):
		area.get_parent().call("GoToPlayer");

func HandleHit(damageTaken: float) -> void:
	if (invulnerabilityTimer.is_stopped()):
		hurtPlayer.play("hurt")
		# Apply Shake
		shakeStrength = randomStrength
		takingDamageSfx.play()
		invulnerabilityTimer.start()
		currentHealth -= damageTaken
		hpBar.value = currentHealth
		if (currentHealth <= 0):
			currentHealth = 0
			Die()

func Die() -> void:
	gameOverMenu.ShowGameOver()
	dieSfx.play()

func HandleHealing(healing: float) -> void:
	hpPickupSfx.play()
	if (currentHealth < maxHealth):
		currentHealth += healing
		hpBar.value = currentHealth

func GainExperience(amount: float) -> void:
	experienceTotal += amount
	experience += amount
	while (experience >= experienceRequired):
		experience -= experienceRequired
		LevelUp()
	xpBar.value = experience

func LevelUp() -> void:
	levelUpSfx.play()
	upgradeMenu.ShowUpgradeMenu()
	level += 1
	experienceRequired = GetRequiredExperience(level + 1)
	xpBar.max_value = experienceRequired
	if (level == 5):
		gun1.hide()
		gun2.show()
	elif (level == 10):
		gun2.hide()
		gun3.show()
	elif (level == 15):
		gun3.hide()
		gun4.show()
	elif (level == 20):
		gun4.hide()
		gun5.show()

func UpgradeStat(upgrade: String) -> void:
	upgradeMenu.HideUpgradeMenu()

	match upgrade:
		"Speed Boost":
			speed *= 1.25
		"Double Jump":
			doubleJumpEnabled = true
			upgradeMenu.allUpgrades.erase("Double Jump")
		"Extra Health":
			maxHealth += maxHealth / 2
			currentHealth += maxHealth / 2
			hpBar.max_value = maxHealth
		"Faster Reload":
			reloadTime *= 0.75
			reloadBar.max_value = reloadTime
			reloadTimer.wait_time = reloadTime
		"Magnetic Pull":
			collisionShapeMagnet.shape.set("radius", collisionShapeMagnet.shape.get("radius") * 2)
		"Piercing Bullets":
			piercing += 1
		"Shotgun Shells":
			(crossHair.material as ShaderMaterial).set_shader_parameter("normal", false)
			shotsFired += 1
		"More Bullets":
			magazineSize += 3
		"More Damage":
			damage += 1
		"Faster Fire Rate":
			shotDelay *= 0.75 # "25% Faster fire rate"
			shotDelayTimer.wait_time = shotDelay

func HandleAmmoRecover(amount: int) -> void:
	ammoPickupSfx.play()
	bulletReserve += amount
	magLabel.text = "[font_size=90][center]%d/%d" % [bulletsInMagazine, bulletReserve]
	if (bulletsInMagazine <= 0):
		HandleReload()

func ShowCongratulationsMenu() -> void:
	congratulationsMenu.ShowCongratulationsMenu()
