using System;
using UnityEngine;

public class UseInteractor
{
	private Func<int, bool> useFunction;

	private int woOwnerId;

	private Collider triggerCollider;

	private bool reset;

	public bool Reset => reset;

	public int WoOwnerId => woOwnerId;

	public Collider TrigggerCollider => triggerCollider;

	public UseInteractor(int woOwnerId, bool reset, Collider triggerCollider, Func<int, bool> useFunction)
	{
		this.woOwnerId = woOwnerId;
		this.useFunction = useFunction;
		this.triggerCollider = triggerCollider;
		this.reset = reset;
	}

	public bool Use(int userWoID)
	{
		return useFunction(userWoID);
	}

	private UseInteractorHandler GetUseInteractorHandler(int woID)
	{
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(woID);
		if (worldObjectClient != null)
		{
			return worldObjectClient.GameObject.GetComponent<UseInteractorHandler>();
		}
		return null;
	}

	public void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		UseInteractorHandler useInteractorHandler = GetUseInteractorHandler(e.instigatorWOID);
		if ((Object)(object)useInteractorHandler != (Object)null)
		{
			useInteractorHandler.AddUseInteractor(this);
		}
	}

	public void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		UseInteractorHandler useInteractorHandler = GetUseInteractorHandler(e.instigatorWOID);
		if ((Object)(object)useInteractorHandler != (Object)null)
		{
			useInteractorHandler.RemoveUseInteractor(this);
		}
	}
}
