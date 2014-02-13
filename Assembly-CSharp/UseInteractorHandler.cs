using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UseInteractorHandler : MVComponent
{
	private Dictionary<int, UseInteractor> useInteractors = new Dictionary<int, UseInteractor>();

	private Collider triggingCollider;

	public void Init(Collider triggingCollider)
	{
		this.triggingCollider = triggingCollider;
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
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, UseInteractor> useInteractor in useInteractors)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(useInteractor.Key);
			if (worldObjectClient == null || useInteractor.Value == null)
			{
				list.Add(useInteractor.Key);
				continue;
			}
			if ((Object)(object)useInteractor.Value.TrigggerCollider == (Object)null)
			{
				list.Add(useInteractor.Key);
				continue;
			}
			Collider trigggerCollider = useInteractor.Value.TrigggerCollider;
			Bounds bounds = trigggerCollider.bounds;
			if (!bounds.Intersects(triggingCollider.bounds))
			{
				Debug.Log((object)"Removing due to bounds not intersecting");
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
		if (MVGameController.Instance.IngameController != null)
		{
			if (flag)
			{
				MVGameController.Instance.IngameController.ShowEUseIcon();
			}
			else
			{
				MVGameController.Instance.IngameController.HideEUseIcon();
			}
		}
	}

	private bool IsInFront(Collider triggerCollider)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)MVGameController.Instance.Game.CameraController).transform;
		Vector3 val = transform.rotation * Vector3.forward;
		Bounds bounds = triggerCollider.bounds;
		Vector3 val2 = bounds.center - transform.position;
		Vector3 normalized = val2.normalized;
		if (Vector3.Dot(val, normalized) > 0f)
		{
			return true;
		}
		return false;
	}

	private List<UseInteractor> SortByDistance()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		List<UseInteractor> source = useInteractors.Values.ToList();
		Bounds bounds = ((Component)triggingCollider).collider.bounds;
		Vector3 triggingColliderPosition = bounds.center;
		return source.OrderBy((UseInteractor a) =>
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = ((Component)a.TrigggerCollider).transform.position - triggingColliderPosition;
			return val.sqrMagnitude;
		}).ToList();
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
