using System.Collections;
using MV.Common;
using UnityEngine;

public abstract class PickupItem : MonoBehaviour
{
	public MVPickupOwner owner;

	protected MeshRenderer[] meshRenderers = new MeshRenderer[0];

	public virtual int Quantity => 0;

	public virtual Color CrossHairColor
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Color.green;
		}
	}

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

	public virtual void OnStateChanged(Hashtable newState)
	{
	}

	public virtual void OnEquip()
	{
	}

	public virtual void OnUnequip()
	{
	}
}
