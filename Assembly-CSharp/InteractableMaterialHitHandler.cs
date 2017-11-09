using System.Collections.Generic;
using UnityEngine;

public class InteractableMaterialHitHandler
{
	private const float deadzoneDist = 1f;

	private Dictionary<AvatarModifierPackageType, ParticleSystem> particles = new Dictionary<AvatarModifierPackageType, ParticleSystem>();

	private AvatarModifierPackageType currentMoveHitParticleType;

	private ParticleSystem currentParticleSystem;

	private Vector3 prevPos = new Vector3(0f, 0f, 0f);

	public void Initialize(MaterialHitPackage[] packages, Transform parent)
	{
		for (int i = 0; i < packages.Length; i++)
		{
			ParticleSystem particleSystem = Object.Instantiate(packages[i].ParticlePrefab);
			particleSystem.transform.SetParent(parent, worldPositionStays: true);
			particles[packages[i].PackageType] = particleSystem;
			particleSystem.Stop();
		}
	}

	public void HandleHit(MVControllerColliderHit moveHit)
	{
		AvatarModifierPackageType modifierPackageType = moveHit.material.modifierPackageType;
		if (!particles.ContainsKey(modifierPackageType))
		{
			if (currentMoveHitParticleType != AvatarModifierPackageType.None)
			{
				DisableCurrentSystem();
			}
			return;
		}
		if (modifierPackageType != currentMoveHitParticleType)
		{
			SetNewCurrentParticleSystem(modifierPackageType);
			SetParticlePlacement(moveHit.hit.point, moveHit.hit.normal);
		}
		UpdateCurrentSystem(moveHit);
	}

	private void DisableCurrentSystem()
	{
		currentMoveHitParticleType = AvatarModifierPackageType.None;
		if (currentParticleSystem.isPlaying)
		{
			currentParticleSystem.Stop();
		}
	}

	private void SetNewCurrentParticleSystem(AvatarModifierPackageType newParticleType)
	{
		if (currentParticleSystem != null && currentParticleSystem.isPlaying)
		{
			currentParticleSystem.Stop(withChildren: true);
		}
		currentParticleSystem = particles[newParticleType];
		currentMoveHitParticleType = newParticleType;
	}

	private void UpdateCurrentSystem(MVControllerColliderHit moveHit)
	{
		if (!currentParticleSystem.isPlaying && Mathf.Abs(prevPos.sqrMagnitude - moveHit.hit.point.sqrMagnitude) > 1f)
		{
			prevPos = moveHit.hit.point;
			SetParticlePlacement(moveHit.hit.point, moveHit.hit.normal);
			currentParticleSystem.Play(withChildren: true);
		}
	}

	private void SetParticlePlacement(Vector3 position, Vector3 eulerRotation)
	{
		currentParticleSystem.transform.position = position;
		currentParticleSystem.transform.LookAt(position + eulerRotation);
	}
}
