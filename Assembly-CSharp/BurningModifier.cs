using System.Collections;
using UnityEngine;

public class BurningModifier : AvatarModifier
{
	public ParticleSystem fireParticles;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Fire;

	protected override void OnDeactivated(Avatar target)
	{
		StartCoroutine(DoFadeAndDestroy());
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
