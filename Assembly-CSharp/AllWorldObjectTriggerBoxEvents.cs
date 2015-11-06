using System;
using MV.Common;
using UnityEngine;

public class AllWorldObjectTriggerBoxEvents : MonoBehaviour
{
	public event EventHandler<TriggerEventArgs> TriggerEnter;

	public event EventHandler<TriggerEventArgs> TriggerExit;

	private void OnTriggerEnter(Collider other)
	{
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.Round)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(other.gameObject.transform);
			if (mVObject != null && TriggerEnter != null)
			{
				TriggerEnter(this, new TriggerEventArgs(mVObject.Id));
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(other.gameObject.transform);
		if (mVObject != null)
		{
			Debug.Log("OnTriggerExit wo: " + mVObject);
			if (TriggerExit != null)
			{
				TriggerExit(this, new TriggerEventArgs(mVObject.Id));
			}
		}
	}
}
