using System.Collections.Generic;
using UnityEngine;

public class OptimizedPerception
{
	private Vector3 position;

	private float radius;

	private HashSet<int> potentialTargets = new HashSet<int>();

	private HashSet<int> removeSet = new HashSet<int>();

	private List<WorldObjectClientRef> targets = new List<WorldObjectClientRef>(16);

	public void Update(Vector3 position, float radius)
	{
		this.position = position;
		this.radius = radius;
		UpdatePotentialTargets();
	}

	public bool TryGetTarget(int woID, out MVWorldObjectClient wo)
	{
		wo = null;
		if (!potentialTargets.Contains(woID))
		{
			return false;
		}
		if (!GetValidTarget(woID, out var wo2))
		{
			potentialTargets.Remove(woID);
			return false;
		}
		wo = wo2.WorldObjectClient;
		return true;
	}

	public List<WorldObjectClientRef> GetTargets()
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

	private bool GetValidTarget(int woID, out WorldObjectClientRef wo)
	{
		if (!MVGameControllerBase.WOCM.Contains(woID))
		{
			Debug.Log("Does not contain woid " + woID);
			wo = null;
			return false;
		}
		wo = MVGameControllerBase.WOCM.GetWorldObjectClientRef(woID);
		if (wo == null || wo.WorldObjectClient == null)
		{
			return false;
		}
		if (wo.WorldObjectClient.InteractionDataHandlerBase == null || !wo.WorldObjectClient.InteractionDataHandlerBase.enabled)
		{
			return false;
		}
		return true;
	}

	private void UpdatePotentialTargets()
	{
		potentialTargets.Clear();
		int num = Physics.OverlapSphereNonAlloc(position, radius, CollisionDetectionGlobalBuffers.colliderBuffer, 1 << LayerMask.NameToLayer("Player"));
		for (int i = 0; i < num; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(CollisionDetectionGlobalBuffers.colliderBuffer[i].transform);
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
