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
		this.attachPoint = attachPoint;
		startRot = attachPoint.rotation;
		this.target = target;
		startTime = Time.time;
	}

	public bool Update()
	{
		if (Time.time > startTime + duration)
		{
			return false;
		}
		float t = Mathf.Clamp01((Time.time - startTime) / duration);
		Quaternion rotation = Quaternion.Slerp(startRot, target.rotation, t);
		attachPoint.rotation = rotation;
		return true;
	}
}
