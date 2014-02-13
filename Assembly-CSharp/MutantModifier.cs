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
		((MonoBehaviour)this).StartCoroutine(DoFadeAndDestroy());
	}

	private IEnumerator DoFadeAndDestroy()
	{
		fireParticles.enableEmission = false;
		fireParticles.loop = false;
		while (fireParticles.particleCount > 0)
		{
			yield return 0;
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void Update()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!owner.IsLocal || isDeactivating)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(((Component)owner).transform.position, hitRadius, 1 << LayerMask.NameToLayer("Player"));
		Collider[] array2 = array;
		foreach (Collider val in array2)
		{
			Avatar component = ((Component)val).GetComponent<Avatar>();
			if (!((Object)(object)component == (Object)(object)owner) && !((Object)(object)component == (Object)null))
			{
				InteractionDataHandlerBase component2 = ((Component)component).gameObject.GetComponent<InteractionDataHandlerBase>();
				if ((Object)(object)component2 != (Object)null)
				{
					component2.HandleInteraction(MutantHitPackage.Create(), interactionIsLocal: false);
				}
			}
		}
	}
}
