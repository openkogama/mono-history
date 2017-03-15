using UnityEngine;

public abstract class MVPlaymodeCameraBase : MVCameraBase
{
	private float shakeStrength;

	private float shakeDuration;

	protected Vector3 shakeOffset = Vector3.zero;

	public float shakeMaxFactor = 1f;

	public float shakeTimeFactor = 6.3f;

	public float shakeStrengthFadeSpeed = 1f;

	public AnimationCurve shakeFactorSpeedCurve;

	public override void Exit(MVCameraController camController)
	{
		AvatarCameraFade component = camController.gameObject.GetComponent<AvatarCameraFade>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	protected void Shake(float speed)
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
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		base.UpdateCamera(camController, targetTransform);
		UpdateImpactSimulation(targetTransform);
	}
}
