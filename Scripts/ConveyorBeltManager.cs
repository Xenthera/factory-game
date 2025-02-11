using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FactoryGame.Scripts;

public partial class ConveyorBeltManager : Node
{

    class ConveyorBeltChain
    {
        private HashSet<ConveyorBelt> _conveyorBelts = [];
        private ConveyorBelt _currentHead;
    }
    
    public static ConveyorBeltManager Instance;
    
    [Export] public PackedScene[] ItemModels;
    
    public static int ItemsPerConveyorStraight = 4;
    public static int ItemsPerConveyorCurved = 4;
    public static float conveyorSpeed = 5f;
    
    
    private float _timer;
    private HashSet<ConveyorBelt> _traversalHistory = [];
    private List<ConveyorBeltChain> _conveyorBeltChains = new List<ConveyorBeltChain>();

    private int currentTick = 30;


    public override void _Ready()
    {
        Instance = this;
        Node parent = GetParent();

        // foreach (Node child in parent.GetChildren())
        // {
        //     if (child is ConveyorBelt)
        //     {
        //         GD.Print("Adding belt");
        //
        //         _conveyorBelts.Add(child as ConveyorBelt);
        //     }
        // }

        // ConveyorBelt currentBelt = _conveyorBelts.First();
        // if (currentBelt != null)
        // {
        //     while (true)
        //     {
        //         if(currentBelt.OtherConveyorBelt != null && !_traversalHistory.Contains(currentBelt.OtherConveyorBelt))
        //         {
        //             currentBelt.OtherConveyorBelt.PreviousConveyorBelt = currentBelt;
        //             _traversalHistory.Add(currentBelt);
        //             currentBelt = currentBelt.OtherConveyorBelt;
        //             currentBelt.IsHead = false;
        //         }
        //         else
        //         {
        //             _currentHead = currentBelt;
        //             currentBelt.IsHead = true;
        //             break;
        //         }
        //     }
        // }
        
        // GD.Print("Current head: " + _currentHead.Name);
        //
        // _traversalHistory.Clear();
    }

    public int GetCurrentTickTime()
    {
        return currentTick;
    }

    public override void _PhysicsProcess(double delta)
    {
        _timer += (float)delta;

        if (_timer >= 1f / 60f)
        {
            currentTick++;

            if (currentTick % conveyorSpeed == 0)
            {
                // ConveyorBelt currentBelt = _currentHead;
                // while (true)
                // {
                //     if (currentBelt == null) break;
                //
                //     currentBelt.ticketyTick();
                //
                //     if (currentBelt.PreviousConveyorBelt != null)
                //     {
                //         if (currentBelt.PreviousConveyorBelt == _currentHead)
                //         {
                //             break;
                //         }
                //
                //         currentBelt = currentBelt.PreviousConveyorBelt;
                //     }
                //     else
                //     {
                //         break;
                //     }
                //
                // }
            }

            if(currentTick > 60) currentTick = 0;
            _timer = 0f;
        }   
    }
}
