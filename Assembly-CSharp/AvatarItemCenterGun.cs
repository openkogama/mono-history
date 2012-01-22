using UnityEngine;

public class AvatarItemCenterGun : AvatarItem
{
	public Transform muzzlePoint;

	public int ammo;

	public override int Quantity
	{
		get
		{
			return ammo;
		}
		set
		{
			ammo = value;
		}
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (ammo > 0)
		{
			Projectile projectile = Projectile.CreateProjectile("Prefabs/AvatarItemCenterGunProjectile", "ParticleFX/Sparks", "ParticleFX/Blood", muzzlePoint.position, owner.LookDirection);
			Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection);
			projectile.Fire(50f, 100f, instigatorActorNr, lineOfFire, owner.mvAvatar.Id);
			ammo--;
			((Component)this).audio.Play();
			if (ammo == 0)
			{
				owner.mvAvatar.Unequip(AvatarItemSlotName.Center);
			}
		}
	}

	public override void TriggerEnd()
	{
	}
}
