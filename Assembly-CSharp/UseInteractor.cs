using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class UseInteractor
{
	private Func<int, bool> useFunction;

	private Func<MVInteractableBase, bool> checkCanUseFunction;

	private Collider triggerCollider;

	private bool reset;

	private int woOwnerID;

	private UseInteratorVisualization useInteractorVisuals;

	public bool Reset => reset;

	public Collider TriggerCollider => triggerCollider;

	public int WoOwnerID => woOwnerID;

	public UseInteractor(int woOwnerID, GameObject owner, bool reset, Collider triggerCollider, Func<int, bool> useFunction, Func<MVInteractableBase, bool> checkCanUseFunction = null, float yOffset = 2.5f)
	{
		useInteractorVisuals = owner.AddComponent<UseInteratorVisualization>();
		useInteractorVisuals.Initialize(yOffset);
		this.woOwnerID = woOwnerID;
		this.useFunction = useFunction;
		this.triggerCollider = triggerCollider;
		this.reset = reset;
		this.checkCanUseFunction = checkCanUseFunction;
	}

	public bool GetInteractorCanBeUsed(MVInteractableBase avatarInteractable)
	{
		if (checkCanUseFunction != null)
		{
			return checkCanUseFunction(avatarInteractable);
		}
		return true;
	}

	public bool Use(int userWoID)
	{
		if ((EvaluateRequirementsUsability() & UseGUIResult.CannotAfford) != UseGUIResult.CannotAfford)
		{
			return useFunction(userWoID);
		}
		if ((EvaluateRequirementsUsability() & UseGUIResult.CannotAfford) == UseGUIResult.CannotAfford)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)10, woOwnerID);
			Dictionary<object, object> data = dictionary;
			NotificationController.OnNotificationReceived(NotificationType.Requirement, data);
		}
		return false;
	}

	private UseInteractorHandler GetUseInteractorHandler(int woID)
	{
		return MVGameControllerBase.WOCM.GetWorldObjectClient(woID)?.GameObject.GetComponent<UseInteractorHandler>();
	}

	public void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		UseInteractorHandler useInteractorHandler = GetUseInteractorHandler(e.instigatorWOID);
		if (useInteractorHandler != null)
		{
			useInteractorHandler.AddUseInteractor(this);
		}
	}

	public void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		UseInteractorHandler useInteractorHandler = GetUseInteractorHandler(e.instigatorWOID);
		if (useInteractorHandler != null)
		{
			useInteractorHandler.RemoveUseInteractor(this);
		}
	}

	public void UpdateData(Dictionary<object, object> data)
	{
		useInteractorVisuals.UpdateData(data, woOwnerID);
	}

	public void AddRequirement(UseRequirement useRequirement)
	{
		useInteractorVisuals.AddUseRequirement(useRequirement);
	}

	public UseGUIResult EvaluateRequirementsUsability()
	{
		return useInteractorVisuals.EvaluateUsability();
	}

	public ShowUseOption GetGUIShowOptions()
	{
		return useInteractorVisuals.GetShowOptions();
	}

	public void PayUseCost()
	{
		useInteractorVisuals.PayUseCost();
	}

	public void OnDestroy(Dictionary<object, object> data)
	{
		useInteractorVisuals.DestroyRequirementObjects(data);
	}
}
