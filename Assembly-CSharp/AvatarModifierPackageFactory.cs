using System;
using System.Collections.Generic;

public static class AvatarModifierPackageFactory
{
	private static Dictionary<AvatarModifierPackageType, AvatarModifierPackage> protoPackages = new Dictionary<AvatarModifierPackageType, AvatarModifierPackage>
	{
		{
			AvatarModifierPackageType.Fire,
			new AvatarModifierPackage(AvatarModifierPackageType.Fire, AvatarModifierPackageAdditionPolicy.Renew, 1f, new AvatarModifierPackage.AvatarModifier[1]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Addition, AvatarModifierEffect.EnvironmentDamagePrSec, Const(10f))
			})
		},
		{
			AvatarModifierPackageType.Mutant,
			new AvatarModifierPackage(AvatarModifierPackageType.Mutant, AvatarModifierPackageAdditionPolicy.Renew, 20f, new AvatarModifierPackage.AvatarModifier[6]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.JumpPower, Const(1.5f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Speed, Const(1.25f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.DisablePickups, Const(1f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.DisableVehicles, Const(1f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.PoisonImmune, Const(1f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.Invulnerable, Const(1f))
			})
		},
		{
			AvatarModifierPackageType.Sticky,
			new AvatarModifierPackage(AvatarModifierPackageType.Sticky, AvatarModifierPackageAdditionPolicy.Renew, 0.4f, new AvatarModifierPackage.AvatarModifier[3]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.JumpPower, Const(0.01f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Speed, Const(0.03f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.Friction, Const(1f))
			})
		},
		{
			AvatarModifierPackageType.Poison,
			new AvatarModifierPackage(AvatarModifierPackageType.Poison, AvatarModifierPackageAdditionPolicy.Renew, float.PositiveInfinity, new AvatarModifierPackage.AvatarModifier[1]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Addition, AvatarModifierEffect.EnvironmentDamagePrSec, Const(25f))
			})
		},
		{
			AvatarModifierPackageType.InstantDeath,
			new AvatarModifierPackage(AvatarModifierPackageType.InstantDeath, AvatarModifierPackageAdditionPolicy.Renew, float.PositiveInfinity, new AvatarModifierPackage.AvatarModifier[1]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Addition, AvatarModifierEffect.EnvironmentDamagePrSec, Const(150f))
			})
		},
		{
			AvatarModifierPackageType.WallJump,
			new AvatarModifierPackage(AvatarModifierPackageType.WallJump, AvatarModifierPackageAdditionPolicy.Renew, 0.2f, new AvatarModifierPackage.AvatarModifier[1]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.WallJump, Const(1f))
			})
		},
		{
			AvatarModifierPackageType.NoFriction,
			new AvatarModifierPackage(AvatarModifierPackageType.NoFriction, AvatarModifierPackageAdditionPolicy.Renew, 0.2f, new AvatarModifierPackage.AvatarModifier[1]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.Friction, Const(0f))
			})
		},
		{
			AvatarModifierPackageType.FlamerBurn,
			new AvatarModifierPackage(AvatarModifierPackageType.FlamerBurn, AvatarModifierPackageAdditionPolicy.Renew, 0.8f, new AvatarModifierPackage.AvatarModifier[1]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Addition, AvatarModifierEffect.FlamerDamagePrSec, Const(20f))
			})
		},
		{
			AvatarModifierPackageType.Underwater,
			new AvatarModifierPackage(AvatarModifierPackageType.Underwater, AvatarModifierPackageAdditionPolicy.Renew, 10f, new AvatarModifierPackage.AvatarModifier[2]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Density, Const(0.5f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.VelocityDamping, Const(0.95f))
			})
		},
		{
			AvatarModifierPackageType.Frozen,
			new AvatarModifierPackage(AvatarModifierPackageType.Frozen, AvatarModifierPackageAdditionPolicy.Renew, 4.2f, new AvatarModifierPackage.AvatarModifier[3]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.Friction, Const(0f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.JumpPower, Const(0.1f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Speed, Const(0.4f))
			})
		},
		{
			AvatarModifierPackageType.NinjaRun,
			new AvatarModifierPackage(AvatarModifierPackageType.NinjaRun, AvatarModifierPackageAdditionPolicy.Renew, 7f, new AvatarModifierPackage.AvatarModifier[2]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Speed, Const(3f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.ThrustFactor, Const(20f))
			})
		},
		{
			AvatarModifierPackageType.Shrunken,
			new AvatarModifierPackage(AvatarModifierPackageType.Shrunken, AvatarModifierPackageAdditionPolicy.Renew, 99999f, new AvatarModifierPackage.AvatarModifier[3]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Speed, Const(0.25f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.Scale, Const(0.25f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.JumpPower, Const(0.25f))
			})
		},
		{
			AvatarModifierPackageType.WindFriction,
			new AvatarModifierPackage(AvatarModifierPackageType.WindFriction, AvatarModifierPackageAdditionPolicy.Renew, float.PositiveInfinity, new AvatarModifierPackage.AvatarModifier[1]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.VelocityDamping, Const(0.96f))
			})
		},
		{
			AvatarModifierPackageType.DisableVehiclePickup,
			new AvatarModifierPackage(AvatarModifierPackageType.DisableVehiclePickup, AvatarModifierPackageAdditionPolicy.Renew, float.PositiveInfinity, new AvatarModifierPackage.AvatarModifier[1]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.DisablePickups, Const(1f))
			})
		}
	};

	private static Func<float> Const(float c)
	{
		return () => c;
	}

	public static AvatarModifierPackage GetPackage(AvatarModifierPackageType packageType)
	{
		AvatarModifierPackage result = protoPackages[packageType];
		result.Renew();
		return result;
	}
}
