using System.Collections.Generic;
using MV.WorldObject;

public class MVToggleBox : MVLogicObject
{
	private bool currentOutputState;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public MVToggleBox(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVToggleBoxPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
	}

	public override void Initialize()
	{
		OnDataUpdate();
		base.Initialize();
	}

	public override void OnDataUpdate()
	{
		currentOutputState = (bool)Data["state"];
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			outputLinkRef.isSet = currentOutputState;
		}
	}
}
