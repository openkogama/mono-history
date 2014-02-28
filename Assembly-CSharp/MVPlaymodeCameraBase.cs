using UnityEngine;

public class MVPlaymodeCameraBase : MVCameraBase
{
	private float shakeStrength;

	private float shakeDuration;

	protected Vector3 shakeOffset = Vector3.zero;

	public float shakeMaxFactor = 1f;

	public float shakeTimeFactor = 6.3f;

	public float shakeStrengthFadeSpeed = 1f;

	public AnimationCurve shakeFactorSpeedCurve;

	public MVPlaymodeCameraBase()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Enter(MVCameraController camController)
	{
		base.Enter(camController);
		camController.RequestCursorLock();
	}

	public override void Exit(MVCameraController camController)
	{
		Screen.lockCursor = false;
		AvatarCameraFade component = ((Component)camController).gameObject.GetComponent<AvatarCameraFade>();
		if ((Object)(object)component != (Object)null)
		{
			((Behaviour)component).enabled = false;
		}
	}

	protected void Shake(float speed)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		float num = shakeFactorSpeedCurve.Evaluate(speed) * shakeMaxFactor;
		shakeOffset = Vector3.zero;
		if (num > 0f)
		{
			float num2 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, ((Component)this).transform.position.x);
			float num3 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, ((Component)this).transform.position.y);
			shakeOffset += ((2f * num2 - 1f) * ((Component)this).transform.right + (2f * num3 - 1f) * ((Component)this).transform.up) * num;
		}
		shakeStrength = Mathf.Lerp(shakeStrength, 0f, Time.deltaTime * Time.deltaTime + shakeStrengthFadeSpeed * Time.deltaTime);
		if (shakeDuration > 0f)
		{
			float num4 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, ((Component)this).transform.position.x);
			float num5 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, ((Component)this).transform.position.y);
			shakeOffset += ((2f * num4 - 1f) * ((Component)this).transform.right + (2f * num5 - 1f) * ((Component)this).transform.up) * shakeStrength;
			shakeDuration -= Time.deltaTime;
		}
	}
}
