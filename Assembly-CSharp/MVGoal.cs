using System.Collections;
using System.Collections.Generic;

public class MVGoal : MVLogicObject
{
	private const string prefabPath = "Prefabs/GoalObject";

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVGoal(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/GoalObject", worldObjects)
	{
	}
}
