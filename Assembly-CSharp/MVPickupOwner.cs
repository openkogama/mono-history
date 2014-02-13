using System;
using System.Collections;
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

	public Vector3 LookOrigin
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lookOrigin;
		}
	}

	public Vector3 LookDirection
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return lookDirection.normalized;
		}
	}

	public bool InGunMode
	{
		get
		{
			if ((Object)(object)CurrentItem == (Object)null)
			{
				return false;
			}
			return CurrentItem.ActivateGunModeOnEquip;
		}
	}

	public MVWorldObjectClient WorldObjectOwner => worldObjectParent;

	public virtual HashSet<int> IgnoreWOIDs => worldObjectParent.WorldIDsRecursive;

	protected MVPickupOwner()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	public float GetAbsolutProjectileSpeed(float projectileSpeed)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = lookDirection - lookDirection.normalized;
		float magnitude = val.magnitude;
		return projectileSpeed + magnitude;
	}

	private Vector3 GetLookDirectionWithAddedVelocityMagnitude(Vector3 lookDirection)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		MVRigidBody enabledMonoBehaviourHighestInHierarchy = MVWorldObjectClientManager.GetEnabledMonoBehaviourHighestInHierarchy<MVRigidBody>(((Component)this).gameObject);
		if ((Object)(object)enabledMonoBehaviourHighestInHierarchy == (Object)null)
		{
			return lookDirection;
		}
		Vector3 velocity = enabledMonoBehaviourHighestInHierarchy.Velocity;
		Vector3 normalized = velocity.normalized;
		Vector3 val = LookDirection;
		float num = Vector3.Dot(normalized, val.normalized);
		if (num <= 0f)
		{
			return lookDirection;
		}
		Vector3 velocity2 = enabledMonoBehaviourHighestInHierarchy.Velocity;
		return velocity2.magnitude * num * lookDirection + lookDirection;
	}

	protected abstract void Equip(AvatarItemType type, int variantId);

	protected abstract void Unequip();

	protected void Init(MVRuntimeDataVariable currentItemRuntimeVariable, MVRuntimeDataVariable isFiringRuntimeVariable)
	{
		currentItemRuntimeVariable.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(currentItemRuntimeVariable.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object item) =>
		{
			UpdateCurrentItem((Hashtable)item);
		}));
		UpdateCurrentItem((Hashtable)currentItemRuntimeVariable.Value);
		isFiringRuntimeVariable.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isFiringRuntimeVariable.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object value) =>
		{
			HandleFiring((bool)value);
		}));
	}

	public void HandleFire(bool inputFire, MVRuntimeDataVariable isFiringRuntimeVariable)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		SetLineOfFireLocal();
		bool flag = Object.op_Implicit((Object)(object)CurrentItem) && CurrentItem.CanFire() && inputFire;
		if ((bool)isFiringRuntimeVariable.Value != flag)
		{
			isFiringRuntimeVariable.Value = flag;
		}
		if (flag && Time.time - prevUpdateLineOfFireTime > 0.5f)
		{
			MVGameController.Instance.Game.UpdateLineOfFire(worldObjectParent.Id, lookDirection, lookOrigin);
			prevUpdateLineOfFireTime = Time.time;
		}
	}

	private void SetLineOfFireLocal()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		MVCameraBase curCamera = MVGameController.Instance.Game.CameraController.CurCamera;
		Vector3 fireDirection = curCamera.FireDirection;
		Vector3 fireOrigin = curCamera.FireOrigin;
		fireDirection = GetLookDirectionWithAddedVelocityMagnitude(fireDirection);
		SetLineOfFire(fireOrigin, fireDirection);
	}

	public void SetLineOfFire(Vector3 lookOrigin, Vector3 lookDirection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		this.lookOrigin = lookOrigin;
		this.lookDirection = lookDirection;
	}

	private void UpdateCurrentItem(Hashtable newState)
	{
		if (!newState.Contains("type"))
		{
			Unequip();
			return;
		}
		AvatarItemType avatarItemType = (AvatarItemType)(int)newState["type"];
		int num = (newState.ContainsKey("variantId") ? ((int)newState["variantId"]) : 0);
		if ((Object)(object)currentItem == (Object)null || avatarItemType != currentItem.Type || num != currentItem.VariantID)
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
		if ((Object)(object)currentItem != (Object)null)
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
		if ((Object)(object)currentItem != (Object)(object)avatarItem)
		{
			Unequip();
		}
		currentItem = avatarItem;
		currentItem.OnEquip();
	}

	protected PickupItem CreateAvatarItem(AvatarItemType type, int variantId)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected Obj, but got Unknown
		string prefabNameForAvatarItemType = PickupItem.GetPrefabNameForAvatarItemType(type);
		if (prefabNameForAvatarItemType.Length == 0)
		{
			Debug.LogWarning((object)"Trying to equip an un-implemented item type!");
			return null;
		}
		prefabNameForAvatarItemType += variantId;
		GameObject val = (GameObject)Object.Instantiate(Resources.Load(prefabNameForAvatarItemType));
		if ((Object)(object)val == (Object)null)
		{
			Debug.LogWarning((object)("Cannot equip item with prefabName: " + prefabNameForAvatarItemType + ", since prefab was not found"));
			return null;
		}
		PickupItem component = val.GetComponent<PickupItem>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogWarning((object)"Item to equip is missing the AvatarItem script");
			return component;
		}
		component.VariantID = variantId;
		SetAvatarItemAsCurrent(component);
		return component;
	}
}
