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
		arcInstance.emit = false;
		((Component)arcInstance).transform.parent = null;
	}

	private void SwordAnimStart()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		swordItem.StartOverlapCheck();
		arcInstance = Object.Instantiate((Object)(object)trailArcPrefab) as TrailArc;
		((Component)arcInstance).transform.parent = target;
		((Component)arcInstance).transform.localPosition = Vector3.zero;
		((Component)arcInstance).transform.localRotation = Quaternion.identity;
	}

	private void OnDisable()
	{
		if (Object.op_Implicit((Object)(object)arcInstance))
		{
			SwordAnimHit();
		}
	}
}
