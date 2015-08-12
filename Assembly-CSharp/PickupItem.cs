using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public abstract class PickupItem : MonoBehaviour
{
	public MVPickupOwner owner;

	protected MeshRenderer[] meshRenderers = new MeshRenderer[0];

	public virtual int Quantity => 0;

	public virtual Color CrossHairColor => Color.green;

	public virtual float ChargeState => 0f;

	public virtual bool ActivateGunModeOnEquip => GameDB.GameType switch
	{
		MVGameType.Classic => true, 
		MVGameType.Platformer => false, 
		_ => true, 
	};

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
			_ => string.Empty, 
		};
	}

	public virtual MeshRenderer[] GetMeshRenderers()
	{
		return meshRenderers;
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
}
