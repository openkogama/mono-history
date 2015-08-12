using System.Collections;
using UnityEngine;

public class AvatarWaterRippleEffect : MonoBehaviour
{
	private static float avatarHeight = 2f;

	public ParticleSystem avatarSplashPrefab;

	private WaterPlaneManager waterPlane;

	private Avatar avatar;

	private float previousAvatarWaterProximity;

	private bool spawningRipples;

	private Vector3 lastRipplePosition = Vector3.zero;

	private float lastRippleTime;

	private IEnumerator DoSurfaceWaterRipples(Transform avatarTfm)
	{
		spawningRipples = true;
		while (waterPlane.IsActive && avatarTfm.position.y + avatarHeight > waterPlane.transform.position.y && avatarTfm.position.y <= waterPlane.transform.position.y)
		{
			Vector3 p = avatarTfm.position;
			p.y = waterPlane.transform.position.y + 0.01f;
			if (Vector3.Distance(lastRipplePosition, p) > 1.5f || lastRippleTime + 1.2f < Time.time)
			{
				Object.Instantiate(avatarSplashPrefab, p, Quaternion.identity);
				lastRipplePosition = p;
				lastRippleTime = Time.time;
			}
			yield return null;
		}
		spawningRipples = false;
	}

	private void Start()
	{
		avatar = GetComponent<Avatar>();
		waterPlane = Object.FindObjectOfType(typeof(WaterPlaneManager)) as WaterPlaneManager;
	}

	private void Update()
	{
		if (waterPlane.IsActive)
		{
			float num = waterPlane.ComputeAvatarWaterProximity(avatar.transform.position);
			if (((previousAvatarWaterProximity <= 0f && num > 0f) || (previousAvatarWaterProximity >= 1f && num < 1f)) && !spawningRipples)
			{
				StartCoroutine(DoSurfaceWaterRipples(avatar.transform));
			}
			previousAvatarWaterProximity = num;
		}
	}
}
