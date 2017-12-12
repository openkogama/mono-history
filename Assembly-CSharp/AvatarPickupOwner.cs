using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class AvatarPickupOwner : MVPickupOwner
{
	private MVAvatar mvAvatar;

	private ILaserPointer laserPointer;

	private PickupItem laserPointerAvatarItem;

	public override HashSet<int> IgnoreWOIDs
	{
		get
		{
			HashSet<int> ignoreWOIDs = base.IgnoreWOIDs;
			if (AdditionalIgnoreWOIDS != null)
			{
				ignoreWOIDs.UnionWith(AdditionalIgnoreWOIDS);
			}
			return ignoreWOIDs;
		}
	}

	public HashSet<int> AdditionalIgnoreWOIDS { private get; set; }

	public ILaserPointer LaserPointer
	{
		get
		{
			if (currentItem != laserPointer)
			{
				MVEquipable component = GetComponent<MVEquipable>();
				if (component != null)
				{
					component.Equip(AvatarItemType.LaserPointer, AvatarEquipableType.Weapon, null);
				}
			}
			return laserPointer;
		}
	}

	public void Init(MVRuntimeDataVariable currentItemRuntimeDataVariable, MVRuntimeDataVariable isFiringRuntimeDataVariable, MVAvatar mvAvatar)
	{
		InitLaser();
		this.mvAvatar = mvAvatar;
		Init(currentItemRuntimeDataVariable, isFiringRuntimeDataVariable);
	}

	private void InitLaser()
	{
		LaserPointer component = PickupItem.InstantiateAvatarItemType(AvatarItemType.LaserPointer).GetComponent<LaserPointer>();
		component.gameObject.SetActive(value: false);
		component.owner = this;
		laserPointer = component;
		laserPointerAvatarItem = component;
	}

	protected override void Equip(AvatarItemType type, int variantId)
	{
		PickupItem pickupItem;
		if (type == AvatarItemType.LaserPointer)
		{
			pickupItem = laserPointerAvatarItem;
			pickupItem.VariantID = 0;
			SetAvatarItemAsCurrent(pickupItem);
		}
		else
		{
			pickupItem = CreateAvatarItem(type, variantId);
		}
		if (mvAvatar.Body != null)
		{
			pickupItem.transform.SetParent(mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.Torso), worldPositionStays: false);
			pickupItem.transform.localPosition = new Vector3(0f, 0.25f, 0f);
			pickupItem.transform.localRotation = Quaternion.identity;
		}
		else
		{
			pickupItem.transform.parent = gameObject.transform;
			pickupItem.transform.localPosition = new Vector3(0f, 0.6f, 0f);
			pickupItem.transform.localRotation = Quaternion.identity;
		}
		currentItem.OnEquip();
		if (onEquipItem != null)
		{
			onEquipItem(currentItem);
		}
	}

	protected override void Unequip()
	{
		if (!(currentItem == null))
		{
			currentItem.OnUnequip();
			if (currentItem.Type != AvatarItemType.LaserPointer)
			{
				Object.Destroy(currentItem.gameObject);
			}
			currentItem = null;
			if (onUnequipItem != null)
			{
				onUnequipItem(null);
			}
		}
	}

	public void HandlePointing(bool inputFire)
	{
		if (inputFire && (currentItem.Type == AvatarItemType.Hand || currentItem.IsHolstered))
		{
			((AvatarLimbManagerLocal)mvAvatar.LimbManager).StartPointing();
		}
	}
}
