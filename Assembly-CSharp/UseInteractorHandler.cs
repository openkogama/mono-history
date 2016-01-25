using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UseInteractorHandler : MVComponent
{
	private Dictionary<int, UseInteractor> useInteractors = new Dictionary<int, UseInteractor>();

	private List<int> removeList = new List<int>();

	private Collider triggingCollider;

	private MVInteractableBase avatarBase;

	private static readonly UseGUIResult useGui = UseGUIResult.NoUseButton | UseGUIResult.NoCost | UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	public void Init(Collider triggingCollider)
	{
		this.triggingCollider = triggingCollider;
		avatarBase = gameObject.GetComponent<MVInteractableBase>();
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
		bool flag = false;
		ShowUseOption option = ShowUseOption.Normal;
		int level = 0;
		if (useInteractors.Count > 0)
		{
			UseInteractor useInteractor = SortByDistance()[0];
			if (useInteractor.GetInteractorCanBeUsed(avatarBase))
			{
				option = useInteractor.GetGUIShowOptions();
				if ((useInteractor.EvaluateRequirementsUsability() & useGui) > UseGUIResult.NoUseButton)
				{
					flag = true;
					level = useInteractor.WoOwnerID;
				}
			}
		}
		if (flag)
		{
			MVGameControllerBase.IPlayModeUI.ShowEUseIcon(option, level);
		}
		else
		{
			MVGameControllerBase.IPlayModeUI.HideEUseIcon();
		}
	}

	private bool IsInFront(Collider triggerCollider)
	{
		Transform transform = MVGameControllerBase.CameraController.transform;
		Vector3 lhs = transform.rotation * Vector3.forward;
		Vector3 normalized = (triggerCollider.bounds.center - transform.position).normalized;
		if (Vector3.Dot(lhs, normalized) > 0f)
		{
			return true;
		}
		return false;
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
			if (item.GetInteractorCanBeUsed(avatarBase) && item.Use(worldObjectParent.Id))
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
