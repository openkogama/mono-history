using System.Collections.Generic;

public class MVAnd : MVLogicObject
{
	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public MVAnd(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVAndPrefab, worldObjects)
	{
	}
}
