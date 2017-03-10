using System.Collections.Generic;

public class MVNegate : MVLogicObject
{
	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public MVNegate(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVNegatePrefab, worldObjects)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gameObject);
	}
}
