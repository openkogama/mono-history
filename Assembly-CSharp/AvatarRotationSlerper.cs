using UnityEngine;

public class AvatarRotationSlerper
{
	private Transform attachPoint;

	private Quaternion startRot;

	private Transform target;

	private float duration = 0.35f;

	private float startTime;

	public AvatarRotationSlerper(Transform attachPoint, ref Transform target)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		this.attachPoint = attachPoint;
		startRot = attachPoint.rotation;
		this.target = target;
		startTime = Time.time;
	}

	public bool Update()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > startTime + duration)
		{
			return false;
		}
		float num = Mathf.Clamp01((Time.time - startTime) / duration);
		Quaternion rotation = Quaternion.Slerp(startRot, target.rotation, num);
		attachPoint.rotation = rotation;
		return true;
	}
}
