using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ConveyorBelt : Node3D, IItemCarrier
{
	[Export] private PackedScene _itemPrefab;

	[Export] private ConveyorBelt OtherConveyorBelt;
	//will be removed when grid manager passes IItemCarriers
	private IItemCarrier _carrier;
	//temp remove
	[Export] private bool IsLead = false;
	[Export] private bool IsEnd = false;
	
	private List<(int itemID, Node3D itemVisual)> _itemQueueFIFO = new List<(int itemID, Node3D itemVisual)>();

	private Vector3 _startPosition = new(0, 0.1f, 0);
	private Vector3 _endPosition = new(1, 0.1f, 0);

	private float _conveyorSpeed = 1f;

	private float _defaultItemSize = 0.2f;
	private float _conveyorPadding = 0.2f;
	private float _conveyorSpacing;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_conveyorSpacing = _defaultItemSize + _conveyorPadding;
		_carrier = OtherConveyorBelt as IItemCarrier;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("add") && IsLead)
		{
			AddItem(0);
		}
		if (Input.IsActionJustPressed("remove") && IsEnd)
		{
			for (int i = 0; i < 3; i++)
			{
				RemoveItem();
			}
		}

		for (int i = 0; i < _itemQueueFIFO.Count; i++)
		{
			(int _, Node3D itemVisual) = _itemQueueFIFO[i];
			Vector3 currentPosition = itemVisual.Position;
			
			Vector3 direction = (_endPosition - currentPosition).Normalized();
			float speed = _conveyorSpeed * (float)delta;
			Vector3 newPosition = currentPosition + direction * speed;
			
			// Clamp position so items do not overshoot the conveyor's endpoint
			if(newPosition.X - _endPosition.X > 0)
			{
				if (_carrier != null)
				{
					bool success = _carrier.AddItem(0);
					if (success)
					{
						RemoveItem();
					}
				}
				newPosition = _endPosition;
			}
			
			// Check spacing: If the next item ahead is stopped or too close 
			if (i < _itemQueueFIFO.Count - 1)  // There must be another item ahead
			{
				(int _, Node3D nextItemVisual) = _itemQueueFIFO[i + 1];  // Next item in queue
				Vector3 nextItemPos = nextItemVisual.Position;

				// Check if the next item is within spacing distance and is close to stopping
				if (newPosition.DistanceTo(nextItemPos) < _conveyorSpacing)
				{
					newPosition = nextItemPos - (direction * _conveyorSpacing);
				}
			}
			
			if(_carrier is ConveyorBelt otherConveyorBelt)
			{
				
					float? dist = otherConveyorBelt.GetLastItemDistanceFromBeginningOfConveyor();
					
					if (dist != null && dist < _conveyorSpacing)
					{
						float offset = dist.Value - _conveyorSpacing; // should be negative
						GD.Print(offset);
						
						Vector3 nextItemPos = new Vector3(_endPosition.X + offset, newPosition.Y, newPosition.Z);
						
						if(newPosition.X - nextItemPos.X > 0)
						{
							newPosition = nextItemPos;
						}
					}
				
			}
			
			itemVisual.Position = newPosition;
			
		}
	}

	public bool AddItem(int itemId)
	{
		if (_itemQueueFIFO.Any())
		{
			GD.Print(_itemQueueFIFO[0].itemVisual.Position.X);
			if (_itemQueueFIFO[0].itemVisual.Position.X < _conveyorSpacing)
			{
				return false;
			}
		}
		
		if (_itemQueueFIFO.Count >= GetMaxCapacity())
		{
			GD.Print("Max cap: " + GetMaxCapacity());
			return false;
		}

		Node3D itemVisual = SpawnItemVisual();
		if (itemVisual == null)
		{
			GD.PrintErr("Failed to spawn item visual");
		}
		
		_itemQueueFIFO.Insert(0, (itemId, itemVisual));
		
		return true;
	}

	public bool RemoveItem()
	{
		if (_itemQueueFIFO.Count == 0)
		{
			return false;
		}
		
		(int itemID, Node3D itemVisual) = _itemQueueFIFO[_itemQueueFIFO.Count - 1];
		itemVisual?.QueueFree();
		
		_itemQueueFIFO.RemoveAt(_itemQueueFIFO.Count - 1);
		return true;
	}

	public bool HasItem(int itemId)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<int> GetItems()
	{
		throw new NotImplementedException();
	}

	// Rename? Returns the item last (or only) item on the belt's distance from the start of the belt. Returns null if no items exist.
	public float? GetLastItemDistanceFromBeginningOfConveyor()
	{
		if (_itemQueueFIFO.Count == 0)
		{
			return null;
		}
		
		return _itemQueueFIFO[0].itemVisual.Position.DistanceTo(_startPosition);
		
	}


	private int GetMaxCapacity()
	{
		return Mathf.RoundToInt(1f / _conveyorSpacing) + 1;
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
