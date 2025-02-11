using System;
using Godot;

namespace FactoryGame.Scripts;

public partial class GridPlacerTool : Node3D
{
    [Export] private WorldGrid _worldGrid;
    [Export] private PackedScene _ghostItem;
    Camera3D _camera;

    private Node3D _currentGhostItem;
    private Vector3 _currentGhostRotationEuler;

    public override void _Ready()
    {
        _currentGhostItem = _ghostItem.Instantiate<Node3D>();
        AddChild(_currentGhostItem);
        _camera = GetViewport().GetCamera3D();
    }

    public override void _Process(double delta)
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        var mousePos = GetViewport().GetMousePosition();
        Vector3 from = _camera.ProjectRayOrigin(mousePos);
        Vector3 to = from + _camera.ProjectRayNormal(mousePos) * 1000;
        
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        var result = spaceState.IntersectRay(query);

        Vector3 snappedPos = Vector3.Zero;
        
        if (result.Count > 0)
        {
            Vector3 pos = (Vector3)result["position"];
             snappedPos = SnapToGrid(pos);
            _currentGhostItem.GlobalPosition = new Vector3(snappedPos.X, 0, snappedPos.Z);
        }

        if (Input.IsActionJustPressed("rotate"))
        {
            _currentGhostRotationEuler.Y += Mathf.DegToRad(-90);
            _currentGhostItem.Rotation = _currentGhostRotationEuler;
        }

        if (Input.IsActionPressed("place"))
        {
            Node3D testItem = _ghostItem.Instantiate<Node3D>();
            AddChild(testItem);
            testItem.GlobalPosition = snappedPos;
            testItem.Rotation = _currentGhostRotationEuler;
        }   
    }
    
    Vector3 SnapToGrid(Vector3 position, int gridSize = 1, float offset = 0.5f)
    {
        return new Vector3(
            Mathf.RoundToInt((position.X - offset) / gridSize) * gridSize + offset,
            Mathf.RoundToInt(position.Y / gridSize) * gridSize,
            Mathf.RoundToInt((position.Z - offset) / gridSize) * gridSize + offset
        );
    }

}