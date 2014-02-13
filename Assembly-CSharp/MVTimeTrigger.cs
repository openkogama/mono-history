using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVTimeTrigger : MVLogicObject
{
	private const string prefabPath = "Prefabs/TimeTriggerObject";

	private GameObject audioGO;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public MVTimeTrigger(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/TimeTriggerObject", worldObjects)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected Obj, but got Unknown
		interactionFlags |= InteractionFlags.HasSettings;
		audioGO = (GameObject)Object.Instantiate(Resources.Load("Audio/AudioPrefabs/TimeTriggerSound"), Vector3.zero, Quaternion.identity);
		audioGO.transform.parent = gameObject.transform;
	}

	public override void OnInputStateChanged()
	{
	}

	public override void OnDataUpdate()
	{
		if ((int)Data["state"] == 2)
		{
			foreach (Link outputLinkRef in OutputLinkRefs)
			{
				outputLinkRef.isSet = true;
			}
			return;
		}
		if ((int)Data["state"] == 0 && (float)Data["duration"] > 0f)
		{
			foreach (Link outputLinkRef2 in OutputLinkRefs)
			{
				outputLinkRef2.isSet = false;
			}
			return;
		}
		if ((int)Data["state"] == 1 && (float)Data["duration"] <= 0f)
		{
			foreach (Link outputLinkRef3 in OutputLinkRefs)
			{
				outputLinkRef3.isSet = false;
			}
			return;
		}
		if ((int)Data["state"] != 3)
		{
			return;
		}
		foreach (Link outputLinkRef4 in OutputLinkRefs)
		{
			outputLinkRef4.isSet = false;
		}
	}
}
