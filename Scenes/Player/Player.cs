using Godot;
using System;

public partial class Player : Damageable
{

	// Movement Variables 
	public float speed;
	[Export] public float walkSpeed = 5.0f;
	[Export] public float sprintSpeed = 10.0f;
	[Export] public float jumpVelocity = 4.5f;
	[Export] public int airborneSensitivity = 3;

	// Mouse Variables
	public bool mouseInput = false;
	public Vector3 mouseRotation;
	public float rotationInput;
	public float tiltInput;
	[Export] public float mouseSensitivity = 0.005f;

	// Camera Variables
	[Export] public float tiltLowerLimit = Mathf.DegToRad(-80.0f);
	[Export] public float tiltUpperLimit = Mathf.DegToRad(80.0f);
	[Export] public Node3D cameraController;
	[Export] public Camera3D camera;
	[Export] public float headbobFrequency = 2.0f;
	[Export] public float headbobAmplitude = 0.08f;
	[Export] public float headbobTime = 0f;
	[Export] public float baseFov = 75.0f;
	[Export] public float fovChange = 1.5f;
	[Export] public float recoilAmount = 0.05f;

	// Weapon UI Variables
	[Export] public AnimationPlayer weaponAnimationPlayer;
	public TextureRect crossHair;

	// Bullets
	[Export] public Timer shotDelay;
	public PackedScene raycastBullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullets/RaycastBullet/RaycastBullet.tscn");

	// Hit variables
	[Export] public Timer invulnerabilityTimer;

	private float currentSeparation = 0.0f;
	private float normalSeparation = 0.26f;
	private float currentRadius = 0.0f;
	private float normalRadius = 0.09f;

	//Leveling System
	[Export] int level = 1;
	[Export] double experience = 0;
	[Export] double experienceTotal = 0;
	[Export] double experienceRequired = 0;
	[Export] RichTextLabel levelingLabel;
	[Export] UpgradeMenu upgradeMenu;


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion) HandleCameraRotation(mouseMotion);
	}

	public override void _Ready()
	{
		MaxHealth = 10;
		CurrentHealth = MaxHealth;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		crossHair = camera.GetNode<CanvasLayer>("PlayerUI").GetNode<TextureRect>("Crosshair");
		experienceRequired = GetRequiredExperience(level + 1);
		levelingLabel.Text = $"Level: {level}\nExperience: {experience}\nRequired Experience: {experienceRequired}";
	}

	public override void _PhysicsProcess(double delta)
	{
		float deltaFloat = (float)delta;
		Vector3 velocityTemp = Velocity;

		// Gravity
		if (!IsOnFloor()) velocityTemp += GetGravity() * deltaFloat;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor()) velocityTemp.Y = jumpVelocity;

		HandleWalking(deltaFloat, velocityTemp);

		// Handle Sprint
		speed = Input.IsActionPressed("sprint") ? sprintSpeed : walkSpeed;

		HandleFov(deltaFloat, velocityTemp);
		HandleShooting();
		HandleHeadbob(deltaFloat, velocityTemp);
		MoveAndSlide();
		HandleCrosshair(deltaFloat);
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

	public void HandleCrosshair(float deltaFloat)
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
	public void HandleHeadbob(float deltaFloat, Vector3 velocity)
	{
		if (IsOnFloor()) headbobTime += deltaFloat * velocity.Length();
		Transform3D cameraTransform = camera.Transform;
		Vector3 cameraPosition = Vector3.Zero;
		cameraPosition.Y = (float)(Math.Sin(headbobTime * headbobFrequency) * headbobAmplitude);
		cameraPosition.X = (float)(Math.Cos(headbobTime * headbobFrequency / 2) * headbobAmplitude);
		cameraTransform.Origin = cameraPosition;
		camera.Transform = cameraTransform;
	}

	public void HandleFov(float deltaFloat, Vector3 velocity)
	{
		float velocityClamped = Mathf.Clamp(velocity.Length(), 0.5f, sprintSpeed * 2);
		float targetFov = baseFov + fovChange * velocityClamped;
		camera.Fov = Mathf.Lerp(camera.Fov, targetFov, deltaFloat * 8);
	}

	public void HandleCameraRotation(InputEventMouseMotion mouseMotion)
	{
		RotateY(-mouseMotion.Relative.X * mouseSensitivity);
		cameraController.RotateX(-mouseMotion.Relative.Y * mouseSensitivity);

		Vector3 cameraRotation = cameraController.Rotation;
		cameraRotation.X = Mathf.Clamp(cameraRotation.X, tiltLowerLimit, tiltUpperLimit);
		cameraController.Rotation = cameraRotation;
	}

	public void HandleWalking(float deltaFloat, Vector3 velocity)
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

	public void HandleShooting()
	{
		if (Input.IsActionPressed("shoot") && shotDelay.IsStopped())
		{
			shotDelay.Start();
			Vector3 shootPosition = new Vector3(camera.GlobalPosition.X, camera.GlobalPosition.Y - 0.3f, camera.GlobalPosition.Z);
			Shoot(shootPosition, -camera.GetCameraTransform().Basis.Z, 0f, 200f);

			ShaderMaterial crosshairmaterial = (ShaderMaterial)crossHair.Material;
			crosshairmaterial.SetShaderParameter("size", 0.3);
			crosshairmaterial.SetShaderParameter("radius", 0.2);
		}
	}

	public void Shoot(Vector3 origin, Vector3 baseDirection, float spread, float range)
	{
		weaponAnimationPlayer.Play("shoot");

		ApplyRecoil();

		Vector3 adjustedDirection = GetRandomPointInCircle(baseDirection, spread).Normalized();

		RayCast3D raycastInstance = raycastBullet.Instantiate<RayCast3D>();
		GetParent().AddChild(raycastInstance);
		raycastInstance.GlobalPosition = origin;
		raycastInstance.TargetPosition = adjustedDirection * range;

	}

	public void ApplyRecoil()
	{
		cameraController.Rotation += new Vector3((float)GD.RandRange(0.9 * recoilAmount, 1.1 * recoilAmount), 0, 0);
	}

	public override void HandleHit(int damage)
	{
		if (invulnerabilityTimer.IsStopped())
		{
			GD.Print("Hitou");
			invulnerabilityTimer.Start();
			CurrentHealth -= damage;

			if (CurrentHealth <= 0) Die();
		}
	}

	public void HandleHealing(int healing)
	{
		GD.Print("curou");
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

	public void UpgradeStat()
	{
		upgradeMenu.HideUpgradeMenu();
		// coisinhas com o upgrade
	}

}