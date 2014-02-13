using System.Collections;
using System.Collections.Generic;

public class MVNegate : MVLogicObject
{
	private const string prefabPath = "Prefabs/NegateObject";

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public MVNegate(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/NegateObject", worldObjects)
	{
	}
}
