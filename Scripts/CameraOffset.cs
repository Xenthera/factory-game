using Godot;
using System;

public partial class CameraOffset : MeshInstance3D
{
	
	[Export] public Camera3D Camera;
	private Material mat;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		(GetActiveMaterial(0) as ShaderMaterial).SetShaderParameter("camera_position", Camera.GlobalPosition);
		GlobalPosition = new Vector3(Camera.GlobalPosition.X, GlobalPosition.Y, Camera.GlobalPosition.Z);
	}
}
