using System.Collections.Generic;

public class MVBattery : MVLogicObject
{
	private const string prefabPath = "Prefabs/BatteryObject";

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public MVBattery(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/BatteryObject", worldObjects)
	{
	}
}
