using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using FactoryGame.Scripts;

public partial class ConveyorBelt : Node3D, IItemCarrier
{

	protected class ConveyorItem
	{
		public Node3D visualItem;

		public ConveyorItem(Node3D visualItem)
		{
			this.visualItem = visualItem;
		}

		public void Destroy()
		{
			visualItem.QueueFree();
		}
	}
	

	[Export] public ConveyorBelt OtherConveyorBelt;
	[Export] public ConveyorBelt PreviousConveyorBelt;
	
	[Export] public Node3D AlternateItemCarrier;
	
	//will be removed when grid manager passes IItemCarriers
	protected IItemCarrier _nextCarrier;

	//temp remove
	[Export] private bool IsLead = false;
	[Export] private bool IsEnd = false;

	public bool IsHead;
	public int ItemBuffer;
	
	protected int[] _itemContainers;
	protected Vector3[] _itemPositions;
	protected Vector3 _itemExitPosition;
	protected ConveyorItem[] _visualItems;

	protected float _yOffset = 0.1f;

	protected bool[] _shifted;
	protected float lerpTimer = 0;
	
	
	Dictionary<int, Color> _itemColors = new Dictionary<int, Color>();
	protected Random random = new Random();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (OtherConveyorBelt != null)
		{
			_nextCarrier = OtherConveyorBelt as IItemCarrier;
		}
		else if (AlternateItemCarrier != null)
		{
			_nextCarrier = AlternateItemCarrier as IItemCarrier;
		}

		_itemContainers = new int[ConveyorBeltManager.ItemsPerConveyorStraight];
		_itemPositions = new Vector3[ConveyorBeltManager.ItemsPerConveyorStraight];
		_visualItems = new ConveyorItem[ConveyorBeltManager.ItemsPerConveyorStraight];
		_shifted = new bool[ConveyorBeltManager.ItemsPerConveyorStraight];
		

		
		for (int i = 0; i < _itemPositions.Length; i++)
		{
			_itemPositions[i] = new Vector3(i * (1f / _itemPositions.Length), _yOffset, 0f);
		}

		_itemExitPosition = _itemPositions[^1];
		_itemExitPosition += new Vector3(1f/_itemPositions.Length, 0f, 0f);

		
		
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
		for (int i = 0; i < _itemPositions.Length; i++)
		{
			if (_itemContainers[i] != 0)
			{
				// Vector3 local = _itemPositions[i] + new Vector3(0f, 0.1f, 0);
				// DebugDraw3D.DrawBox(GlobalTransform.Origin + GlobalTransform.Basis * local, GlobalBasis.GetRotationQuaternion(), Vector3.One * 0.3f, _itemColors[_itemContainers[i]], true);
			}
			//Vector3 shiftedDebugPos = _itemPositions[i] + new Vector3(0f, 0.5f, 0);
			//DebugDraw3D.DrawSphere(GlobalTransform.Origin + GlobalTransform.Basis * shiftedDebugPos, 0.05f,  _shifted[i] ? Colors.Aqua : Colors.Red);
			
		}

		if (Input.IsActionPressed("add") && IsLead)
		{
			var randomIndex = random.Next(ConveyorBeltManager.Instance.ItemModels.Length);
			AddItem(randomIndex + 1);
		}
		if (Input.IsActionPressed("remove") && IsEnd)
		{
			for (int i = 0; i < 3; i++)
			{
				RemoveItem();
			}
		}

		for (int i = _itemContainers.Length - 1; i >= 0; i--)
		//for (int i = 0; i < _itemContainers.Length; i++)
		{
			if (_shifted[i])
			{
				if (i < _itemPositions.Length - 1)
				{
					if (_visualItems[i] != null)
						_visualItems[i].visualItem.Position = _itemPositions[i].Lerp(_itemPositions[i + 1],
							ConveyorBeltManager.Instance.GetCurrentTickTime() % ConveyorBeltManager.conveyorSpeed /
							ConveyorBeltManager.conveyorSpeed);
				}
				else
				{
					if (_visualItems[i] != null)
						_visualItems[i].visualItem.Position = _itemPositions[i].Lerp(_itemExitPosition,
							ConveyorBeltManager.Instance.GetCurrentTickTime() % ConveyorBeltManager.conveyorSpeed /
							ConveyorBeltManager.conveyorSpeed);
				}
			}
		}
	}

	protected virtual void updateDiscretePositions()
	{
		for (int i = _itemContainers.Length - 1; i >= 0; i--)
		{
			if (i == _itemContainers.Length - 1) //last position on conveyor
			{
				if (_itemContainers[i] == 0 || _nextCarrier == null) continue;
				
				if(_nextCarrier.AddItem(_itemContainers[i]))
				{
					_visualItems[i]?.Destroy();
					_visualItems[i] = null;
					_itemContainers[i] = 0;
				}
			}
			else
			{
				if (_itemContainers[i] != 0 && _itemContainers[i + 1] == 0)
				{

					_itemContainers[i + 1] = _itemContainers[i];
					_itemContainers[i] = 0;
					_shifted[i + 1] = true;
				}
			}
		}
	}

	private void spawnVisualEntities(int itemId)
	{
		if (_itemContainers[0] != 0)
		{
			Node3D item = SpawnItemVisual(itemId);
			_visualItems[0] = new ConveyorItem(item);
		}
	}

	public void ticketyTick()
	{
		for (int i = 0; i < _shifted.Length; i++)
		{
			_shifted[i] = false;
		}

		updateDiscretePositions();
		shiftVisualItems();
	}

	protected virtual void shiftVisualItems()
	{
		for (var i = _visualItems.Length - 1; i >= 0; i--)
		{			   

				if (_shifted[i])
				{
					_visualItems[i] = _visualItems[i - 1];
					//_visualItems[i + 1].visualItem.Position = _itemPositions[i + 1];
					_visualItems[i - 1] = null;
				}
		}
	}

	public bool AddItem(int itemId)
	{
		if(_itemContainers[0] != 0) return false;
		_itemContainers[0] = itemId;
		_shifted[0] = true;
		spawnVisualEntities(itemId);

		return true;
	}
	

	public bool RemoveItem()
	{
		if (_itemContainers[^1] != 0)
		{
			_itemContainers[^1] = 0;
			if (_visualItems[^1] != null)
			{
				_visualItems[^1].Destroy();
				_visualItems[^1] = null;
			}
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

	Node3D SpawnItemVisual(int itemId)
	{
		
		var instance = ConveyorBeltManager.Instance.ItemModels[itemId - 1].Instantiate<Node3D>();
		AddChild(instance);

		instance.Position = _itemPositions[0];
		return instance;
	}
}
