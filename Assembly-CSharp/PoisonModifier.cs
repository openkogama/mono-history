using System.Collections;
using UnityEngine;

public class PoisonModifier : AvatarModifier
{
	public ParticleSystem poisonParticles;

	private bool isDeactivating;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Poison;

	protected override void OnActivated(Avatar target)
	{
		transform.SetParent(target.mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.Head), worldPositionStays: false);
		if (!gameObject.activeInHierarchy)
		{
			poisonParticles.Stop();
		}
	}

	protected override void OnDeactivated(Avatar target)
	{
		isDeactivating = true;
		if (gameObject.activeInHierarchy)
		{
			transform.parent = null;
			StartCoroutine(DoFadeAndDestroy());
		}
		else
		{
			poisonParticles.Stop();
			Destroy();
		}
	}

	private void OnDisable()
	{
		if (isDeactivating)
		{
			Object.Destroy(gameObject);
		}
	}

	private IEnumerator DoFadeAndDestroy()
	{
		poisonParticles.Stop();
		while (poisonParticles.particleCount > 0)
		{
			yield return 0;
		}
		Destroy();
	}

	private void Destroy()
	{
		StopAllCoroutines();
		Object.Destroy(gameObject);
	}
}
