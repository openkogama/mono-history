using System.Collections.Generic;

public class MVGoal : MVLogicObject
{
	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVGoal(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVGoalPrefab, worldObjects)
	{
	}
}
