using UnityEngine;

public class AvatarWaterRippleEffect : MonoBehaviour
{
	private static float avatarHeight = 2f;

	public ParticleSystem avatarSplashPrefab;

	private WaterPlaneManager waterPlane;

	private Avatar avatar;

	private ParticleSystem ripple;

	private float previousAvatarWaterProximity;

	private float lastRippleTime;

	private Vector3 lastRipplePosition;

	public Avatar Avatar
	{
		get
		{
			if (avatar == null)
			{
				avatar = GetComponent<Avatar>();
			}
			return avatar;
		}
	}

	private void Start()
	{
		waterPlane = Object.FindObjectOfType(typeof(WaterPlaneManager)) as WaterPlaneManager;
		ripple = Object.Instantiate(avatarSplashPrefab, Vector3.zero, Quaternion.identity) as ParticleSystem;
	}

	private void Update()
	{
		if (!waterPlane.IsActive)
		{
			return;
		}
		Vector3 position = Avatar.transform.position;
		Vector3 position2 = waterPlane.transform.position;
		lastRippleTime += Time.deltaTime;
		float num = waterPlane.ComputeAvatarWaterProximity(position);
		if ((previousAvatarWaterProximity <= 0f && num > 0f) || (previousAvatarWaterProximity >= 1f && num < 1f))
		{
			previousAvatarWaterProximity = num;
		}
		if (position.y + avatarHeight > position2.y && position.y <= position2.y)
		{
			position.y = position2.y;
			if (lastRippleTime > 1.2f || Vector3.Distance(lastRipplePosition, position) > 1.5f)
			{
				ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
				{
					position = position,
					velocity = default,
					startSize = ripple.startSize,
					startLifetime = ripple.startLifetime,
					startColor = Color.white
				};
				ripple.Emit(emitParams, 1);
				lastRipplePosition = position;
				lastRippleTime = 0f;
			}
		}
	}
}
