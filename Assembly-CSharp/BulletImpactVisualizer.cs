using UnityEngine;

public abstract class BulletImpactVisualizer : MonoBehaviour, IBulletImpactVisualizer
{
	public abstract void VisualizeBulletImpact(VoxelHit voxelHit, Ray lineOfFire, int shooterActorNumber, float damage);
}
