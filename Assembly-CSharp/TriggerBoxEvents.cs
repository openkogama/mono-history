using System;
using MV.Common;
using UnityEngine;

public class TriggerBoxEvents : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Will be fetched with GetComponent<Collider>(), if null.")]
	private Collider triggerCollider;

	private bool isInTrigger;

	public Collider Collider
	{
		get
		{
			if (!triggerCollider)
			{
				triggerCollider = GetComponent<Collider>();
			}
			return triggerCollider;
		}
	}

	public bool IsInTrigger => isInTrigger;

	public event EventHandler<TriggerEventArgs> TriggerEnterOverride;

	public event EventHandler<TriggerEventArgs> TriggerExitOverride;

	public event EventHandler<TriggerEventArgs> TriggerEnter;

	public event EventHandler<TriggerEventArgs> TriggerExit;

	protected void OnValidate()
	{
		if (triggerCollider == null)
		{
			int num = GetComponents<Collider>().Length;
			if (num > 1)
			{
				Debug.LogWarning("TriggerBoxEvents: triggerCollider is not manually defined, and there are multiple to choose from.");
			}
			else if (num == 1)
			{
				triggerCollider = GetComponent<Collider>();
				triggerCollider.isTrigger = true;
			}
		}
		if (!triggerCollider.isTrigger)
		{
			Debug.LogWarning("TriggerBoxEvents: triggerCollider is not marked as trigger.");
		}
	}

	public void OnMVTriggerEnter(Collider other)
	{
		MVWorldObjectClient validWorldObject = GetValidWorldObject(other);
		if (validWorldObject != null && MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.Round)
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
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<Rigidbody>(mVWorldObjectClient.Id);
		if (woIDHighestInHierarchyWithComponent == -1)
		{
			return null;
		}
		if (woIDHighestInHierarchyWithComponent != mVWorldObjectClient.Id)
		{
			mVWorldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		}
		if (mVWorldObjectClient.OwnerActorNr != MVGameControllerBase.Game.LocalPlayer.ActorNr)
		{
			return null;
		}
		return mVWorldObjectClient;
	}
}
