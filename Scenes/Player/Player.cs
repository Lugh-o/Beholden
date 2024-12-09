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

	// Weapon UI Variables
	[Export] public AnimationPlayer weaponAnimationPlayer;

	// Bullets
	[Export] public RayCast3D weaponGunBarrel;
	public PackedScene MockBullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullets/MockBullet/MockBullet.tscn");
	public MockBullet MockBulletInstance;
	[Export] public Timer shotDelay;

	// Hit variables
	[Export] public Timer invulnerabilityTimer;


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion) HandleCameraRotation(mouseMotion);
	}

	public override void _Ready()
	{
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

		HandleHeadbob(deltaFloat, velocityTemp);
		HandleFov(deltaFloat, velocityTemp);
		HandleShooting();
		MoveAndSlide();
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
		{ // Lose control if not on floor
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
		else
		{
			velocity.X = Mathf.Lerp(velocity.X, direction.X * speed, deltaFloat * airborneSensitivity);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * speed, deltaFloat * airborneSensitivity);
		}
		Velocity = velocity;
	}

	public void HandleShooting()
	{
		if (Input.IsActionPressed("shoot"))
		{
			if (shotDelay.IsStopped())
			{
				shotDelay.Start();
				weaponAnimationPlayer.Play("shoot");
				MockBulletInstance = MockBullet.Instantiate<MockBullet>();
				MockBulletInstance.Position = weaponGunBarrel.GlobalPosition;

				Transform3D currentTransform = MockBulletInstance.Transform;
				currentTransform.Basis = weaponGunBarrel.GlobalTransform.Basis;
				MockBulletInstance.Transform = currentTransform;

				GetParent().AddChild(MockBulletInstance);
			}
		}
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

	public override void Die()
	{

	}

}
