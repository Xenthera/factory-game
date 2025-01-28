using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ConveyorBelt : Node3D, IItemCarrier
{
	[Export] private PackedScene _itemPrefab;

	[Export] public ConveyorBelt OtherConveyorBelt;
	[Export] public ConveyorBelt PreviousConveyorBelt;
	//will be removed when grid manager passes IItemCarriers
	private IItemCarrier _nextCarrier;

	private ConveyorBelt _previousConveyor;
	//temp remove
	[Export] private bool IsLead = false;
	[Export] private bool IsEnd = false;
	

	private int[] _itemContainers;
	private Vector3[] _itemPositions;
	private Node3D[] _visualItems;
	private bool[] _shiftedThisTick;
	

	private Vector3 _startPosition = new(0, 0.1f, 0);
	private Vector3 _endPosition = new(1, 0.1f, 0);

	private float _conveyorSpeed = 5f;

	private float _defaultItemSize = 0.2f;
	private float _conveyorPadding = 0.05f;
	private float _conveyorSpacing;

	private float _t = 0;


	
	
	Dictionary<int, Color> _itemColors = new Dictionary<int, Color>();
	private Random random = new Random();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_conveyorSpacing = _defaultItemSize + _conveyorPadding;
		_nextCarrier = OtherConveyorBelt as IItemCarrier;

		_itemContainers = new int[5];
		_itemPositions = new Vector3[5];
		_visualItems = new Node3D[5];
		_shiftedThisTick = new bool[5];
		
		for (int i = 0; i < _itemPositions.Length; i++)
		{
			_itemPositions[i] = _startPosition + new Vector3(i * (1f / _itemPositions.Length), 0f, 0f);
		}
		
		
		_itemColors.Add(1, Colors.Lavender);
		_itemColors.Add(2, Colors.Green);
		_itemColors.Add(3, Colors.Blue);
		_itemColors.Add(4, Colors.Yellow);
		_itemColors.Add(5, Colors.Purple);
		_itemColors.Add(6, Colors.Orange);
		_itemColors.Add(7, Colors.Cyan);
		_itemColors.Add(8, Colors.Magenta);
		_itemColors.Add(9, Colors.Brown);
		_itemColors.Add(10, Colors.Gray);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Color c = Colors.Cyan;

		for (int i = 0; i < _itemPositions.Length; i++)
		{
			if (_itemContainers[i] != 0)
			{
				Vector3 local = _itemPositions[i] + new Vector3(0f, 0.1f, 0);
				DebugDraw3D.DrawBox(GlobalTransform.Origin + GlobalTransform.Basis * local, GlobalBasis.GetRotationQuaternion(), Vector3.One * 0.15f, _itemColors[_itemContainers[i]], true);
			}
			
		}

		if (Input.IsActionPressed("add") && IsLead)
		{
			var randomIndex = random.Next(_itemColors.Count);
			var randomKey = new List<int>(_itemColors.Keys)[randomIndex];
			AddItem(randomKey);
		}
		if (Input.IsActionPressed("remove") && IsEnd)
		{
			for (int i = 0; i < 3; i++)
			{
				RemoveItem();
			}
		}

		for (int i = 0; i < _visualItems.Length; i++)
		{
			if (_shiftedThisTick[i])
			{
				if (_visualItems[i] == null) continue;

				float xPos = (1f / _itemContainers.Length) * i;
				float nextXPos = (1f / _itemContainers.Length) * (i + 1);
				
				_visualItems[i].Position =
					new Vector3(
						Mathf.Lerp(xPos,
							nextXPos, _t), _visualItems[i].Position.Y, _visualItems[i].Position.Z);
			}
		}
		
		
		

		_t += (float)delta * 6;
		if (_t >= 1.0f)
		{
			for (int i = _visualItems.Length - 1; i >= 0; i--)
			{
				
				if (_shiftedThisTick[i])
				{
					if (i == _visualItems.Length - 1)
					{
						_visualItems[i].QueueFree();
						_visualItems[i] = null;
					}

					if (i < _visualItems.Length - 1)
					{
						_visualItems[i + 1] = _visualItems[i];
						_visualItems[i] = null;
					}
					
				}
				
				
			}
		}
		
		

	}

	private void updateDiscretePositions()
	{
		for (int i = _itemContainers.Length - 1; i >= 0; i--)
		{
			if (i == _itemContainers.Length - 1) //last position on conveyor
			{
				if (_itemContainers[i] == 0 || _nextCarrier == null) continue;
				if (_nextCarrier.AddItem(_itemContainers[i]))
				{
					_itemContainers[i] = 0;
					_shiftedThisTick[i] = true;

				}
			}
			else
			{
				if (_itemContainers[i] != 0 && _itemContainers[i + 1] == 0)
				{
					_itemContainers[i + 1] = _itemContainers[i];
					_itemContainers[i] = 0;
					_shiftedThisTick[i] = true;
				}
			}
		}
		
	}

	private void spawnVisualEntities()
	{
		
		if (_shiftedThisTick[0]) // New Item this tick
		{
			GD.Print("Spawning visual");
			Node3D item = SpawnItemVisual();
			_visualItems[0] = item;
		}
		
		
		
	}

	private void resetVisualTracker()
	{
		for (int i = 0; i < 5; i++)
		{
			_shiftedThisTick[i] = false;
		}
	}

	public void ticketyTick()
	{
		
		resetVisualTracker();
		updateDiscretePositions();
		spawnVisualEntities();
		
		

		

		_t = 0;
	}

	public bool AddItem(int itemId)
	{
		if(_itemContainers[0] != 0) return false;
		_itemContainers[0] = itemId;
		spawnVisualEntities();

		return true;
	}
	

	public bool RemoveItem()
	{
		if (_itemContainers[^1] != 0)
		{
			_itemContainers[^1] = 0;
			return true;	
		}

		return false;
	}

	public bool IsFull()
	{
		return _itemContainers[0] != 0;
	}

	public bool HasItem(int itemId)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<int> GetItems()
	{
		throw new NotImplementedException();
	}

	Node3D SpawnItemVisual()
	{
		if (_itemPrefab == null)
		{
			GD.PrintErr("No Item Prefab Provided");
			return null;
		}
		
		var instance = _itemPrefab.Instantiate<Node3D>();
		AddChild(instance);

		instance.Position = _startPosition;
		return instance;
	}
}
