using System.Collections;
using UnityEngine;

public class FrozenModifier : AvatarModifier
{
	public ParticleSystem fireParticles;

	private bool isDeactivating;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Frozen;

	protected override void OnActivated(Avatar target)
	{
		target.StartBlinking(BlinkType.Frozen);
	}

	protected override void OnDeactivated(Avatar target)
	{
		isDeactivating = true;
		target.StopBlinking(BlinkType.Frozen);
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(DoFadeAndDestroy());
		}
		else
		{
			Object.Destroy(gameObject);
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
		fireParticles.Stop();
		while (fireParticles.particleCount > 0)
		{
			yield return 0;
		}
		Object.Destroy(gameObject);
	}
}
