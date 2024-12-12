using Godot;
using System;

public partial class Player : Damageable
{

	// Movement Variables 
	private float speed;
	[Export] private float walkSpeed = 5.0f;
	[Export] private float sprintSpeed = 10.0f;
	[Export] private float jumpVelocity = 4.5f;
	[Export] private int airborneSensitivity = 3;

	// Mouse Variables
	private bool mouseInput = false;
	private Vector3 mouseRotation;
	private float rotationInput;
	private float tiltInput;
	[Export] private float mouseSensitivity = 0.005f;

	// Camera Variables
	[Export] private float tiltLowerLimit = Mathf.DegToRad(-80.0f);
	[Export] private float tiltUpperLimit = Mathf.DegToRad(80.0f);
	[Export] private Node3D cameraController;
	[Export] private Camera3D camera;
	[Export] private float headbobFrequency = 2.0f;
	[Export] private float headbobAmplitude = 0.08f;
	[Export] private float headbobTime = 0f;
	[Export] private float baseFov = 75.0f;
	[Export] private float fovChange = 1.5f;

	[Export] private double randomStrength = 0.01;
	[Export] private double shakeFade = 12.0;
	private double shakeStrength = 0;

	// Weapon UI Variables
	[Export] private AnimationPlayer weaponAnimationPlayer;
	private TextureRect crossHair;

	// Bullets
	[Export] private Timer shotDelayTimer;
	[Export] private Timer reloadTimer;

	private PackedScene raycastBullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullets/RaycastBullet/RaycastBullet.tscn");

	[Export] private float shotDelay = 0.15f;
	[Export] private float reloadTime = 2.2f;

	[Export] private int bulletReserve = 26;
	[Export] private int bulletsInMagazine = 6;
	[Export] private int magazineSize = 13;
	[Export] private bool isAuto = false;
	private bool isReloading = false;

	// Hit variables
	[Export] private Timer invulnerabilityTimer;

	private float currentSeparation = 0.0f;
	private float normalSeparation = 0.26f;
	private float currentRadius = 0.0f;
	private float normalRadius = 0.09f;

	//Leveling System
	[Export] private int level = 1;
	[Export] private double experience = 0;
	[Export] private double experienceTotal = 0;
	[Export] private double experienceRequired = 0;
	[Export] private UpgradeMenu upgradeMenu;

	// UI
	[Export] private RichTextLabel levelingLabel;
	[Export] private RichTextLabel magLabel;
	[Export] private RichTextLabel timerLabel;
	private Timer surviveTimer;

	// Slide variables
	[Export] public float slideSpeed = 60.0f;
	private bool isSliding = false;
	private float friction = 0f;
    public CollisionShape3D colShape;


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion) HandleCameraRotation(mouseMotion);
	}

	public override void _Ready()
	{
		colShape = GetNode<CollisionShape3D>("CollisionShape3D");
		MaxHealth = 10;
		CurrentHealth = MaxHealth;
		shotDelayTimer.WaitTime = shotDelay;
		reloadTimer.WaitTime = reloadTime;
		surviveTimer = GetParent().GetNode<Timer>("SurviveTimer");

		Input.MouseMode = Input.MouseModeEnum.Captured;
		crossHair = camera.GetNode<CanvasLayer>("PlayerUI").GetNode<TextureRect>("Crosshair");
		experienceRequired = GetRequiredExperience(level + 1);
		levelingLabel.Text = $"Level: {level}\nExperience: {experience}\nRequired Experience: {experienceRequired}";
	}

	public override void _Process(double delta)
	{
		magLabel.Text = $"bulletReserve: {bulletReserve} \nbulletsInMagazine: {bulletsInMagazine} \nmagazineSize: {magazineSize} \nisReloading: {isReloading} \n CurrentHealth: {CurrentHealth}";
		int timeLeft = (int)surviveTimer.TimeLeft;
		int minutes = timeLeft / 60;
		int seconds = timeLeft % 60;
		timerLabel.Text = $"[center]{minutes:D2}:{seconds:D2}";
	}

	public override void _PhysicsProcess(double delta)
	{
		float deltaFloat = (float)delta;
		Vector3 velocityTemp = Velocity;

		// Gravity
		if (!IsOnFloor()) velocityTemp += GetGravity() * deltaFloat;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor()) velocityTemp.Y = jumpVelocity;

		// Reload on Input
		if (Input.IsActionJustPressed("reload")) HandleReload();

		if (Input.IsActionJustPressed("slide") && IsOnFloor() && !isSliding) isSliding = true;

		if (Input.IsActionJustReleased("slide")) isSliding = false;


		if (!isSliding)
		{
			HandleWalking(deltaFloat, velocityTemp);
			HandleHeadbob(deltaFloat, velocityTemp);

			speed = sprintSpeed;

			colShape.Scale = new Vector3(1f, 1f, 1f);
			colShape.Position = new Vector3(colShape.Position.X, 0f, colShape.Position.Z);
			cameraController.Position = new Vector3(cameraController.Position.X, 0f, cameraController.Position.Z);
			friction = 1f;
		}

		HandleFov(deltaFloat, velocityTemp);
		HandleShooting();
		HandleShake(deltaFloat);

		if (isSliding)
		{
			Velocity *= friction;
			colShape.Scale = new Vector3(0.2f, 0.2f, 0.2f);
			colShape.Position = new Vector3(colShape.Position.X, -0.8f, colShape.Position.Z);
			cameraController.Position = new Vector3(cameraController.Position.X, -0.5f, cameraController.Position.Z);
			if(friction >= 0f) friction -= 0.1f*deltaFloat;
			GD.Print(friction);
		}

		MoveAndSlide();
		HandleCrosshair(deltaFloat);
	}
	private void OnArea3DEntered(Area3D area)
	{
		if (area.GetParent().IsInGroup("drops")) area.GetParent().Call("GoToPlayer");
	}

	private static Vector3 GetRandomPointInCircle(Vector3 direction, float radius)
	{
		float angle = (float)GD.RandRange(0, Mathf.Pi * 2);

		float randomRadius = (float)GD.RandRange(0, radius);

		float offsetX = Mathf.Cos(angle) * randomRadius;
		float offsetY = (Mathf.Sin(angle) * randomRadius) - 0.02f;

		Vector3 right = direction.Cross(Vector3.Up).Normalized();
		Vector3 up = direction.Cross(right).Normalized();

		return direction + (right * offsetX) + (up * offsetY);
	}

	private void HandleCrosshair(float deltaFloat)
	{
		ShaderMaterial crosshairmaterial = (ShaderMaterial)crossHair.Material;

		//Normal Crosshair
		currentSeparation = (float)crosshairmaterial.GetShaderParameter("size");
		currentSeparation = Mathf.Lerp(currentSeparation, normalSeparation, (float)(5 * deltaFloat));
		crosshairmaterial.SetShaderParameter("size", currentSeparation);

		//Shotgun crosshair
		currentRadius = (float)crosshairmaterial.GetShaderParameter("radius");
		currentRadius = Mathf.Lerp(currentRadius, normalRadius, (float)(5 * deltaFloat));
		crosshairmaterial.SetShaderParameter("radius", currentRadius);
	}

	private void HandleHeadbob(float deltaFloat, Vector3 velocity)
	{
		if (IsOnFloor()) headbobTime += deltaFloat * velocity.Length();
		Transform3D cameraTransform = camera.Transform;
		Vector3 cameraPosition = Vector3.Zero;
		cameraPosition.Y = (float)(Math.Sin(headbobTime * headbobFrequency) * headbobAmplitude);
		cameraPosition.X = (float)(Math.Cos(headbobTime * headbobFrequency / 2) * headbobAmplitude);
		cameraTransform.Origin = cameraPosition;
		camera.Transform = cameraTransform;
	}

	private void HandleFov(float deltaFloat, Vector3 velocity)
	{
		float velocityClamped = Mathf.Clamp(velocity.Length(), 0.5f, sprintSpeed * 2);
		float targetFov = baseFov + fovChange * velocityClamped;
		camera.Fov = Mathf.Lerp(camera.Fov, targetFov, deltaFloat * 8);
	}

	private void HandleCameraRotation(InputEventMouseMotion mouseMotion)
	{
		RotateY(-mouseMotion.Relative.X * mouseSensitivity);
		cameraController.RotateX(-mouseMotion.Relative.Y * mouseSensitivity);

		Vector3 cameraRotation = cameraController.Rotation;
		cameraRotation.X = Mathf.Clamp(cameraRotation.X, tiltLowerLimit, tiltUpperLimit);
		cameraController.Rotation = cameraRotation;
	}

	private void HandleWalking(float deltaFloat, Vector3 velocity)
	{
		Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackwards");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (IsOnFloor())
		{
			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * speed;
				velocity.Z = direction.Z * speed;
			}
			else
			{
				velocity.X = Mathf.Lerp(velocity.X, direction.X * speed, deltaFloat * 7);
				velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * speed, deltaFloat * 7);
			}
		}
		else // Lose control if not on floor
		{
			velocity.X = Mathf.Lerp(velocity.X, direction.X * speed, deltaFloat * airborneSensitivity);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * speed, deltaFloat * airborneSensitivity);
		}
		Velocity = velocity;
	}

	private void HandleShooting()
	{
		bool shot = isAuto ? Input.IsActionPressed("shoot") : Input.IsActionJustPressed("shoot");

		if (shot && shotDelayTimer.IsStopped() && !isReloading)
		{
			if (bulletsInMagazine == 0 && bulletReserve == 0)
			{
				GD.Print("out of ammo");
				return;
			}
			shotDelayTimer.Start();
			weaponAnimationPlayer.Play("shoot");

			ApplyShake();

			bulletsInMagazine--;
			Vector3 shootPosition = camera.GlobalPosition with { Y = camera.GlobalPosition.Y - 0.3f };
			Vector3 adjustedDirection = GetRandomPointInCircle(-camera.GetCameraTransform().Basis.Z, 0f).Normalized();

			RayCast3D raycastInstance = raycastBullet.Instantiate<RayCast3D>();
			GetParent().AddChild(raycastInstance);
			raycastInstance.GlobalPosition = shootPosition;
			raycastInstance.TargetPosition = adjustedDirection * 200f;

			ShaderMaterial crosshairmaterial = (ShaderMaterial)crossHair.Material;
			crosshairmaterial.SetShaderParameter("size", 0.3);
			crosshairmaterial.SetShaderParameter("radius", 0.2);
			if (bulletsInMagazine <= 0)
			{
				HandleReload();
			}
		}
	}

	private void HandleShake(float deltaFloat)
	{
		if (shakeStrength > 0)
		{
			shakeStrength = Mathf.Lerp(shakeStrength, 0, shakeFade * deltaFloat);

			camera.Rotation = new Vector3((float)GD.RandRange(-shakeStrength, shakeStrength), (float)GD.RandRange(-shakeStrength, shakeStrength), (float)GD.RandRange(-shakeStrength, shakeStrength));
		}
	}

	private void ApplyShake()
	{
		shakeStrength = randomStrength;
	}

	public override void HandleHit(int damage)
	{
		if (invulnerabilityTimer.IsStopped())
		{
			invulnerabilityTimer.Start();
			CurrentHealth -= damage;

			if (CurrentHealth <= 0)
			{
				CurrentHealth = 0;
				Die();
			}
		}
	}

	public void HandleHealing(int healing)
	{
		if (CurrentHealth < MaxHealth) CurrentHealth += healing;
	}

	public override void Die()
	{

	}

	private static double GetRequiredExperience(int level)
	{
		return Math.Round(Math.Pow(level, 1.8) + level * 4);
	}

	public void GainExperience(double amount)
	{
		experienceTotal += amount;
		experience += amount;
		while (experience >= experienceRequired)
		{
			experience -= experienceRequired;
			LevelUp();
		}
		levelingLabel.Text = $"Level: {level}\nExperience: {experience}\nRequired Experience: {experienceRequired}";
	}

	private void LevelUp()
	{
		upgradeMenu.ShowUpgradeMenu();
		level += 1;
		experienceRequired = GetRequiredExperience(level + 1);
	}

	public int getLevel()
	{
		return level;
	}

	public void UpgradeStat()
	{
		upgradeMenu.HideUpgradeMenu();
	}

	private void HandleReload()
	{
		if (bulletReserve == 0 || bulletsInMagazine == magazineSize)
		{
			GD.Print("unable to reload");
		}
		else if (!isReloading && bulletReserve > 0)
		{
			isReloading = true;
			reloadTimer.Start();
		}
	}

	private void _onReloadTimerTimeout()
	{
		isReloading = false;
		int bulletsNeeded = magazineSize - bulletsInMagazine;
		bulletReserve -= bulletsNeeded;

		if (bulletReserve >= 0)
		{
			bulletsInMagazine += bulletsNeeded;
		}
		else
		{
			bulletsInMagazine += bulletsNeeded + bulletReserve;
			bulletReserve = 0;
		}
	}

	public void HandleAmmoRecover(int amount)
	{
		bulletReserve += amount;
		if (bulletsInMagazine <= 0)
		{
			HandleReload();
		}
	}

}