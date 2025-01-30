using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using FactoryGame.Scripts;

public partial class ConveyorBeltSplitter : ConveyorBelt
{

	

	[Export] public ConveyorBelt[] OtherConveyorBelts;

	protected override void updateDiscretePositions()
	{
		if (!OtherConveyorBelts.Any())
		{
			_nextCarrier = null;
		}
		else
		{
			var randomIndex = random.Next(OtherConveyorBelts.Length);
			_nextCarrier = OtherConveyorBelts[randomIndex];

		}
		base.updateDiscretePositions();
	}
}
