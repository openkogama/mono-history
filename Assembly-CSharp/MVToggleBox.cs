using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;

public class MVToggleBox : MVLogicObject
{
	private const string prefabPath = "Prefabs/ToggleBoxObject";

	private bool currentOutputState;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public MVToggleBox(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/ToggleBoxObject", worldObjects)
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
