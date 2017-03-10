using System.Collections.Generic;

public class MVBattery : MVLogicObject
{
	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public MVBattery(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVBatteryPrefab, worldObjects)
	{
	}
}
