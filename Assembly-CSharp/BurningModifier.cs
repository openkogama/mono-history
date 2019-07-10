using System.Collections;
using UnityEngine;

public class BurningModifier : AvatarModifier
{
	public ParticleSystem fireParticles;

	private bool isDeactivating;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Fire;

	protected override void OnActivated(Avatar target)
	{
		if (!gameObject.activeInHierarchy)
		{
			fireParticles.Stop();
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
			fireParticles.Stop();
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
		fireParticles.Stop();
		while (fireParticles.particleCount > 0)
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
