using System.Collections.Generic;
using MV.WorldObject;
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

	public List<WorldObjectClientRef> GetTargets(MVTeam alliedTeam)
	{
		removeSet.Clear();
		targets.Clear();
		foreach (int potentialTarget in potentialTargets)
		{
			if (!GetValidTarget(potentialTarget, alliedTeam, out var wo))
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

	private bool GetValidTarget(int woID, MVTeam alliedTeam, out WorldObjectClientRef wo)
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
		bool flag = MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(wo.WorldObjectClient.OwnerActorNr) != alliedTeam;
		bool flag2 = MVGameControllerBase.Game.TeamManager.TeamCount() <= 1;
		bool flag3 = flag || flag2;
		return wo.WorldObjectClient.InteractionDataHandlerBase != null && wo.WorldObjectClient.InteractionDataHandlerBase.enabled && flag3;
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
				if (interactionDataHandlerBase != null && interactionDataHandlerBase.enabled)
				{
					potentialTargets.Add(id);
				}
			}
		}
	}
}
