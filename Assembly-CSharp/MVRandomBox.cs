using System.Collections.Generic;
using MV.WorldObject;

public class MVRandomBox : MVLogicObject
{
	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public MVRandomBox(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVRandomBoxPrefab, worldObjects)
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
