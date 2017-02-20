using System.Collections;
using UnityEngine;

public class PoisonModifier : AvatarModifier
{
	public ParticleSystem poisonParticles;

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
