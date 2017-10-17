using UnityEngine;

public class AvatarWaterRippleEffect : MonoBehaviour
{
	private const float avatarHeight = 2f;

	[SerializeField]
	[Header("Rings")]
	private ParticleSystem waterRingParticlesPrefab;

	[SerializeField]
	[Header("Splash")]
	private ParticleSystem waterSplashParticlesPrefab;

	[Tooltip("Actual number is based off avatar speed.")]
	[Range(0f, 8f)]
	[SerializeField]
	private float baseNumberOfSplashParticles = 1f;

	[SerializeField]
	[Tooltip("Actual number is based off avatar speed.")]
	[Range(0f, 4f)]
	private float baseSplashParticlesSpeed = 0.5f;

	[SerializeField]
	private Color splashTint;

	[Header("Pillar")]
	[SerializeField]
	private ParticleSystem waterPillarParticlesPrefab;

	[Range(0f, 10f)]
	[SerializeField]
	private float waterPillarDensity = 0.5f;

	[SerializeField]
	private Color pillarTint;

	[Header("AirBubbles")]
	[SerializeField]
	private ParticleSystem airBubbleParticlesPrefab;

	[SerializeField]
	private Vector3 airBubbleOffset;

	[Range(0f, 1f)]
	[SerializeField]
	[Header("Sound")]
	private float splashSoundVolume = 0.5f;

	[Header("Dependencies")]
	[SerializeField]
	private Avatar avatar;

	private AudioClip splashSound;

	private Vector3 lastParticlePosition;

	private Color baseWaterColor;

	private ParticleSystem waterRingParticles;

	private ParticleSystem waterSplashParticles;

	private ParticleSystem waterPillarParticles;

	private ParticleSystem airBubbleParticles;

	private GameObject airBubbleCollitionPlane;

	private float lastRingEmissionTime;

	private bool avatarHasEnteredWater;

	public void SetSplashSound(AudioClip a)
	{
		splashSound = a;
	}

	private void Start()
	{
		waterRingParticles = Object.Instantiate(waterRingParticlesPrefab);
		waterSplashParticles = Object.Instantiate(waterSplashParticlesPrefab);
		waterPillarParticles = Object.Instantiate(waterPillarParticlesPrefab);
		airBubbleParticles = Object.Instantiate(airBubbleParticlesPrefab);
		airBubbleParticles.transform.SetParent(avatar.transform);
		airBubbleParticles.transform.localPosition = airBubbleOffset;
		airBubbleCollitionPlane = Object.Instantiate(new GameObject("AirBubbleCollitionPlane"));
		airBubbleCollitionPlane.transform.Rotate(new Vector3(180f, 0f, 0f));
		airBubbleParticles.collision.SetPlane(0, airBubbleCollitionPlane.transform);
	}

	private void Update()
	{
		if (!MVGameControllerBase.WaterPlaneManager.IsActive)
		{
			return;
		}
		lastRingEmissionTime += Time.deltaTime;
		airBubbleCollitionPlane.transform.position = MVGameControllerBase.WaterPlaneManager.transform.position;
		Vector3 position = avatar.transform.position;
		float waterElevation = MVGameControllerBase.WaterPlaneManager.WaterElevation;
		bool flag = position.y + 2f > waterElevation && position.y <= waterElevation;
		if (flag)
		{
			position.y = waterElevation;
			if (lastRingEmissionTime > 1.2f || Vector3.Distance(lastParticlePosition, position) > 1.5f)
			{
				EmitWaterRing(position);
			}
		}
		bool flag2 = position.y + 2f < waterElevation;
		if (flag2)
		{
			airBubbleParticles.startLifetime = (waterElevation - airBubbleParticles.transform.position.y) / airBubbleParticles.startSpeed;
			if (!airBubbleParticles.isPlaying)
			{
				airBubbleParticles.Play();
			}
		}
		else if (airBubbleParticles.isPlaying)
		{
			airBubbleParticles.Stop();
		}
		if (flag && !avatarHasEnteredWater)
		{
			EmitWaterSplash(position, avatar.mvAvatar.Velocity);
			EmitWaterPillar(position, avatar.mvAvatar.Velocity);
			MVGameControllerBase.AudioManager.Play("AvatarWaterSplashSound", splashSound, position, splashSoundVolume, SoundRangeDistance.Long);
		}
		avatarHasEnteredWater = flag || flag2;
	}

	private void EmitWaterRing(Vector3 position)
	{
		waterRingParticles.transform.position = position;
		waterRingParticles.Emit(1);
		lastParticlePosition = position;
		lastRingEmissionTime = 0f;
	}

	private void EmitWaterSplash(Vector3 position, Vector3 velocity)
	{
		waterSplashParticles.transform.position = position;
		waterSplashParticles.startSpeed = velocity.magnitude * baseSplashParticlesSpeed;
		Color waterColor = MVGameControllerBase.WaterPlaneManager.WaterColor;
		waterSplashParticles.startColor = (waterColor + splashTint) / 2f;
		waterSplashParticles.Emit((int)(velocity.magnitude * baseNumberOfSplashParticles));
	}

	private void EmitWaterPillar(Vector3 position, Vector3 velocity)
	{
		waterPillarParticles.transform.position = position;
		float magnitude = velocity.magnitude;
		float num = waterPillarDensity * magnitude;
		for (int i = 1; (float)i < num; i++)
		{
			waterPillarParticles.startSpeed = num - (float)i;
			Color waterColor = MVGameControllerBase.WaterPlaneManager.WaterColor;
			waterPillarParticles.startColor = (waterColor + pillarTint) / 2f;
			waterPillarParticles.Emit(1);
		}
	}
}
