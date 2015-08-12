using UnityEngine;

public class RollVisualization
{
	public float rotateRollFactor = 7f;

	public float rollSpeed = 9.5f;

	public float rollMax = 30f;

	private float angleDiff;

	private Quaternion prevWorldRot = Quaternion.identity;

	public void AnimateHullInertia(Transform transform, Transform hullTransform)
	{
		Vector3 vector = prevWorldRot * Vector3.forward;
		Vector3 vector2 = transform.rotation * Vector3.forward;
		angleDiff = Mathf.SmoothStep(angleDiff, MathFunctions.SignedAngle(vector.normalized, vector2.normalized, Vector3.up) / Time.deltaTime * rotateRollFactor, Time.deltaTime * rollSpeed);
		if (Mathf.Abs(angleDiff) < 0.0001f)
		{
			angleDiff = 0f;
		}
		prevWorldRot = transform.rotation;
		hullTransform.localRotation *= Quaternion.AngleAxis(Mathf.Clamp(0f - angleDiff, 0f - rollMax, rollMax), Vector3.forward);
	}
}
