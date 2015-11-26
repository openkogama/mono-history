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
	protected Transform center;

	protected MeshRenderer[] meshRenderers = new MeshRenderer[0];

	public Vector3 Origin => center.position;

	public virtual int Quantity => 0;

	public virtual Color CrossHairColor => Color.green;

	public virtual float ChargeState => 0f;

	public virtual bool ActivateGunModeOnEquip => true;

	public abstract AvatarItemType Type { get; }

	public int VariantID { get; set; }

	public static string GetPrefabNameForAvatarItemType(AvatarItemType type)
	{
		return type switch
		{
			AvatarItemType.CenterGun => "Prefabs/AvatarItems/AvatarItemCenterGun", 
			AvatarItemType.ImpulseGun => "Prefabs/AvatarItems/AvatarItemImpulseGun", 
			AvatarItemType.LaserPointer => "Prefabs/AvatarItems/AvatarItemLaserPointer", 
			AvatarItemType.Bazooka => "Prefabs/AvatarItems/AvatarItemBazooka", 
			AvatarItemType.Hand => "Prefabs/AvatarItems/AvatarItemHand", 
			AvatarItemType.RailGun => "Prefabs/AvatarItems/AvatarItemRailGun", 
			AvatarItemType.Sword => "Prefabs/AvatarItems/AvatarItemSword", 
			AvatarItemType.Shotgun => "Prefabs/AvatarItems/AvatarItemShotgun", 
			AvatarItemType.Flamethrower => "Prefabs/AvatarItems/AvatarItemFlamethrower", 
			AvatarItemType.CubeGun => "Prefabs/AvatarItems/AvatarItemCubeGun", 
			AvatarItemType.SixShooter => "Prefabs/AvatarItems/AvatarItemSixShooter", 
			AvatarItemType.DoubleSixShooter => "Prefabs/AvatarItems/AvatarItemDoubleSixShooter", 
			AvatarItemType.ThrowingStar => "Prefabs/AvatarItems/AvatarItemThrowingStar", 
			AvatarItemType.MultiThrowingStar => "Prefabs/AvatarItems/AvatarItemMultiThrowingStar", 
			AvatarItemType.GrowthGun => "Prefabs/AvatarItems/AvatarItemGrowthGun", 
			AvatarItemType.MouseGun => "Prefabs/AvatarItems/AvatarItemMouseGun", 
			AvatarItemType.SlapGun => "Prefabs/AvatarItems/AvatarItemSlapGun", 
			_ => string.Empty, 
		};
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
