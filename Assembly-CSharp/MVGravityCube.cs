using System.Collections.Generic;

public class MVGravityCube : MVLogicObject
{
	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVGravityCube(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVGravityCubePrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
	}

	public override void Initialize()
	{
		base.Initialize();
		OnDataUpdate();
	}

	public override void OnDataUpdate()
	{
		MVPhysics.Gravity = (float)Data["gravity"];
	}

	public override bool IsSingletonObject()
	{
		return true;
	}

	public override void Destroy()
	{
		MVPhysics.Reset();
		base.Destroy();
	}
}
