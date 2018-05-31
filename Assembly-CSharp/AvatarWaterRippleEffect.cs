using MV.Common;
using UnityEngine;

public class AvatarWaterRippleEffect : WaterSplashComponent
{
	[SerializeField]
	[Header("AirBubbles")]
	private ParticleSystem airBubbleParticlesPrefab;

	[SerializeField]
	private Vector3 airBubbleOffset = new Vector3(0f, 1.325f, 0.4f);

	[SerializeField]
	[Header("Dependencies")]
	private Avatar avatar;

	private ParticleSystem airBubbleParticles;

	private GameObject airBubbleCollitionPlane;

	private bool isInitialized;

	private float AvatarHeight => bounds.size.y;

	public override void Initialize(IMovable obj)
	{
		base.Initialize(obj);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit || MVGameControllerBase.WaterPlaneManager.IsActive)
		{
			airBubbleCollitionPlane = Object.Instantiate(new GameObject("AirBubbleCollitionPlane"));
			airBubbleCollitionPlane.transform.Rotate(new Vector3(180f, 0f, 0f));
			airBubbleParticles = Object.Instantiate(airBubbleParticlesPrefab);
			airBubbleParticles.transform.SetParent(avatar.transform);
			airBubbleParticles.transform.localPosition = airBubbleOffset;
			airBubbleParticles.collision.SetPlane(0, airBubbleCollitionPlane.transform);
			isInitialized = true;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!isInitialized)
		{
			return;
		}
		if (MVGameControllerBase.WaterPlaneManager.IsActive)
		{
			Vector3 position = avatar.transform.position;
			airBubbleCollitionPlane.transform.position = MVGameControllerBase.WaterPlaneManager.transform.position;
			float waterLevel = MVGameControllerBase.WaterPlaneManager.WaterLevel;
			if (position.y + AvatarHeight < waterLevel)
			{
				airBubbleParticles.startLifetime = (waterLevel - airBubbleParticles.transform.position.y) / airBubbleParticles.startSpeed;
				if (!airBubbleParticles.isPlaying)
				{
					airBubbleParticles.Play();
				}
			}
			else if (airBubbleParticles.isPlaying)
			{
				airBubbleParticles.Stop();
			}
		}
		else if (airBubbleParticles != null && airBubbleParticles.isPlaying)
		{
			airBubbleParticles.Stop();
		}
	}
}
