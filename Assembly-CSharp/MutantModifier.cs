using System.Collections;
using UnityEngine;

public class MutantModifier : AvatarModifier
{
	public ParticleSystem fireParticles;

	private bool isDeactivating;

	private int layerMask;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Mutant;

	private void Awake()
	{
		layerMask = 1 << LayerMask.NameToLayer("Player");
	}

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
		MVAvatarLocal mVAvatarLocal = (MVAvatarLocal)owner.mvAvatar;
		float colliderRadius = mVAvatarLocal.GetColliderRadius();
		Collider[] array = Physics.OverlapSphere(owner.transform.position, colliderRadius * 2f, layerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			Avatar component = collider.GetComponent<Avatar>();
			if (!(component == owner) && !(component == null))
			{
				InteractionDataHandlerBase interactionDataHandlerBase = component.InteractionDataHandlerBase;
				if (interactionDataHandlerBase != null)
				{
					interactionDataHandlerBase.HandleInteraction(MutantHitPackage.Create(), interactionIsLocal: false);
				}
			}
		}
	}
}
