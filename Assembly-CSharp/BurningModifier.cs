using System.Collections;
using UnityEngine;

public class BurningModifier : AvatarModifier
{
	public ParticleSystem fireParticles;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Fire;

	protected override void OnDeactivated(Avatar target)
	{
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(DoFadeAndDestroy());
		}
		else
		{
			Object.Destroy(gameObject);
		}
	}

	private IEnumerator DoFadeAndDestroy()
	{
		fireParticles.Stop();
		while (fireParticles.particleCount > 0)
		{
			yield return 0;
		}
		Object.Destroy(gameObject);
	}
}
