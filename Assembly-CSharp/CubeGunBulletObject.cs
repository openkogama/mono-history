using System;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class CubeGunBulletObject : MonoBehaviour
{
	[SerializeField]
	private Bullet bullet;

	[SerializeField]
	private CubeBullet cubeBullet;

	[SerializeField]
	private AudioSource audioSource;

	private byte materialID;

	public Bullet Bullet => bullet;

	public CubeBullet CubeBullet => cubeBullet;

	public static CubeGunBulletObject Create(MVPickupOwner owner, Vector3 origin, byte materialID)
	{
		CubeGunBulletObject cubeGunBulletObject = PrefabPool.Instance.EnumPoolManager.Instantiate<CubeGunBulletObject>(PoolEnums.CubeGunBullet);
		cubeGunBulletObject.materialID = materialID;
		cubeGunBulletObject.Bullet.ResetBullet();
		cubeGunBulletObject.Bullet.InitiatedPoolType = PoolEnums.CubeGunBullet;
		cubeGunBulletObject.transform.localPosition = origin;
		cubeGunBulletObject.transform.localRotation = Quaternion.identity;
		cubeGunBulletObject.CubeBullet.SetCubeMaterial(materialID);
		if (owner.IsLocal)
		{
			Bullet bullet = cubeGunBulletObject.Bullet;
			bullet.onHitLocal = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHitLocal, new Bullet.OnHitDelegate(cubeGunBulletObject.HandleCubeHitLocal));
		}
		Bullet bullet2 = cubeGunBulletObject.Bullet;
		bullet2.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet2.onHit, new Bullet.OnHitDelegate(cubeGunBulletObject.HandleCubeHit));
		cubeGunBulletObject.Bullet.PooledObjectReference = cubeGunBulletObject;
		return cubeGunBulletObject;
	}

	private void HandleCubeHitLocal(VoxelHit voxelHit, Ray lineOfFire)
	{
		IntVector cubePos = PickupItemCubeGun.GetCubePos(voxelHit);
		float toughness = MVGameControllerBase.Game.MaterialRepository.GetMaterial(materialID).physicalProperties.toughness;
		if (toughness == 0f)
		{
			MVCubeModelFineGrainedTerrain singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>();
			singletonWorldObject.AddCube(cubePos, new Cube(CubeDataPacker.CornersToByteArray(CubeBase.IdentityCorners), Cube.CreateMaterialArray(materialID)));
			singletonWorldObject.HandleDelta();
		}
		else
		{
			MVGameControllerBase.Game.World.RuntimeEventManager.SendRuntimeEvent(new SingleCubeFineGrainedEvent(cubePos, materialID));
		}
	}

	private void HandleCubeHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		MVGameControllerBase.AudioManager.Play("CubeGun", audioSource, transform.position);
	}
}
