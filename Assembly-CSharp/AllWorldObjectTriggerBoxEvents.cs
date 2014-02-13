using System;
using MV.Common;
using UnityEngine;

public class AllWorldObjectTriggerBoxEvents : MonoBehaviour
{
	public event EventHandler<TriggerEventArgs> TriggerEnter;

	public event EventHandler<TriggerEventArgs> TriggerExit;

	private void OnTriggerEnter(Collider other)
	{
		if (MVGameController.Instance.Game.NetworkGameStateListener.CurrentGameState != MVGameStateType.Round)
		{
			return;
		}
		MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(((Component)other).gameObject.transform);
		if (mVObject != null)
		{
			Debug.Log((object)("OnTriggerEnter wo: " + mVObject));
			if (TriggerEnter != null)
			{
				TriggerEnter(this, new TriggerEventArgs(mVObject.Id));
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(((Component)other).gameObject.transform);
		if (mVObject != null)
		{
			Debug.Log((object)("OnTriggerExit wo: " + mVObject));
			if (TriggerExit != null)
			{
				TriggerExit(this, new TriggerEventArgs(mVObject.Id));
			}
		}
	}
}
