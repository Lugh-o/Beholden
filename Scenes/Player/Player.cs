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

	[Export] private int bulletReserve = 40;
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
	[Export] private RichTextLabel magLabel;
	[Export] private RichTextLabel timerLabel;
	[Export] private GameOverMenu gameOverMenu;
	[Export] public CongratulationsMenu congratulationsMenu;
	[Export] private TextureRect gun1;
	[Export] private TextureRect gun2;
	[Export] private TextureRect gun3;
	[Export] private TextureRect gun4;
	[Export] private TextureRect gun5;
	[Export] private TextureProgressBar hpBar;
	[Export] private TextureProgressBar xpBar;
	[Export] private TextureProgressBar reloadBar;
	[Export] private HBoxContainer reloadContainer;
	[Export] private AnimationPlayer hurtPlayer;

	private Level01 level01;
	private Timer surviveTimer;

	// Slide variables
	[Export] public float slideSpeed = 60.0f;
	private bool isSliding = false;
	private float friction = 0f;
	public CollisionShape3D colShape;

	// SFX Variables
	private AudioStreamPlayer takingDamageSfx;
	private AudioStreamPlayer levelUpSfx;
	private AudioStreamPlayer slideSfx;
	private AudioStreamPlayer dieSfx;
	// private AudioStreamPlayer footstepSfx;
	private AudioStreamPlayer shotSfx;
	private AudioStreamPlayer noAmmoShotSfx;
	private AudioStreamPlayer reloadSfx;
	private AudioStreamPlayer hpPickupSfx;
	private AudioStreamPlayer ammoPickupSfx;

	//Double jump
	public bool canDoubleJump = true;
	public bool doubleJumpEnabled = false;

	//Magnetic Field
	private CollisionShape3D colShapeMagnetic;

	//Piercing
	public int piercing = 0;

	//Shotgun Shells
	private int shotsFired = 1;

	//More Damage
	public int damage = 1;
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion) HandleCameraRotation(mouseMotion);
	}

	public override void _Ready()
	{
		Node sfxNode = GetNodeOrNull("SFX");
		takingDamageSfx = sfxNode.GetNodeOrNull<AudioStreamPlayer>("TakingDamageSFX");
		levelUpSfx = sfxNode.GetNodeOrNull<AudioStreamPlayer>("LevelUpSFX");
		slideSfx = sfxNode.GetNodeOrNull<AudioStreamPlayer>("SlideSFX");
		dieSfx = sfxNode.GetNodeOrNull<AudioStreamPlayer>("DieSFX");
		// footstepsSfx = sfxNode.GetNodeOrNull<AudioStreamPlayer>("FootstepSFX");
		shotSfx = sfxNode.GetNodeOrNull<AudioStreamPlayer>("ShotSFX");
		noAmmoShotSfx = sfxNode.GetNodeOrNull<AudioStreamPlayer>("NoAmmoShotSFX");
		reloadSfx = sfxNode.GetNodeOrNull<AudioStreamPlayer>("ReloadSFX");
		hpPickupSfx = sfxNode.GetNodeOrNull<AudioStreamPlayer>("HealingPickupSFX");
		ammoPickupSfx = sfxNode.GetNodeOrNull<AudioStreamPlayer>("AmmoPickupSFX");

		colShape = GetNode<CollisionShape3D>("CollisionShape3D");
		colShapeMagnetic = GetNode<Area3D>("Area3D").GetNode<CollisionShape3D>("CollisionShape3D");
		MaxHealth = 10;
		CurrentHealth = MaxHealth;

		hpBar.Value = MaxHealth;
		hpBar.MaxValue = MaxHealth;

		shotDelayTimer.WaitTime = shotDelay;
		reloadTimer.WaitTime = reloadTime;
		reloadBar.MaxValue = reloadTime;

		level01 = (Level01)GetParent();
		surviveTimer = level01.GetNode<Timer>("SurviveTimer");
		bulletReserve = 40;

		Input.MouseMode = Input.MouseModeEnum.Captured;
		crossHair = camera.GetNode<CanvasLayer>("PlayerUI").GetNode<TextureRect>("Crosshair");

		experienceRequired = GetRequiredExperience(level + 1);
		xpBar.MaxValue = experienceRequired;

		magLabel.Text = $"[font_size=90][center]{bulletsInMagazine}/{bulletReserve}";
	}

	public override void _Process(double delta)
	{
		int timeLeft = (int)surviveTimer.TimeLeft;
		int minutes = timeLeft / 60;
		int seconds = timeLeft % 60;
		timerLabel.Text = $"[font_size=90][center]{minutes:D2}:{seconds:D2}";
		reloadBar.Value = reloadTimer.TimeLeft;
	}

	public override void _PhysicsProcess(double delta)
	{
		float deltaFloat = (float)delta;
		Vector3 velocityTemp = Velocity;

		// Gravity
		if (!IsOnFloor()) velocityTemp += GetGravity() * deltaFloat;
		// Handle Jump.
		if (Input.IsActionJustPressed("jump"))
		{
			if (IsOnFloor())
			{
				velocityTemp.Y = jumpVelocity;
			}
			else if (canDoubleJump)
			{
				velocityTemp.Y = jumpVelocity;
				canDoubleJump = false;
			}
		}

		//Handle Double Jump
		if (IsOnFloor()) canDoubleJump = doubleJumpEnabled;

		// Reload on Input
		if (Input.IsActionJustPressed("reload")) HandleReload();

		// Handle Slide kinda
		if (Input.IsActionJustPressed("slide") && IsOnFloor() && !isSliding)
		{
			slideSfx.Play();
			isSliding = true;
		}

		if (Input.IsActionJustReleased("slide"))
		{
			slideSfx.Stop();
			isSliding = false;
		}
		if (!isSliding)
		{

			slideSfx.Stop();
			HandleWalking(deltaFloat, velocityTemp);
			HandleHeadbob(deltaFloat, velocityTemp);

			speed = sprintSpeed;

			colShape.Scale = new Vector3(1f, 1f, 1f);
			colShape.Position = new Vector3(colShape.Position.X, 0f, colShape.Position.Z);
			cameraController.Position = new Vector3(cameraController.Position.X, 0f, cameraController.Position.Z);
			friction = 1f;
		}
		else
		{
			Velocity *= friction;
			colShape.Scale = new Vector3(0.2f, 0.2f, 0.2f);
			colShape.Position = new Vector3(colShape.Position.X, -0.8f, colShape.Position.Z);
			cameraController.Position = new Vector3(cameraController.Position.X, -0.5f, cameraController.Position.Z);
			if (friction >= 0f) friction -= 0.01f * deltaFloat;
		}

		HandleFov(deltaFloat, velocityTemp);
		HandleShooting();
		HandleShake(deltaFloat);
		HandleCrosshair(deltaFloat);
		MoveAndSlide();
	}

	private static void OnArea3DEntered(Area3D area)
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
		ShaderMaterial crosshairmaterial = (ShaderMaterial)crossHair.Material;

		if (shot && shotDelayTimer.IsStopped() && !isReloading)
		{
			if (bulletsInMagazine == 0 && bulletReserve == 0)
			{
				noAmmoShotSfx.Play();
				return;
			}
			shotSfx.Play();
			shotDelayTimer.Start();
			weaponAnimationPlayer.Play("shoot");

			//Apply shake
			shakeStrength = randomStrength;

			for (int i = 0; i < shotsFired; i++)
			{
			magLabel.Text = $"[font_size=90][center]{bulletsInMagazine}/{bulletReserve}";

			if (bulletsInMagazine > 0)
			{
				float dispersionRadius = (float)crosshairmaterial.GetShaderParameter("radius");
				if (shotsFired == 1) dispersionRadius = 0;
				bulletsInMagazine--;
				Vector3 shootPosition = camera.GlobalPosition with { Y = camera.GlobalPosition.Y - 0.3f };
				Vector3 adjustedDirection = GetRandomPointInCircle(-camera.GetCameraTransform().Basis.Z, dispersionRadius).Normalized();

				RayCast3D raycastInstance = raycastBullet.Instantiate<RayCast3D>();
				(raycastInstance as RaycastBullet).player = this;
				GetParent().AddChild(raycastInstance);
				raycastInstance.GlobalPosition = shootPosition;
				raycastInstance.TargetPosition = adjustedDirection * 200f;

				crosshairmaterial.SetShaderParameter("size", 0.3);
				crosshairmaterial.SetShaderParameter("radius", 0.2);
				if (bulletsInMagazine <= 0)
				{
					HandleReload();
				}
				magLabel.Text = $"[font_size=90][center]{bulletsInMagazine}/{bulletReserve}";
                }
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

	public override void HandleHit(int damage)
	{
		if (invulnerabilityTimer.IsStopped())
		{
			hurtPlayer.Play("hurt");
			//Apply shake
			shakeStrength = randomStrength;

			takingDamageSfx.Play();
			invulnerabilityTimer.Start();
			CurrentHealth -= damage;
			hpBar.Value = CurrentHealth;

			if (CurrentHealth <= 0)
			{
				CurrentHealth = 0;
				Die();
			}
		}
	}

	public void HandleHealing(int healing)
	{
		hpPickupSfx.Play();
		if (CurrentHealth < MaxHealth) CurrentHealth += healing;
		hpBar.Value = CurrentHealth;
	}

	public override void Die()
	{
		gameOverMenu.ShowGameOver();
		dieSfx.Play();
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
		xpBar.Value = experience;
	}

	private void LevelUp()
	{
		levelUpSfx.Play();
		upgradeMenu.ShowUpgradeMenu();
		level += 1;
		experienceRequired = GetRequiredExperience(level + 1);
		xpBar.MaxValue = experienceRequired;

		if (level == 5)
		{
			gun1.Hide();
			gun2.Show();
		}
		else if (level == 10)
		{
			gun2.Hide();
			gun3.Show();
		}
		else if (level == 15)
		{
			gun3.Hide();
			gun4.Show();
		}
		else if (level == 20)
		{
			gun4.Hide();
			gun5.Show();
		}
	}

	public int getLevel()
	{
		return level;
	}

	public void UpgradeStat(string upgrade)
	{
		upgradeMenu.HideUpgradeMenu();

		switch (upgrade)
		{
			case "Speed Boost":
				sprintSpeed *= 1.25f;
				break;
			case "Double Jump":
				doubleJumpEnabled = true;
				upgradeMenu.allUpgrades.Remove("Double Jump");
				break;
			case "Extra Health":
				MaxHealth += MaxHealth / 2;
				CurrentHealth += MaxHealth / 2;
				hpBar.MaxValue = MaxHealth;
				break;
			case "Faster Reload":
				reloadTime *= 0.75f;
				reloadBar.MaxValue = reloadTime;
				reloadTimer.WaitTime = reloadTime;
				break;
			case "Magnetic Pull":
				var currentRadius = (float)colShapeMagnetic.Shape.Get("radius");
				GD.Print(currentRadius);
				colShapeMagnetic.Shape.Set("radius", currentRadius * 2f);
				break;
			case "Piercing Bullets":
				piercing += 1;
				break;
			case "Shotgun Shells":
				(crossHair.Material as ShaderMaterial).SetShaderParameter("normal", false);
				shotsFired += 1;
				break;
			case "More Bullets":
				magazineSize += 3;
				break;
            case "More Damage":
                damage += 1;
                break;
            case "Faster Fire Rate":
				shotDelay *= 0.75f; //"25% Faster fire rate"
                shotDelayTimer.WaitTime = shotDelay;
                break;
        }
	}

	private void HandleReload()
	{
		if (bulletReserve == 0 || bulletsInMagazine == magazineSize)
		{
			GD.Print("unable to reload");
		}
		else if (!isReloading && bulletReserve > 0)
		{
			reloadContainer.Show();
			reloadSfx.Play();
			isReloading = true;
			reloadTimer.Start();
			magLabel.Text = $"[font_size=90][center]{bulletsInMagazine}/{bulletReserve}";

		}
	}

	private void _onReloadTimerTimeout()
	{
		isReloading = false;
		reloadContainer.Hide();
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
		magLabel.Text = $"[font_size=90][center]{bulletsInMagazine}/{bulletReserve}";
	}

	public void HandleAmmoRecover(int amount)
	{
		ammoPickupSfx.Play();
		bulletReserve += amount;
		magLabel.Text = $"[font_size=90][center]{bulletsInMagazine}/{bulletReserve}";
		if (bulletsInMagazine <= 0) HandleReload();
	}

	public void ShowCongratulationsMenu(){
		congratulationsMenu.ShowCongratulationsMenu();
	}

}
