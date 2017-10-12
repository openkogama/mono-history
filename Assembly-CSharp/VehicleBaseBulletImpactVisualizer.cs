using UnityEngine;

public class VehicleBaseBulletImpactVisualizer : BulletImpactVisualizer
{
	[Range(0f, 10f)]
	[SerializeField]
	private float particlesPerPointOfDamage = 1f;

	public override void VisualizeBulletImpact(VoxelHit voxelHit, Ray lineOfFire, int shooterActorNumber, float damage = 100f)
	{
		ParticleSystem particleSystem = OneShotPooledParticleSystem.Instantiate(PoolEnums.VehicleBulletImpact, voxelHit.point, Quaternion.LookRotation(-lineOfFire.direction));
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		ParticleSystem.Burst[] array = new ParticleSystem.Burst[1];
		array[0].time = 0f;
		array[0].minCount = (short)(damage * particlesPerPointOfDamage);
		array[0].maxCount = (short)(damage * particlesPerPointOfDamage);
		emission.SetBursts(array);
		particleSystem.Play();
	}
}
