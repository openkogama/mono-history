using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class SizeGunBase : PickupItemWithDelay
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float range = 300f;

	[SerializeField]
	private ObscuredInt maxAmmo = 5;

	[SerializeField]
	private Color hitColor = new Color(0.2f, 0.3f, 0.9f);

	[SerializeField]
	private Color missColor = new Color(0.9f, 0.3f, 0.2f);

	private ObscuredInt currentAmmo;

	private int layerMask;

	public override int Quantity => currentAmmo;

	public override AvatarItemType Type => AvatarItemType.MouseGun;

	protected override bool IsAmmoDepleted => (int)currentAmmo <= 0;

	public override int MaxQuantity => maxAmmo;

	private void Awake()
	{
		layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
		currentAmmo = maxAmmo;
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		currentAmmo = maxAmmo;
	}

	protected override void OnFire(bool isLocal)
	{
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		bool flag = false;
		MVGameControllerBase.AudioManager.Play("Sound - SizeGunFire", audioSource, muzzlePoint.position);
		Vector3 point;
		if (CollisionDetection.MVHit(ray, out var voxelHit, range, owner.IgnoreWOIDs, layerMask))
		{
			point = voxelHit.point;
			int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
			if (owner.IsLocal && worldObjectClient != null && worldObjectClient is MVAvatar)
			{
				InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
				if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(worldObjectClient, MVGameControllerBase.Game.LocalPlayer.Avatar))
				{
					interactionDataHandlerBase.HandleInteraction(owner, GetPackageData(), interactionIsLocal: false);
					((IBulletImpactVisualizer)worldObjectClient).VisualizeBulletImpact(voxelHit, ray, owner.WorldObjectOwner.OwnerActorNr, 0f);
				}
			}
			flag = true;
		}
		else
		{
			point = ray.GetPoint(range);
		}
		RailRay railRay = PrefabPool.Instance.EnumPoolManager.Instantiate<RailRay>(PoolEnums.RailGunRay);
		railRay.transform.localPosition = muzzlePoint.position;
		railRay.target = point;
		railRay.startColor = ((!flag) ? missColor : hitColor);
		railRay.Reset();
		ReduceAmmo();
	}

	protected virtual InteractionData GetPackageData()
	{
		Debug.LogError("This function shouldn't get called. Override this in sub class");
		return MouseGunHitPackage.Create();
	}

	private void ReduceAmmo()
	{
		--currentAmmo;
		if ((int)currentAmmo <= 0)
		{
			MVEquipable component = owner.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Unequip();
			}
		}
	}
}
