using Godot;
using Godot.Collections;

namespace FactoryGame.Scripts;

public partial class WorldGrid : Node3D
{
    private Dictionary<Vector3I, Node3D> nodes = new Dictionary<Vector3I, Node3D>();
    
}