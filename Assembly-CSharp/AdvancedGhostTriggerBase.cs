using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdvancedGhostTriggerBase : MonoBehaviour
{
	protected HashSet<int> attackTargets = new HashSet<int>();

	public int[] AttackTargets => attackTargets.ToArray();

	private void Reset()
	{
		attackTargets.Clear();
	}

	private void OnTriggerStay(Collider other)
	{
		if (enabled)
		{
			AddTarget(other);
		}
	}

	private void AddTarget(Collider other)
	{
		if (TryGetValidWorldObjectID(other, out var woID))
		{
			attackTargets.Add(woID);
		}
	}

	private bool TryGetValidWorldObjectID(Collider collider, out int woID)
	{
		woID = -1;
		if (collider.gameObject.layer != LayerMask.NameToLayer("Player"))
		{
			return false;
		}
		MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(collider.transform);
		InteractionDataHandlerBase interactionDataHandlerBase = mVObject.InteractionDataHandlerBase;
		if (interactionDataHandlerBase == null)
		{
			return false;
		}
		woID = mVObject.Id;
		return true;
	}

	private void LateUpdate()
	{
		Reset();
	}

	private void OnDisable()
	{
		Reset();
	}

	private void OnDestroy()
	{
		Reset();
	}
}
