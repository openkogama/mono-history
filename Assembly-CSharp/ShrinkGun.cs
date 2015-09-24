using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class ShrinkGun : PickupItemWithDelay
{
	public float range = 300f;

	public RailRay railGunRayPrefab;

	public AudioClip hitSound;

	public AudioClip missSound;

	public ObscuredInt ammo;

	public Color hitColor = new Color(0.2f, 0.3f, 0.9f);

	public Color missColor = new Color(0.9f, 0.3f, 0.2f);

	private int currAmmo;

	public override int Quantity => ammo;

	public override AvatarItemType Type => AvatarItemType.ShrinkGun;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		ammo = 13;
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		Debug.Log("TriggerBegin");
		--ammo;
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		bool flag = false;
		int layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
		Vector3 point;
		if (CollisionDetection.MVHit(ray, out var voxelHit, range, owner.IgnoreWOIDs, layerMask))
		{
			InteractionData interaction = ShrinkGunHitPackage.Create();
			Debug.Log("Created shrinkgunhitpackage");
			MVGameController.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, interaction.Damage);
			point = voxelHit.point;
			int woIDHighestInHierarchyWithComponent = MVGameController.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
			if (owner.IsLocal && worldObjectClient != null)
			{
				InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
				if (component != null)
				{
					component.HandleInteraction(interaction, interactionIsLocal: false);
				}
				Debug.Log("handling shrink interaction package");
			}
			flag = true;
		}
		else
		{
			point = ray.GetPoint(range);
		}
		RailRay railRay = Object.Instantiate(railGunRayPrefab, muzzlePoint.position, Quaternion.identity) as RailRay;
		railRay.target = point;
		railRay.startColor = ((!flag) ? missColor : hitColor);
		if ((int)ammo <= 0)
		{
			MVEquipable component2 = owner.GetComponent<MVEquipable>();
			if (component2 != null)
			{
				component2.Unequip();
			}
		}
	}
}
