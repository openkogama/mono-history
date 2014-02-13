using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;

public class MVRandomBox : MVLogicObject
{
	private const string prefabPath = "Prefabs/RandomBoxObject";

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public MVRandomBox(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/RandomBoxObject", worldObjects)
	{
	}

	public override void OnDataUpdate()
	{
		int num = 0;
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			if (num == (int)Data["currentOutput"])
			{
				outputLinkRef.isSet = true;
			}
			else
			{
				outputLinkRef.isSet = false;
			}
			num++;
		}
	}
}
