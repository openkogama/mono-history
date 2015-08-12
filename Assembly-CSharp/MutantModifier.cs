using System.Collections;
using UnityEngine;

public class MutantModifier : AvatarModifier
{
	public ParticleSystem fireParticles;

	private float hitRadius = 0.7f;

	private bool isDeactivating;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Mutant;

	protected override void OnActivated(Avatar target)
	{
		target.StartBlinking(BlinkType.Poison, float.PositiveInfinity);
		owner = target;
		fireParticles.Play();
		isDeactivating = false;
	}

	protected override void OnDeactivated(Avatar target)
	{
		target.StopBlinking(BlinkType.Poison);
		isDeactivating = true;
		StartCoroutine(DoFadeAndDestroy());
	}

	private IEnumerator DoFadeAndDestroy()
	{
		fireParticles.enableEmission = false;
		fireParticles.loop = false;
		while (fireParticles.particleCount > 0)
		{
			yield return 0;
		}
		Object.Destroy(gameObject);
	}

	private void Update()
	{
		if (!owner.IsLocal || isDeactivating)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(owner.transform.position, hitRadius, 1 << LayerMask.NameToLayer("Player"));
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			Avatar component = collider.GetComponent<Avatar>();
			if (!(component == owner) && !(component == null))
			{
				InteractionDataHandlerBase component2 = component.gameObject.GetComponent<InteractionDataHandlerBase>();
				if (component2 != null)
				{
					component2.HandleInteraction(MutantHitPackage.Create(), interactionIsLocal: false);
				}
			}
		}
	}
}
