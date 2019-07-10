using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class AvatarPickupOwner : MVPickupOwner
{
	private MVAvatar mvAvatar;

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

	public void Init(MVRuntimeDataVariable currentItemRuntimeDataVariable, MVRuntimeDataVariable isFiringRuntimeDataVariable, MVAvatar mvAvatar)
	{
		this.mvAvatar = mvAvatar;
		Init(currentItemRuntimeDataVariable, isFiringRuntimeDataVariable);
	}

	protected override void Equip(AvatarItemType type, int variantId)
	{
		PickupItem pickupItem = null;
		pickupItem = CreateAvatarItem(type, variantId);
		if (pickupItem == null)
		{
			Debug.LogError("AvatarItem is null. This is thought to be cheaters manipulating equip data. Equipping Hand.");
			pickupItem = CreateAvatarItem(AvatarItemType.Hand, variantId);
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
		SetAvatarItemAsCurrent(pickupItem);
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
