using UnityEngine;

public class VehicleBaseObject : ObjectPrefab
{
	[SerializeField]
	protected BulletImpactVisualizer bulletImpactVisualizer;

	public IBulletImpactVisualizer BulletImpactVisualizer => bulletImpactVisualizer;
}
