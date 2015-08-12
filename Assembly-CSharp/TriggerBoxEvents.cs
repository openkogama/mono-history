using System;
using MV.Common;
using UnityEngine;

public class TriggerBoxEvents : MonoBehaviour
{
	private bool isInTrigger;

	public bool IsInTrigger => isInTrigger;

	public event EventHandler<TriggerEventArgs> TriggerEnterOverride;

	public event EventHandler<TriggerEventArgs> TriggerExitOverride;

	public event EventHandler<TriggerEventArgs> TriggerEnter;

	public event EventHandler<TriggerEventArgs> TriggerExit;

	public void OnMVTriggerEnter(Collider other)
	{
		MVWorldObjectClient validWorldObject = GetValidWorldObject(other);
		if (validWorldObject != null && MVGameController.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.Round)
		{
			isInTrigger = true;
			if (TriggerEnterOverride != null)
			{
				TriggerEnterOverride(this, new TriggerEventArgs(validWorldObject.Id));
			}
			else if (TriggerEnter != null)
			{
				TriggerEnter(this, new TriggerEventArgs(validWorldObject.Id));
			}
		}
	}

	public void OnMVTriggerExit(Collider other)
	{
		MVWorldObjectClient validWorldObject = GetValidWorldObject(other);
		if (validWorldObject != null)
		{
			isInTrigger = false;
			if (TriggerExitOverride != null)
			{
				TriggerExitOverride(this, new TriggerEventArgs(validWorldObject.Id));
			}
			else if (TriggerExit != null)
			{
				TriggerExit(this, new TriggerEventArgs(validWorldObject.Id));
			}
		}
	}

	private MVWorldObjectClient GetValidWorldObject(Collider other)
	{
		MVWorldObjectClient mVWorldObjectClient = MVWorldObjectClientManager.GetMVObject(other.gameObject.transform);
		if (mVWorldObjectClient == null)
		{
			return null;
		}
		int woIDHighestInHierarchyWithComponent = MVGameController.WOCM.GetWoIDHighestInHierarchyWithComponent<Rigidbody>(mVWorldObjectClient.Id);
		if (woIDHighestInHierarchyWithComponent == -1)
		{
			return null;
		}
		if (woIDHighestInHierarchyWithComponent != mVWorldObjectClient.Id)
		{
			mVWorldObjectClient = MVGameController.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		}
		if (mVWorldObjectClient.OwnerActorNr != MVGameController.Game.LocalPlayer.ActorNr)
		{
			return null;
		}
		return mVWorldObjectClient;
	}
}
