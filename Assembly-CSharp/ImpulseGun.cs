using System.Collections.Generic;
using UnityEngine;

public class ImpulseGun : AvatarItem
{
	public Transform muzzlePoint;

	public int ammo;

	public float hitImpulse = 700f;

	public float maxRange = 400f;

	public override void TriggerBegin(int instigatorActorNr)
	{
		if (ammo > 0)
		{
			Fire(instigatorActorNr);
			ammo--;
			if (ammo == 0)
			{
				owner.mvAvatar.Unequip(AvatarItemSlotName.Center);
			}
		}
	}

	private void Fire(int avatarId)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		LayerMask val = LayerMask.op_Implicit(1 << LayerMask.NameToLayer("Default"));
		val = LayerMask.op_Implicit(LayerMask.op_Implicit(val) + (1 << (LayerMask.NameToLayer("Player") & 0x1F)));
		if (!CollisionDetection.MVHit(ray, out var voxelHit, maxRange, new HashSet<int> { avatarId }, LayerMask.op_Implicit(val)))
		{
			return;
		}
		MVWorldObjectClient mVWorldObjectClient = MVGameController.Instance.WOCM.WorldObjects[voxelHit.woId];
		if (mVWorldObjectClient is MVAvatar)
		{
			AvatarController avatarController = (mVWorldObjectClient as MVAvatar).AvatarController;
			Vector3 direction = ray.direction;
			avatarController.ApplyImpulse(direction.normalized * hitImpulse, suspendImpactDamage: true);
			if (mVWorldObjectClient.OwnerActorNr == MVGameController.Instance.WOCM.LocalPlayerActorNumber)
			{
				MVGameController.Instance.WOCM.WeCamera.CurCamera.Shake(0.05f, 0.7f);
			}
		}
	}

	public override void TriggerEnd()
	{
	}

	private void OnGUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (owner.IsLocal)
		{
			GUI.TextField(new Rect(70f, 10f, 75f, 20f), "AMMO: " + ammo);
		}
	}
}
