using System.Collections.Generic;

public class MVAnd : MVLogicObject
{
	private const string prefabPath = "Prefabs/AndObject";

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public MVAnd(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/AndObject", worldObjects)
	{
	}
}
