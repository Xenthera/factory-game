using Godot;
using System;

public partial class CameraController : Camera3D
{
	[Export] public float moveSpeed = 10.0f; 
	[Export] public float zoomSpeed = 30.0f; 
	[Export] private Node3D RotationPoint;
	
	
	private float zoomDistance = 10;
	private float minZoomDistance = 1.5f;
	private float maxZoomDistance = 220;

	public override void _Ready()
	{
		RotationPoint.RotateY(45);
		Zoom(0);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event is InputEventPanGesture touch)
		{
			Zoom(touch.Delta.Y);
			Rotate(touch.Delta.X);
		}
	}

	public override void _Process(double delta)
	{
		Vector3 direction = Vector3.Zero;

		// Calculate movement based on input and camera's rotation
		if (Input.IsActionPressed("move_forward"))
		{
			direction.Z -= 1;  // Move forward
		}
		if (Input.IsActionPressed("move_back"))
		{
			direction.Z += 1;  // Move backward
		}
		if (Input.IsActionPressed("move_left"))
		{
			direction.X -= 1;  // Move left
		}
		if (Input.IsActionPressed("move_right"))
		{
			direction.X += 1;  // Move right
		}

		if (Input.IsActionPressed("rotate_left"))
		{
			RotationPoint.RotateY(-2 * (float)delta);
		}
		
		if (Input.IsActionPressed("rotate_right"))
		{
			RotationPoint.RotateY(2 * (float)delta);
		}

		float rotation = RotationPoint.Rotation.Y;

		// Normalize the direction to avoid faster diagonals
		direction = direction.Normalized();

		Vector3 moveDir = direction.Rotated(Vector3.Up, rotation).Normalized();
		
		// Apply movement relative to camera's local space
		RotationPoint.GlobalPosition += moveDir * moveSpeed * (float)delta;
		
		LookAt(RotationPoint.GlobalPosition);
	}

	private void Zoom(float deltaDirection)
	{
		zoomDistance += deltaDirection * zoomSpeed * (float)GetProcessDeltaTime();
		zoomDistance = Mathf.Clamp(zoomDistance, minZoomDistance, maxZoomDistance);

		Vector3 forward = Transform.Basis.Z;
		Vector3 absoluteTranslation = forward * zoomDistance;
		
		Transform = new Transform3D(Transform.Basis, absoluteTranslation);
	}

	private void Rotate(float deltaRotation)
	{
		//RotationPoint.RotateY(deltaRotation * (float)GetProcessDeltaTime() * 4);
	}
}
