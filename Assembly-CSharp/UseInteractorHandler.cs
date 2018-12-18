using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public class UseInteractorHandler : MVComponent
{
	private Dictionary<int, UseInteractor> useInteractors = new Dictionary<int, UseInteractor>();

	private List<int> removeList = new List<int>();

	private Collider triggingCollider;

	private MVInteractableBase interactionBase;

	private const UseGUIResult useGui = UseGUIResult.NoUseButton | UseGUIResult.NoCost | UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	public void Init(Collider triggingCollider)
	{
		this.triggingCollider = triggingCollider;
		interactionBase = gameObject.GetComponent<MVInteractableBase>();
	}

	public void AddUseInteractor(UseInteractor useInteractor)
	{
		useInteractors.Add(useInteractor.WoOwnerID, useInteractor);
	}

	public void RemoveUseInteractor(UseInteractor useInteractor)
	{
		useInteractors.Remove(useInteractor.WoOwnerID);
	}

	private void UpdateInteractorsWOID()
	{
		removeList.Clear();
		foreach (KeyValuePair<int, UseInteractor> useInteractor in useInteractors)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(useInteractor.Key);
			if (worldObjectClient == null || useInteractor.Value == null)
			{
				removeList.Add(useInteractor.Key);
				continue;
			}
			if (useInteractor.Value.TriggerCollider == null)
			{
				removeList.Add(useInteractor.Key);
				continue;
			}
			Collider triggerCollider = useInteractor.Value.TriggerCollider;
			if (!triggerCollider.bounds.Intersects(triggingCollider.bounds))
			{
				removeList.Add(useInteractor.Key);
			}
		}
		foreach (int remove in removeList)
		{
			useInteractors.Remove(remove);
		}
	}

	private void Update()
	{
		UpdateInteractorsWOID();
		UpdateUseVisuals();
	}

	private void UpdateUseVisuals()
	{
		if (!MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Playing))
		{
			return;
		}
		bool flag = false;
		ShowUseOption option = ShowUseOption.Normal;
		int woId = 0;
		if (useInteractors.Count > 0)
		{
			UseInteractor useInteractor = SortByDistance()[0];
			if (useInteractor.GetInteractorCanBeUsed(interactionBase))
			{
				option = useInteractor.GetGUIShowOptions();
				if ((useInteractor.EvaluateRequirementsUsability() & (UseGUIResult.NoUseButton | UseGUIResult.NoCost | UseGUIResult.CanAfford | UseGUIResult.CannotAfford)) > UseGUIResult.NoUseButton)
				{
					flag = true;
					woId = useInteractor.WoOwnerID;
				}
			}
		}
		if (flag)
		{
			MVGameControllerBase.PlayModeUI.ShowEUseIcon(option, woId);
		}
		else
		{
			MVGameControllerBase.PlayModeUI.HideEUseIcon();
		}
	}

	private List<UseInteractor> SortByDistance()
	{
		List<UseInteractor> source = useInteractors.Values.ToList();
		Vector3 triggingColliderPosition = triggingCollider.bounds.center;
		return source.OrderBy((UseInteractor a) => (a.TriggerCollider.transform.position - triggingColliderPosition).magnitude).ToList();
	}

	public bool Use()
	{
		UpdateInteractorsWOID();
		if (useInteractors.Count == 0)
		{
			return false;
		}
		UseInteractor useInteractor = null;
		List<UseInteractor> list = SortByDistance();
		foreach (UseInteractor item in list)
		{
			if (item.GetInteractorCanBeUsed(interactionBase) && item.Use(worldObjectParent.Id))
			{
				item.PayUseCost();
				useInteractor = item;
				break;
			}
		}
		if (useInteractor != null)
		{
			if (useInteractor.Reset)
			{
				Reset();
			}
			return true;
		}
		return false;
	}

	public void Reset()
	{
		useInteractors.Clear();
	}
}
