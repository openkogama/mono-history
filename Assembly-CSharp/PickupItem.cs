using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public abstract class PickupItem : MonoBehaviour
{
	protected bool firedThisFrame;

	public MVPickupOwner owner;

	[SerializeField]
	protected Transform muzzlePoint;

	[SerializeField]
	protected Transform holsterTransformOffset;

	[SerializeField]
	protected Transform firstPersonTransform;

	private Transform originalParent;

	private Vector3 originalPos;

	private Quaternion originalRot;

	private Vector3 originalScale;

	[SerializeField]
	protected Transform center;

	[SerializeField]
	protected MeshRenderer[] meshRenderers = new MeshRenderer[0];

	public bool IsHolstered { get; private set; }

	protected virtual bool IsAmmoDepleted => Quantity <= 0;

	public Vector3 Origin => center.position;

	public virtual int Quantity => 0;

	public virtual Color CrossHairColor => Color.green;

	public virtual float ChargeState => 0f;

	public virtual bool ActivateGunModeOnEquip => true;

	public virtual bool CanHolster => true;

	public bool FirstPersonCapable => firstPersonTransform != null;

	public bool IsInFirstPersonMode => firstPersonTransform != null && !IsHolstered;

	public bool IsAmmoEmpty => IsAmmoDepleted;

	public virtual bool CanUnequip => true;

	public abstract AvatarItemType Type { get; }

	public int VariantID { get; set; }

	public static GameObject InstantiateAvatarItemType(AvatarItemType type)
	{
		return type switch
		{
			AvatarItemType.CenterGun => Object.Instantiate(PrefabPool.Instance.AvatarItemCenterGun), 
			AvatarItemType.ImpulseGun => Object.Instantiate(PrefabPool.Instance.AvatarItemImpulseGun), 
			AvatarItemType.LaserPointer => Object.Instantiate(PrefabPool.Instance.AvatarItemLaserPointer), 
			AvatarItemType.Bazooka => Object.Instantiate(PrefabPool.Instance.AvatarItemBazooka), 
			AvatarItemType.Hand => Object.Instantiate(PrefabPool.Instance.AvatarItemHand), 
			AvatarItemType.RailGun => Object.Instantiate(PrefabPool.Instance.AvatarItemRailGun), 
			AvatarItemType.Sword => Object.Instantiate(PrefabPool.Instance.AvatarItemSword), 
			AvatarItemType.Shotgun => Object.Instantiate(PrefabPool.Instance.AvatarItemShotgun), 
			AvatarItemType.Flamethrower => Object.Instantiate(PrefabPool.Instance.AvatarItemFlamethrower), 
			AvatarItemType.CubeGun => Object.Instantiate(PrefabPool.Instance.AvatarItemCubeGun), 
			AvatarItemType.SixShooter => Object.Instantiate(PrefabPool.Instance.AvatarItemSixShooter), 
			AvatarItemType.DoubleSixShooter => Object.Instantiate(PrefabPool.Instance.AvatarItemDoubleSixShooter), 
			AvatarItemType.ThrowingStar => Object.Instantiate(PrefabPool.Instance.AvatarItemThrowingStar), 
			AvatarItemType.MultiThrowingStar => Object.Instantiate(PrefabPool.Instance.AvatarItemMultiThrowingStar), 
			AvatarItemType.GrowthGun => Object.Instantiate(PrefabPool.Instance.AvatarItemGrowthGun), 
			AvatarItemType.MouseGun => Object.Instantiate(PrefabPool.Instance.AvatarItemMouseGun), 
			AvatarItemType.SlapGun => Object.Instantiate(PrefabPool.Instance.AvatarItemSlapGun), 
			AvatarItemType.HealRay => Object.Instantiate(PrefabPool.Instance.AvatarItemHealRay), 
			AvatarItemType.CollectTheItemCollectable => Object.Instantiate(PrefabPool.Instance.AvatarItemCollectTheItem), 
			_ => null, 
		};
	}

	public void HolsterPickup(Transform targetHolsterTransform)
	{
		if (!IsHolstered)
		{
			AlignThisTo(targetHolsterTransform, holsterTransformOffset);
			IsHolstered = true;
			OnHolstered();
		}
	}

	public void UnholsterPickup()
	{
		if (IsHolstered)
		{
			RevertToOriginalTransform();
			IsHolstered = false;
			OnUnholstered();
		}
	}

	public void EnterFirstPersonView(MVCameraBase camera)
	{
		if (!IsHolstered)
		{
			transform.SetParent(camera.transform, worldPositionStays: false);
			transform.localPosition = firstPersonTransform.localPosition;
			transform.localRotation = firstPersonTransform.localRotation;
			transform.localScale = firstPersonTransform.localScale;
		}
	}

	public void LeaveFirstPersonView()
	{
		if (!IsHolstered)
		{
			RevertToOriginalTransform();
		}
	}

	private void RevertToOriginalTransform()
	{
		transform.SetParent(originalParent, worldPositionStays: false);
		transform.localPosition = originalPos;
		transform.localRotation = originalRot;
		transform.localScale = originalScale;
	}

	private void AlignThisTo(Transform targetHolsterTransform, Transform offset)
	{
		transform.SetParent(originalParent, worldPositionStays: false);
		Quaternion localRotation = offset.localRotation * targetHolsterTransform.localRotation;
		transform.localRotation = localRotation;
		Vector3 vector = targetHolsterTransform.position - offset.position;
		transform.position += vector;
		transform.localScale = offset.localScale;
	}

	public virtual bool CanFire()
	{
		return true;
	}

	public virtual void TriggerBegin(int instigatorActorNr)
	{
	}

	public virtual void TriggerEnd()
	{
	}

	public virtual void OnStateChanged(Dictionary<object, object> newState)
	{
	}

	public virtual void OnEquip()
	{
		originalParent = transform.parent;
		originalPos = transform.localPosition;
		originalRot = transform.localRotation;
		originalScale = transform.localScale;
	}

	public virtual void OnUnequip()
	{
	}

	public virtual void ResetAmmo()
	{
	}

	public virtual void OnLeaveVehicleWithWeapon()
	{
	}

	public virtual void OnEnterVehicleWithWeapon()
	{
	}

	protected virtual void OnHolstered()
	{
		TriggerEnd();
	}

	protected virtual void OnUnholstered()
	{
	}

	protected virtual int GetAmmoMultiplier(int defaultAmmo)
	{
		if (MVGameControllerBase.Game.LocalPlayer.BoostController.TryGetActiveBoost(BoostType.AmmoIntMultiplier, out var boost))
		{
			return defaultAmmo * (int)boost.Value;
		}
		return defaultAmmo;
	}

	public virtual void UpdateWithDirection(Vector3 dir)
	{
		dir.Normalize();
		Vector3 vector = transform.rotation * Vector3.forward;
		Debug.DrawLine(center.position, center.position + vector, Color.red);
		Debug.DrawLine(center.position, center.position + dir, Color.blue);
		Vector3 vector2 = Vector3.Cross(Vector3.up, vector);
		vector2.Normalize();
		Debug.DrawLine(center.position, center.position + vector2, Color.green);
		float num = MathFunctions.SignedAngle(dir, vector, -vector2);
		Quaternion localRotation = Quaternion.Euler(num * 57.29578f, 0f, 0f);
		center.localRotation = localRotation;
	}

	public bool GetAndResetFiredThisFrame()
	{
		bool result = firedThisFrame;
		firedThisFrame = false;
		return result;
	}
}
