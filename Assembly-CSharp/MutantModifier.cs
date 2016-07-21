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
		ParticleSystem.EmissionModule em = fireParticles.emission;
		em.enabled = false;
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
		int num = Physics.OverlapSphereNonAlloc(owner.transform.position, colliderRadius * 2f, CollisionDetectionGlobalBuffers.colliderBuffer, layerMask);
		for (int i = 0; i < num; i++)
		{
			Avatar component = CollisionDetectionGlobalBuffers.colliderBuffer[i].GetComponent<Avatar>();
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
