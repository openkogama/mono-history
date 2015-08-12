using UnityEngine;

public static class QuaternionMath
{
	public static Quaternion Inverse(Quaternion q)
	{
		q = Conjugate(q);
		float num = MagnitudeSquared(q);
		for (int i = 0; i < 4; i++)
		{
			q[i] /= num;
		}
		return q;
	}

	public static Quaternion Conjugate(Quaternion q)
	{
		return new Quaternion(0f - q.x, 0f - q.y, 0f - q.z, q.w);
	}

	public static float MagnitudeSquared(Quaternion q)
	{
		return q[3] * q[3] + q[0] * q[0] + q[1] * q[1] + q[2] * q[2];
	}
}
