using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UseInteractorHandler : MVComponent
{
	private Dictionary<int, UseInteractor> useInteractors = new Dictionary<int, UseInteractor>();

	private Collider triggingCollider;

	private MVInteractableBase avatarBase;

	public void Init(Collider triggingCollider)
	{
		this.triggingCollider = triggingCollider;
		avatarBase = gameObject.GetComponent<MVInteractableBase>();
	}

	public void AddUseInteractor(UseInteractor useInteractor)
	{
		useInteractors.Add(useInteractor.WoOwnerId, useInteractor);
	}

	public void RemoveUseInteractor(UseInteractor useInteractor)
	{
		useInteractors.Remove(useInteractor.WoOwnerId);
	}

	private void UpdateInteractorsWOID()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, UseInteractor> useInteractor in useInteractors)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(useInteractor.Key);
			if (worldObjectClient == null || useInteractor.Value == null)
			{
				list.Add(useInteractor.Key);
				continue;
			}
			if (useInteractor.Value.TrigggerCollider == null)
			{
				list.Add(useInteractor.Key);
				continue;
			}
			Collider trigggerCollider = useInteractor.Value.TrigggerCollider;
			if (!trigggerCollider.bounds.Intersects(triggingCollider.bounds))
			{
				Debug.Log("Removing due to bounds not intersecting");
				list.Add(useInteractor.Key);
			}
		}
		foreach (int item in list)
		{
			useInteractors.Remove(item);
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
		if (useInteractors.Count > 0)
		{
			flag = true;
		}
		if (flag && !avatarBase.HasModifierEffect(AvatarModifierEffect.DisableVehicles))
		{
			ShowUseOption option = ShowUseOption.Normal;
			List<UseInteractor> list = SortByDistance();
			if (list.Count > 0)
			{
				MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(list[0].WoOwnerId);
				if (worldObjectClient is MVWorldObjectSpawnerVehicle && (worldObjectClient as MVWorldObjectSpawnerVehicle).GameCoinLogic.PurchaseAmount > 0)
				{
					option = ((worldObjectClient as MVWorldObjectSpawnerVehicle).GameCoinLogic.CanUse() ? ShowUseOption.GameCoinsEnough : ShowUseOption.GameCoinsInsufficient);
				}
			}
			MVGameController.PlayController.ShowEUseIcon(option);
		}
		else
		{
			MVGameController.PlayController.HideEUseIcon();
		}
	}

	private bool IsInFront(Collider triggerCollider)
	{
		Transform transform = MVGameController.Game.CameraController.transform;
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
		Vector3 triggingColliderPosition = triggingCollider.GetComponent<Collider>().bounds.center;
		return source.OrderBy((UseInteractor a) => (a.TrigggerCollider.transform.position - triggingColliderPosition).sqrMagnitude).ToList();
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
			if (item.Use(worldObjectParent.Id))
			{
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
