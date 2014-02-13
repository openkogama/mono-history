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

	public AvatarWaterRippleEffect()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	private IEnumerator DoSurfaceWaterRipples(Transform avatarTfm)
	{
		spawningRipples = true;
		while (waterPlane.IsActive && avatarTfm.position.y + avatarHeight > ((Component)waterPlane).transform.position.y && avatarTfm.position.y <= ((Component)waterPlane).transform.position.y)
		{
			Vector3 p = avatarTfm.position;
			p.y = ((Component)waterPlane).transform.position.y + 0.01f;
			if (Vector3.Distance(lastRipplePosition, p) > 1.5f || lastRippleTime + 1.2f < Time.time)
			{
				Object.Instantiate((Object)(object)avatarSplashPrefab, p, Quaternion.identity);
				lastRipplePosition = p;
				lastRippleTime = Time.time;
			}
			yield return null;
		}
		spawningRipples = false;
	}

	private void Start()
	{
		avatar = ((Component)this).GetComponent<Avatar>();
		waterPlane = Object.FindObjectOfType(typeof(WaterPlaneManager)) as WaterPlaneManager;
	}

	private void Update()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (waterPlane.IsActive)
		{
			float num = waterPlane.ComputeAvatarWaterProximity(((Component)avatar).transform.position);
			if (((previousAvatarWaterProximity <= 0f && num > 0f) || (previousAvatarWaterProximity >= 1f && num < 1f)) && !spawningRipples)
			{
				((MonoBehaviour)this).StartCoroutine(DoSurfaceWaterRipples(((Component)avatar).transform));
			}
			previousAvatarWaterProximity = num;
		}
	}
}
