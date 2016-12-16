using UnityEngine;

public class CameraImpact
{
	public readonly Vector3 impactDirection;

	public readonly AnimationCurve impactCurve;

	public readonly float forceMultiplier;

	public readonly Space impactSpace;

	public float time;

	public CameraImpact(Vector3 impactDirection, AnimationCurve impactCurve, float forceMultiplier, Space impactSpace = Space.World)
	{
		this.impactDirection = impactDirection;
		this.impactCurve = impactCurve;
		this.forceMultiplier = forceMultiplier;
		this.impactSpace = impactSpace;
	}
}
