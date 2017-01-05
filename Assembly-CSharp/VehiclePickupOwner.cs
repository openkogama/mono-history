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
		Debug.Log("OnLocalObjectsDestroyed");
		IsLocal = false;
	}

	protected override void Equip(AvatarItemType type, int variantId)
	{
		PickupItem pickupItem = CreateAvatarItem(type, variantId);
		pickupItem.transform.parent = mountTransform;
		pickupItem.transform.localPosition = Vector3.zero;
		pickupItem.transform.localRotation = Quaternion.identity;
		if (onEquipItem != null)
		{
			onEquipItem(currentItem);
		}
	}

	protected override void Unequip()
	{
		if (!(currentItem == null) && !currentItem.IsHolstered)
		{
			currentItem.OnUnequip();
			Object.Destroy(currentItem.gameObject);
			currentItem = null;
			if (onUnequipItem != null)
			{
				onUnequipItem(null);
			}
		}
	}
}
