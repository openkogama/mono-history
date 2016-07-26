using System.Collections;
using UnityEngine;

public class BurningModifier : AvatarModifier
{
	public ParticleSystem fireParticles;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Fire;

	protected override void OnActivated(Avatar target)
	{
		Debug.Log("Activated");
		if (!gameObject.activeInHierarchy)
		{
			fireParticles.Stop();
		}
	}

	protected override void OnDeactivated(Avatar target)
	{
		Debug.Log("Disabled");
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
