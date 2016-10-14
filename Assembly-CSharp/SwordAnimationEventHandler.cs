using UnityEngine;

public class SwordAnimationEventHandler : MonoBehaviour
{
	public PickupItemSword swordItem;

	public Transform target;

	public TrailArc trailArcPrefab;

	private TrailArc arcInstance;

	private void SwordAnimHit()
	{
		swordItem.StopOverlapCheck();
		if ((bool)arcInstance)
		{
			arcInstance.emit = false;
			arcInstance.transform.parent = null;
		}
	}

	private void SwordAnimStart()
	{
		swordItem.StartOverlapCheck();
		arcInstance = Object.Instantiate(trailArcPrefab);
		arcInstance.transform.parent = target;
		arcInstance.transform.localPosition = Vector3.zero;
		arcInstance.transform.localRotation = Quaternion.identity;
	}

	private void OnDisable()
	{
		if ((bool)arcInstance)
		{
			SwordAnimHit();
		}
	}
}
