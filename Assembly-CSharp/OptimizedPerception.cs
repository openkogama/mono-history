using System.Collections.Generic;
using UnityEngine;

public class OptimizedPerception
{
	private HashSet<int> potentialTargets = new HashSet<int>();

	private Vector3 position;

	private float radius;

	public void Update(Vector3 position, float radius)
	{
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
		if (!MVGameController.WOCM.Contains(woID))
		{
			Debug.Log("Does not contain woid " + woID);
			wo = null;
			return false;
		}
		wo = MVGameController.WOCM.GetWorldObjectClient(woID);
		if (!wo.GameObject.GetComponent<InteractionDataHandlerBase>().enabled)
		{
			return false;
		}
		return true;
	}

	private void UpdatePotentialTargets()
	{
		potentialTargets.Clear();
		Collider[] array = Physics.OverlapSphere(position, radius, 1 << LayerMask.NameToLayer("Player"));
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(collider.transform);
			if (mVObject != null)
			{
				int id = mVObject.Id;
				InteractionDataHandlerBase component = mVObject.GameObject.GetComponent<InteractionDataHandlerBase>();
				if (!(component == null) && component.enabled)
				{
					potentialTargets.Add(id);
				}
			}
		}
	}
}
