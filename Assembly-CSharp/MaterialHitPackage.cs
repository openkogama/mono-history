using UnityEngine;

public struct MaterialHitPackage(AvatarModifierPackageType type, ParticleSystem prefab)
{
	public AvatarModifierPackageType PackageType = type;

	public ParticleSystem ParticlePrefab = prefab;
}
