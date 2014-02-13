using UnityEngine;

public class RollVisualization
{
	public float rotateRollFactor = 7f;

	public float rollSpeed = 9.5f;

	public float rollMax = 30f;

	private float angleDiff;

	private Quaternion prevWorldRot = Quaternion.identity;

	public RollVisualization()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
	}

	public void AnimateHullInertia(Transform transform, Transform hullTransform)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = prevWorldRot * Vector3.forward;
		Vector3 val2 = transform.rotation * Vector3.forward;
		angleDiff = Mathf.SmoothStep(angleDiff, MathFunctions.SignedAngle(val.normalized, val2.normalized, Vector3.up) / Time.deltaTime * rotateRollFactor, Time.deltaTime * rollSpeed);
		if (Mathf.Abs(angleDiff) < 0.0001f)
		{
			angleDiff = 0f;
		}
		prevWorldRot = transform.rotation;
		hullTransform.localRotation *= Quaternion.AngleAxis(Mathf.Clamp(0f - angleDiff, 0f - rollMax, rollMax), Vector3.forward);
	}
}
