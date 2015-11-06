using UnityEngine;

public class CameraShake : MonoBehaviour
{
	private float shakeStrength;

	private float shakeDuration;

	private Vector3 shakeOffset = Vector3.zero;

	[SerializeField]
	private float shakeMaxFactor = 1f;

	[SerializeField]
	private float shakeTimeFactor = 6.3f;

	[SerializeField]
	private float shakeStrengthFadeSpeed = 1f;

	[SerializeField]
	private AnimationCurve shakeFactorSpeedCurve;

	public Vector3 Shake(Vector3 position, float speed)
	{
		return position + Shake(speed);
	}

	private Vector3 Shake(float speed)
	{
		float num = shakeFactorSpeedCurve.Evaluate(speed) * shakeMaxFactor;
		shakeOffset = Vector3.zero;
		if (num > 0f)
		{
			float num2 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, transform.position.x);
			float num3 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, transform.position.y);
			shakeOffset += ((2f * num2 - 1f) * transform.right + (2f * num3 - 1f) * transform.up) * num;
		}
		shakeStrength = Mathf.Lerp(shakeStrength, 0f, Time.deltaTime * Time.deltaTime + shakeStrengthFadeSpeed * Time.deltaTime);
		if (shakeDuration > 0f)
		{
			float num4 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, transform.position.x);
			float num5 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, transform.position.y);
			shakeOffset += ((2f * num4 - 1f) * transform.right + (2f * num5 - 1f) * transform.up) * shakeStrength;
			shakeDuration -= Time.deltaTime;
		}
		return shakeOffset;
	}
}
