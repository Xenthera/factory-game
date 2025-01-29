using Godot;

namespace FactoryGame.Scripts;

public partial class ConveyorBeltRight : ConveyorBelt
{

    public override void _Ready()
    {
        base._Ready();
        
        _itemContainers = new int[ConveyorBeltManager.ItemsPerConveyorCurved];
        _itemPositions = new Vector3[ConveyorBeltManager.ItemsPerConveyorCurved];
        _visualItems = new ConveyorItem[ConveyorBeltManager.ItemsPerConveyorCurved];
        _shifted = new bool[ConveyorBeltManager.ItemsPerConveyorCurved];

        _itemPositions[0] = new Vector3(0,     _yOffset, 0);
        _itemPositions[1] = new Vector3(0.25f, _yOffset, 0);
        _itemPositions[2] = new Vector3(0.5f,  _yOffset, 0f);
        _itemPositions[3] = new Vector3(0.5f,  _yOffset, 0.25f);
    }
   
   

}