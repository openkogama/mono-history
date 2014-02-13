using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class AvatarPickupOwner : MVPickupOwner
{
	private MVBody body;

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
				MVEquipable component = ((Component)this).GetComponent<MVEquipable>();
				if ((Object)(object)component != (Object)null)
				{
					component.Equip(AvatarItemType.LaserPointer, null);
				}
			}
			return laserPointer;
		}
	}

	public void Init(MVRuntimeDataVariable currentItemRuntimeDataVariable, MVRuntimeDataVariable isFiringRuntimeDataVariable, MVAvatar mvAvatar, MVBody body)
	{
		InitLaser();
		Init(currentItemRuntimeDataVariable, isFiringRuntimeDataVariable);
		this.body = body;
		mvAvatar.ScaleChanged += OnAvatarScaleChanged;
	}

	private void InitLaser()
	{
		LaserPointer laserPointer = Resources.Load(PickupItem.GetPrefabNameForAvatarItemType(AvatarItemType.LaserPointer), typeof(LaserPointer)) as LaserPointer;
		LaserPointer laserPointer2 = Object.Instantiate((Object)(object)laserPointer) as LaserPointer;
		((Component)laserPointer2).gameObject.SetActiveRecursively(false);
		laserPointer2.owner = this;
		this.laserPointer = laserPointer2;
		laserPointerAvatarItem = laserPointer2;
	}

	protected override void Equip(AvatarItemType type, int variantId)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
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
		if (body != null)
		{
			((Component)pickupItem).transform.parent = body.BodyData.GetPartBone("Torso");
			((Component)pickupItem).transform.localPosition = new Vector3(0f, 0.25f, 0f);
			((Component)pickupItem).transform.localRotation = Quaternion.identity;
		}
		else
		{
			((Component)pickupItem).transform.parent = ((Component)this).gameObject.transform;
			((Component)pickupItem).transform.localPosition = new Vector3(0f, 0.6f, 0f);
			((Component)pickupItem).transform.localRotation = Quaternion.identity;
		}
		if (onEquipItem != null)
		{
			onEquipItem(currentItem);
		}
	}

	protected override void Unequip()
	{
		if (!((Object)(object)currentItem == (Object)null))
		{
			currentItem.OnUnequip();
			if (currentItem.Type != AvatarItemType.LaserPointer)
			{
				Object.Destroy((Object)(object)((Component)currentItem).gameObject);
			}
			currentItem = null;
			if (onUnequipItem != null)
			{
				onUnequipItem(null);
			}
		}
	}

	private void OnAvatarScaleChanged(object sender, ScaleChangedEventArgs e)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)currentItem != (Object)null)
		{
			((Component)currentItem).gameObject.transform.localScale = worldObjectParent.Scale;
		}
	}
}
