using UnityEngine;

public class UIElipsoidHelper
{
	private readonly Matrix4x4 elipsoidSpaceToWorld;

	private readonly Matrix4x4 worldToElipsoidSpace;

	private readonly float width;

	private readonly float height;

	public UIElipsoidHelper(float width, float height)
	{
		this.width = width;
		this.height = height;
		elipsoidSpaceToWorld = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(width, height, 1f));
		worldToElipsoidSpace = elipsoidSpaceToWorld.inverse;
	}

	public bool IsIdentical(float width, float height)
	{
		return Mathf.Approximately(this.width, width) && Mathf.Approximately(this.height, height);
	}

	public Vector3 Clamp(Vector3 deltaDir)
	{
		Vector3 vector = worldToElipsoidSpace.MultiplyVector(deltaDir);
		vector = Vector3.ClampMagnitude(vector, 1f);
		return elipsoidSpaceToWorld.MultiplyVector(vector);
	}

	public float NormalizedDistance(Vector3 deltaDir)
	{
		return worldToElipsoidSpace.MultiplyVector(deltaDir).magnitude;
	}
}
