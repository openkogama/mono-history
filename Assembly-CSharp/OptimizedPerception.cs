using System.Collections.Generic;
using UnityEngine;

public class OptimizedPerception
{
	private Vector3 position;

	private float radius;

	private HashSet<int> potentialTargets = new HashSet<int>();

	private HashSet<int> removeSet = new HashSet<int>();

	private List<MVWorldObjectClient> targets = new List<MVWorldObjectClient>(16);

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
		removeSet.Clear();
		targets.Clear();
		foreach (int potentialTarget in potentialTargets)
		{
			if (!GetValidTarget(potentialTarget, out var wo))
			{
				removeSet.Add(potentialTarget);
			}
			else
			{
				targets.Add(wo);
			}
		}
		foreach (int item in removeSet)
		{
			potentialTargets.Remove(item);
		}
		return targets;
	}

	private bool GetValidTarget(int woID, out MVWorldObjectClient wo)
	{
		if (!MVGameControllerBase.WOCM.Contains(woID))
		{
			Debug.Log("Does not contain woid " + woID);
			wo = null;
			return false;
		}
		wo = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		if (!wo.InteractionDataHandlerBase.enabled)
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
				InteractionDataHandlerBase interactionDataHandlerBase = mVObject.InteractionDataHandlerBase;
				if (!(interactionDataHandlerBase == null) && interactionDataHandlerBase.enabled)
				{
					potentialTargets.Add(id);
				}
			}
		}
	}
}
