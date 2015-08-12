using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public abstract class MVPickupOwner : MVComponent
{
	public delegate void OnEquipItemDelegate(PickupItem equippedItem);

	public delegate void OnUnequipItemDelegate(PickupItem unequippedItem);

	public delegate void OnHandleFiringDelegate(bool isFiring);

	private const float _updateLineOfFireInterval = 0.5f;

	public OnEquipItemDelegate onEquipItem;

	public OnUnequipItemDelegate onUnequipItem;

	public OnHandleFiringDelegate onHandleFiring;

	protected PickupItem currentItem;

	private Vector3 lookOrigin = Vector3.one;

	private Vector3 lookDirection = Vector3.one;

	private float prevUpdateLineOfFireTime = Time.time;

	public PickupItem CurrentItem => currentItem;

	public bool IsLocal { get; set; }

	public Vector3 LookOrigin => lookOrigin;

	public Vector3 LookDirection => lookDirection.normalized;

	public bool InGunMode
	{
		get
		{
			if (CurrentItem == null)
			{
				return false;
			}
			return CurrentItem.ActivateGunModeOnEquip;
		}
	}

	public MVWorldObjectClient WorldObjectOwner => worldObjectParent;

	public virtual HashSet<int> IgnoreWOIDs => worldObjectParent.WorldIDsRecursive;

	public float GetAbsolutProjectileSpeed(float projectileSpeed)
	{
		float magnitude = (lookDirection - lookDirection.normalized).magnitude;
		return projectileSpeed + magnitude;
	}

	private Vector3 GetLookDirectionWithAddedVelocityMagnitude(Vector3 lookDirection)
	{
		MVRigidBody enabledMonoBehaviourHighestInHierarchy = MVWorldObjectClientManager.GetEnabledMonoBehaviourHighestInHierarchy<MVRigidBody>(gameObject);
		if (enabledMonoBehaviourHighestInHierarchy == null)
		{
			return lookDirection;
		}
		float num = Vector3.Dot(enabledMonoBehaviourHighestInHierarchy.Velocity.normalized, LookDirection.normalized);
		if (num <= 0f)
		{
			return lookDirection;
		}
		return enabledMonoBehaviourHighestInHierarchy.Velocity.magnitude * num * lookDirection + lookDirection;
	}

	protected abstract void Equip(AvatarItemType type, int variantId);

	protected abstract void Unequip();

	protected void Init(MVRuntimeDataVariable currentItemRuntimeVariable, MVRuntimeDataVariable isFiringRuntimeVariable)
	{
		currentItemRuntimeVariable.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(currentItemRuntimeVariable.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object item) =>
		{
			UpdateCurrentItem((Dictionary<object, object>)item);
		}));
		UpdateCurrentItem((Dictionary<object, object>)currentItemRuntimeVariable.Value);
		isFiringRuntimeVariable.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isFiringRuntimeVariable.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object value) =>
		{
			HandleFiring((bool)value);
		}));
	}

	public void HandleFire(bool inputFire, MVRuntimeDataVariable isFiringRuntimeVariable)
	{
		SetLineOfFireLocal();
		bool flag = (bool)CurrentItem && CurrentItem.CanFire() && inputFire;
		if ((bool)isFiringRuntimeVariable.Value != flag)
		{
			isFiringRuntimeVariable.Value = flag;
		}
		if (flag && Time.time - prevUpdateLineOfFireTime > 0.5f)
		{
			MVGameController.Game.UpdateLineOfFire(worldObjectParent.Id, lookDirection, lookOrigin);
			prevUpdateLineOfFireTime = Time.time;
		}
	}

	private void SetLineOfFireLocal()
	{
		MVCameraBase curCamera = MVGameController.Game.CameraController.CurCamera;
		Vector3 fireDirection = curCamera.FireDirection;
		Vector3 fireOrigin = curCamera.FireOrigin;
		fireDirection = GetLookDirectionWithAddedVelocityMagnitude(fireDirection);
		SetLineOfFire(fireOrigin, fireDirection);
	}

	public void SetLineOfFire(Vector3 lookOrigin, Vector3 lookDirection)
	{
		this.lookOrigin = lookOrigin;
		this.lookDirection = lookDirection;
	}

	private void UpdateCurrentItem(Dictionary<object, object> newState)
	{
		if (!newState.ContainsKey("type"))
		{
			Unequip();
			return;
		}
		AvatarItemType avatarItemType = (AvatarItemType)(int)newState["type"];
		int num = (newState.ContainsKey("variantId") ? ((int)newState["variantId"]) : 0);
		if (currentItem == null || avatarItemType != currentItem.Type || num != currentItem.VariantID)
		{
			Equip(avatarItemType, num);
		}
		currentItem.OnStateChanged(newState);
	}

	private void HandleFiring(bool isFiring)
	{
		if (onHandleFiring != null)
		{
			onHandleFiring(isFiring);
		}
		if (currentItem != null)
		{
			if (isFiring)
			{
				currentItem.TriggerBegin(worldObjectParent.Id);
			}
			else
			{
				currentItem.TriggerEnd();
			}
		}
	}

	protected void SetAvatarItemAsCurrent(PickupItem avatarItem)
	{
		avatarItem.owner = this;
		if (currentItem != avatarItem)
		{
			Unequip();
		}
		currentItem = avatarItem;
		currentItem.OnEquip();
	}

	protected PickupItem CreateAvatarItem(AvatarItemType type, int variantId)
	{
		string prefabNameForAvatarItemType = PickupItem.GetPrefabNameForAvatarItemType(type);
		if (prefabNameForAvatarItemType.Length == 0)
		{
			Debug.LogWarning("Trying to equip an un-implemented item type!");
			return null;
		}
		prefabNameForAvatarItemType += variantId;
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(prefabNameForAvatarItemType));
		if (gameObject == null)
		{
			Debug.LogWarning("Cannot equip item with prefabName: " + prefabNameForAvatarItemType + ", since prefab was not found");
			return null;
		}
		PickupItem component = gameObject.GetComponent<PickupItem>();
		if (component == null)
		{
			Debug.LogWarning("Item to equip is missing the AvatarItem script");
			return component;
		}
		component.VariantID = variantId;
		SetAvatarItemAsCurrent(component);
		return component;
	}
}
