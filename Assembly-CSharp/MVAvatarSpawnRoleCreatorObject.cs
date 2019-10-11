using UnityEngine;

public class MVAvatarSpawnRoleCreatorObject : ObjectPrefab
{
	[SerializeField]
	private TintObject spawnPlate;

	[SerializeField]
	public GameObject useInteractionRotator;

	public TintObject SpawnPlate => spawnPlate;
}
