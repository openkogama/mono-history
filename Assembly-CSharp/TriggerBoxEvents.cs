using System;
using MV.WorldObject;
using UnityEngine;

public class TriggerBoxEvents : MonoBehaviour
{
	public event EventHandler<TriggerEventArgs> TriggerEnter;

	public event EventHandler<TriggerEventArgs> TriggerExit;

	private void OnTriggerEnter(Collider other)
	{
		MVWorldObjectClient worldObjectGoId = MVGameController.Instance.WOCM.GetWorldObjectGoId(((Object)((Component)other).gameObject).GetInstanceID());
		if (worldObjectGoId.WorldObjectType == WorldObjectType.Avatar && TriggerEnter != null)
		{
			TriggerEnter(this, new TriggerEventArgs(worldObjectGoId.Id));
		}
	}

	private void OnTriggerExit(Collider other)
	{
		MVWorldObjectClient worldObjectGoId = MVGameController.Instance.WOCM.GetWorldObjectGoId(((Object)((Component)other).gameObject).GetInstanceID());
		if (worldObjectGoId.WorldObjectType == WorldObjectType.Avatar && TriggerExit != null)
		{
			TriggerExit(this, new TriggerEventArgs(worldObjectGoId.Id));
		}
	}
}
