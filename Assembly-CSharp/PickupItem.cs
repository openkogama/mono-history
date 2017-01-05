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

	private Vector3 previousHolsterPos;

	private Quaternion previousHolsterRot;

	private Vector3 previousHolsterScale;

	[SerializeField]
	protected Transform center;

	[SerializeField]
	protected MeshRenderer[] meshRenderers = new MeshRenderer[0];

	public bool IsHolstered { get; private set; }

	public Vector3 Origin => center.position;

	public virtual int Quantity => 0;

	public virtual Color CrossHairColor => Color.green;

	public virtual float ChargeState => 0f;

	public virtual bool ActivateGunModeOnEquip => true;

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
			AvatarItemType.GodzillaLaser => Object.Instantiate(PrefabPool.Instance.AvatarItemGodzillaLaser), 
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
			AvatarItemType.CollectTheItemCollectable => Object.Instantiate(PrefabPool.Instance.AvatarItemCollectTheItem), 
			_ => null, 
		};
	}

	public void HolsterPickup()
	{
		if (!IsHolstered)
		{
			if (owner.WorldObjectOwner is MVAvatar mVAvatar)
			{
				Transform partBone = mVAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.Holster);
				previousHolsterPos = transform.localPosition;
				previousHolsterRot = transform.localRotation;
				previousHolsterScale = transform.localScale;
				Quaternion localRotation = holsterTransformOffset.localRotation * partBone.localRotation;
				transform.localRotation = localRotation;
				Vector3 vector = partBone.position - holsterTransformOffset.position;
				transform.position += vector;
				transform.localScale = holsterTransformOffset.localScale;
				IsHolstered = true;
			}
			OnHolstered();
		}
	}

	public void UnholsterPickup()
	{
		if (IsHolstered)
		{
			transform.localPosition = previousHolsterPos;
			transform.localRotation = previousHolsterRot;
			transform.localScale = previousHolsterScale;
			IsHolstered = false;
			OnUnholstered();
		}
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
