using System;
using MV.WorldObject;
using UnityEngine;

public class FlagEvents : MonoBehaviour
{
	public event EventHandler<EventArgs> FlagCaptured;

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log((object)"Flag collided with");
		MVWorldObjectClient worldObjectGoId = MVGameController.Instance.WOCM.GetWorldObjectGoId(((Object)((Component)other).gameObject).GetInstanceID());
		if (worldObjectGoId.WorldObjectType == WorldObjectType.Avatar && FlagCaptured != null)
		{
			FlagCaptured(this, new EventArgs());
		}
	}
}
