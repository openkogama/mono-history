using System.Collections;
using System.Collections.Generic;

public class MVBattery : MVLogicObject
{
	private const string prefabPath = "Prefabs/BatteryObject";

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public MVBattery(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/BatteryObject", worldObjects)
	{
	}
}
