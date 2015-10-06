using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MouseGun : PickupItemWithDelay
{
	private static int layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));

	public float range = 300f;

	public RailRay railGunRayPrefab;

	public AudioClip shootSound;

	private AudioSource audioSource;

	public ObscuredInt ammo = 5;

	public Color hitColor = new Color(0.2f, 0.3f, 0.9f);

	public Color missColor = new Color(0.9f, 0.3f, 0.2f);

	private int currAmmo;

	public override int Quantity => ammo;

	public override AvatarItemType Type => AvatarItemType.MouseGun;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	public void Start()
	{
		audioSource = gameObject.GetComponent<AudioSource>();
	}

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		ammo = 5;
	}

	protected override void OnFire(bool isLocal)
	{
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		bool flag = false;
		audioSource.PlayOneShot(shootSound);
		Vector3 point;
		if (CollisionDetection.MVHit(ray, out var voxelHit, range, owner.IgnoreWOIDs, layerMask))
		{
			InteractionData packageData = GetPackageData();
			point = voxelHit.point;
			int woIDHighestInHierarchyWithComponent = MVGameController.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
			if (owner.IsLocal && worldObjectClient != null && worldObjectClient is MVAvatar)
			{
				InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
				if (component != null)
				{
					component.HandleInteraction(packageData, interactionIsLocal: false);
				}
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
		ReduceAmmo();
	}

	protected virtual InteractionData GetPackageData()
	{
		return MouseGunHitPackage.Create();
	}

	private void ReduceAmmo()
	{
		--ammo;
		if ((int)ammo <= 0)
		{
			MVEquipable component = owner.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Unequip();
			}
		}
	}
}
