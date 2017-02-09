using UnityEngine;

public interface IBulletImpactVisualizer
{
	void VisualizeBulletImpact(VoxelHit voxelHit, Ray lineOfFire, int shooterActorNumber, float damage = 100f);
}
