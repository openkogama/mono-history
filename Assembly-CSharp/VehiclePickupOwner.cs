using MV.Common;
using UnityEngine;

public class VehiclePickupOwner : MVPickupOwner
{
	private Transform mountTransform;

	public void Init(MVRuntimeDataVariable currentItemRuntimeVariable, MVRuntimeDataVariable isFiringRuntimeVariable, Transform mountTransform)
	{
		Init(currentItemRuntimeVariable, isFiringRuntimeVariable);
		this.mountTransform = mountTransform;
	}

	public void OnLocalObjectsDestroyed()
	{
		Debug.Log((object)"OnLocalObjectsDestroyed");
		IsLocal = false;
	}

	public void OnLocalVehicleEnter()
	{
	}

	protected override void Equip(AvatarItemType type, int variantId)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		PickupItem pickupItem = CreateAvatarItem(type, variantId);
		((Component)pickupItem).transform.parent = mountTransform;
		((Component)pickupItem).transform.localPosition = Vector3.zero;
		((Component)pickupItem).transform.localRotation = Quaternion.identity;
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
			Object.Destroy((Object)(object)((Component)currentItem).gameObject);
			currentItem = null;
			if (onUnequipItem != null)
			{
				onUnequipItem(null);
			}
		}
	}
}
