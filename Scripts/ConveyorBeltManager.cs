using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FactoryGame.Scripts;

public partial class ConveyorBeltManager : Node
{
    
    public static ConveyorBeltManager Instance;
    
    [Export] public PackedScene[] ItemModels;
    
    public static int ItemsPerConveyorStraight = 4;
    public static int ItemsPerConveyorCurved = 4;
    
    private HashSet<ConveyorBelt> _conveyorBelts = [];
    private ConveyorBelt _currentHead;
    
    private float _timer;
    private HashSet<ConveyorBelt> _traversalHistory = [];


    public override void _Ready()
    {
        Instance = this;
        Node parent = GetParent();

        foreach (Node child in parent.GetChildren())
        {
            if (child is ConveyorBelt)
            {
                GD.Print("Adding belt");

                _conveyorBelts.Add(child as ConveyorBelt);
            }
        }

        ConveyorBelt currentBelt = _conveyorBelts.First();
        if (currentBelt != null)
        {
            while (true)
            {
                if(currentBelt.OtherConveyorBelt != null && !_traversalHistory.Contains(currentBelt.OtherConveyorBelt))
                {
                    currentBelt.OtherConveyorBelt.PreviousConveyorBelt = currentBelt;
                    _traversalHistory.Add(currentBelt);
                    currentBelt = currentBelt.OtherConveyorBelt;
                }
                else
                {
                    _currentHead = currentBelt;
                    break;
                }
            }
        }
        
        GD.Print("Current head: " + _currentHead.Name);

        _traversalHistory.Clear();
    }

    public override void _Process(double delta)
    {
        _timer += (float)delta;

        if (_timer >= 1f / (60f / 2f))
        {
            ConveyorBelt currentBelt = _currentHead;
            while (true)
            {
                if (currentBelt == null) break;
                
                currentBelt.ticketyTick();
        
                if (currentBelt.PreviousConveyorBelt != null)
                {
                    if (currentBelt.PreviousConveyorBelt == _currentHead)
                    {
                        break;
                    }
                    currentBelt = currentBelt.PreviousConveyorBelt;
                }
                else
                {
                    break;
                }
                
            }
            
            _timer = 0f;
        }   
    }
}
