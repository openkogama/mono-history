using System.Collections.Generic;
using MV.WorldObject;

public static class AvatarPackages
{
	public static Dictionary<InteractionPackageType, InteractionPackage> packages = new Dictionary<InteractionPackageType, InteractionPackage>
	{
		{
			InteractionPackageType.ImpulseGunHit,
			new ImpulseHitPackage()
		},
		{
			InteractionPackageType.SwordHit,
			new SwordHitPackage()
		},
		{
			InteractionPackageType.RailGunHit,
			new RailgunHitPackage()
		},
		{
			InteractionPackageType.MutantHit,
			new MutantHitPackage()
		},
		{
			InteractionPackageType.ShotgunHit,
			new ShotgunHitPackage()
		},
		{
			InteractionPackageType.FlamethrowerHit,
			new FlamethrowerHitPackage()
		},
		{
			InteractionPackageType.CenterGun,
			new CenterGunHitPackage()
		},
		{
			InteractionPackageType.SentryTowerFire,
			new SentryTowerFirePackage()
		},
		{
			InteractionPackageType.SentryTowerIce,
			new SentryTowerIcePackage()
		},
		{
			InteractionPackageType.AdvancedGhostBodyRotateWeaponPackage,
			new AdvancedGhostBodyRotateWeaponPackage()
		},
		{
			InteractionPackageType.ProximityDamageAndImpulse,
			new ProximityDamageAndImpulse()
		}
	};
}
