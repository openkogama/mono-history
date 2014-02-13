using System.Collections.Generic;
using UnityEngine;

public class OptimizedPerception
{
	private HashSet<int> potentialTargets = new HashSet<int>();

	private Vector3 position;

	private float radius;

	public void Update(Vector3 position, float radius)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		this.position = position;
		this.radius = radius;
		UpdatePotentialTargets();
	}

	public bool TryGetTarget(int woID, out MVWorldObjectClient wo)
	{
		if (!potentialTargets.Contains(woID))
		{
			wo = null;
			return false;
		}
		if (!GetValidTarget(woID, out wo))
		{
			potentialTargets.Remove(woID);
			return false;
		}
		return true;
	}

	public List<MVWorldObjectClient> GetTargets()
	{
		HashSet<int> hashSet = new HashSet<int>();
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		foreach (int potentialTarget in potentialTargets)
		{
			if (!GetValidTarget(potentialTarget, out var wo))
			{
				hashSet.Add(potentialTarget);
			}
			else
			{
				list.Add(wo);
			}
		}
		foreach (int item in hashSet)
		{
			potentialTargets.Remove(item);
		}
		return list;
	}

	private bool GetValidTarget(int woID, out MVWorldObjectClient wo)
	{
		if (!MVGameController.Instance.WOCM.Contains(woID))
		{
			Debug.Log((object)("Does not contain woid " + woID));
			wo = null;
			return false;
		}
		wo = MVGameController.Instance.WOCM.GetWorldObjectClient(woID);
		return true;
	}

	private void UpdatePotentialTargets()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		potentialTargets.Clear();
		Collider[] array = Physics.OverlapSphere(position, radius, 1 << LayerMask.NameToLayer("Player"));
		Collider[] array2 = array;
		foreach (Collider val in array2)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(((Component)val).transform);
			if (mVObject != null)
			{
				int id = mVObject.Id;
				InteractionDataHandlerBase component = mVObject.GameObject.GetComponent<InteractionDataHandlerBase>();
				if (!((Object)(object)component == (Object)null))
				{
					potentialTargets.Add(id);
				}
			}
		}
	}
}
