using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemHand : PickupItemWithDelay
{
	public ParticleSystem hitParticles;

	public float pushMagnitude = 500f;

	public float pushRadius = 3f;

	public override AvatarItemType Type => AvatarItemType.Hand;

	public override bool ActivateGunModeOnEquip => false;

	public override int Quantity => 0;

	protected override void OnFire(bool isLocal)
	{
	}

	private void DoRemoveCubes()
	{
		Ray ray = new Ray(muzzlePoint.position - owner.LookDirection, owner.LookDirection);
		Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2f, Color.red, 10f);
		if (CollisionDetection.MVHit(ray, out var voxelHit, 2f, new HashSet<int>(), 1 << LayerMask.NameToLayer("Default")))
		{
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(voxelHit.woId);
			if (worldObjectClient.WorldObjectType == WorldObjectType.CubeModelPrototypeTerrain || worldObjectClient.WorldObjectType == WorldObjectType.CubeModelTerrainFineGrained)
			{
				MVGameController.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, 20f);
			}
		}
	}
}
