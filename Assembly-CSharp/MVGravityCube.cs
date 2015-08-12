using System.Collections.Generic;

public class MVGravityCube : MVLogicObject
{
	private const string prefabPath = "Prefabs/GravityCubeObject";

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVGravityCube(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/GravityCubeObject", worldObjects)
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
		MVPhysics.Gravity = 30f;
	}
}
