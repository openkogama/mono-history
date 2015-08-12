using System.Collections;
using UnityEngine;

public class NinjaRunModifier : AvatarModifier
{
	public ParticleSystem runParticles;

	public TrailArc trailArcPrefab;

	private TrailArc arcInstance;

	private AudioSource soundEffect;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.NinjaRun;

	protected override void OnActivated(Avatar target)
	{
		soundEffect = GetComponent<AudioSource>();
		owner = target;
		arcInstance = Object.Instantiate(trailArcPrefab);
		arcInstance.transform.parent = target.transform;
		arcInstance.transform.localPosition = Vector3.zero + Vector3.forward * 3f;
		arcInstance.transform.localRotation = Quaternion.identity;
		arcInstance.maxPointsDrawn = 25;
		arcInstance.pointsStored = 320;
		arcInstance.faceCamera = false;
		arcInstance.twist = true;
	}

	protected override void OnDeactivated(Avatar target)
	{
		StartCoroutine(DoFadeAndDestroy());
	}

	private IEnumerator DoFadeAndDestroy()
	{
		arcInstance.emit = false;
		arcInstance.transform.parent = null;
		Object.Destroy(gameObject);
		yield return 0;
	}

	private void Update()
	{
		if (!MVInputWrapper.GetBooleanControl(KogamaControls.MoveForward))
		{
			soundEffect.volume = 0f;
			arcInstance.lifetime = 0f;
			if (arcInstance.maxPointsDrawn > 1)
			{
				arcInstance.maxPointsDrawn--;
			}
		}
		else
		{
			soundEffect.volume = 1f;
			arcInstance.lifetime = 1f;
			arcInstance.maxPointsDrawn = 25;
		}
	}
}
