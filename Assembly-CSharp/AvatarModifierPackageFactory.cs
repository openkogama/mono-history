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
			new AvatarModifierPackage(AvatarModifierPackageType.FlamerBurn, AvatarModifierPackageAdditionPolicy.Renew, 0.5f, new AvatarModifierPackage.AvatarModifier[1]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Addition, AvatarModifierEffect.FlamerDamagePrSec, Const(25f))
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
			new AvatarModifierPackage(AvatarModifierPackageType.Shrunken, AvatarModifierPackageAdditionPolicy.Renew, 35f, new AvatarModifierPackage.AvatarModifier[7]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Speed, Const(0.4f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.Scale, Const(0.25f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.JumpPower, Const(0.6f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.DamageMultiplier, Const(4f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.DisableWeapons, Const(1f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.DisableVehicles, Const(1f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.Weight, Const(0.25f))
			}, new Dictionary<AvatarModifierPackageType, ModifierActions>
			{
				{
					AvatarModifierPackageType.Shrunken,
					ModifierActions.Renew
				},
				{
					AvatarModifierPackageType.Enlarged,
					ModifierActions.CancelOut
				}
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
			AvatarModifierPackageType.Shielded,
			new AvatarModifierPackage(AvatarModifierPackageType.Shielded, AvatarModifierPackageAdditionPolicy.Renew, float.PositiveInfinity, new AvatarModifierPackage.AvatarModifier[1]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.DamageMultiplier, Const(0.5f))
			}, null, persist: true)
		},
		{
			AvatarModifierPackageType.DisableVehiclePickup,
			new AvatarModifierPackage(AvatarModifierPackageType.DisableVehiclePickup, AvatarModifierPackageAdditionPolicy.Renew, float.PositiveInfinity, new AvatarModifierPackage.AvatarModifier[1]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.DisablePickups, Const(1f))
			})
		},
		{
			AvatarModifierPackageType.Enlarged,
			new AvatarModifierPackage(AvatarModifierPackageType.Enlarged, AvatarModifierPackageAdditionPolicy.Renew, 35f, new AvatarModifierPackage.AvatarModifier[7]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Speed, Const(1.5f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.Scale, Const(2f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.JumpPower, Const(2f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.DamageMultiplier, Const(0.5f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.DisableWeapons, Const(1f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.DisableVehicles, Const(1f)),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.Weight, Const(2f))
			}, new Dictionary<AvatarModifierPackageType, ModifierActions>
			{
				{
					AvatarModifierPackageType.Shrunken,
					ModifierActions.CancelOut
				},
				{
					AvatarModifierPackageType.Enlarged,
					ModifierActions.Renew
				}
			})
		},
		{
			AvatarModifierPackageType.GodzillaS,
			AssembleGodzillaModifierPackage(AvatarModifierPackageType.GodzillaS)
		},
		{
			AvatarModifierPackageType.GodzillaM,
			AssembleGodzillaModifierPackage(AvatarModifierPackageType.GodzillaM)
		},
		{
			AvatarModifierPackageType.GodzillaL,
			AssembleGodzillaModifierPackage(AvatarModifierPackageType.GodzillaL)
		},
		{
			AvatarModifierPackageType.GodzillaXL,
			AssembleGodzillaModifierPackage(AvatarModifierPackageType.GodzillaXL)
		},
		{
			AvatarModifierPackageType.GodzillaLaserBurnS,
			AssembleGodzillaLaserBurnModifierPackage(AvatarModifierPackageType.GodzillaS)
		},
		{
			AvatarModifierPackageType.GodzillaLaserBurnM,
			AssembleGodzillaLaserBurnModifierPackage(AvatarModifierPackageType.GodzillaM)
		},
		{
			AvatarModifierPackageType.GodzillaLaserBurnL,
			AssembleGodzillaLaserBurnModifierPackage(AvatarModifierPackageType.GodzillaL)
		},
		{
			AvatarModifierPackageType.GodzillaLaserBurnXL,
			AssembleGodzillaLaserBurnModifierPackage(AvatarModifierPackageType.GodzillaXL)
		},
		{
			AvatarModifierPackageType.GodzillaGrowthInvulnerability,
			AssembleInvulnerabilityPackage(AvatarModifierPackageType.GodzillaGrowthInvulnerability, 3f)
		},
		{
			AvatarModifierPackageType.SpawnProtection,
			AssembleInvulnerabilityPackage(AvatarModifierPackageType.SpawnProtection, 2f)
		}
	};

	public static Func<float> Const(float c)
	{
		return () => c;
	}

	private static AvatarModifierPackage AssembleInvulnerabilityPackage(AvatarModifierPackageType type, float time)
	{
		AvatarModifierPackage.AvatarModifier[] avatarModifiers = new AvatarModifierPackage.AvatarModifier[1]
		{
			new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.Invulnerable, Const(1f))
		};
		return new AvatarModifierPackage(type, AvatarModifierPackageAdditionPolicy.Renew, time, avatarModifiers, null, persist: true);
	}

	private static AvatarModifierPackage AssembleGodzillaModifierPackage(AvatarModifierPackageType godzillaType)
	{
		float sizeModifier = GodzillaModifier.constants[(GodzillaModifier.GodzillaModifierPackageType)godzillaType].sizeModifier;
		AvatarModifierPackage.AvatarModifier[] avatarModifiers = new AvatarModifierPackage.AvatarModifier[3]
		{
			new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.DamageMultiplier, Const(0.4f / sizeModifier)),
			new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.GodzillaImmunity, Const(1f)),
			new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.PoisonImmune, Const(1f))
		};
		return new AvatarModifierPackage(godzillaType, AvatarModifierPackageAdditionPolicy.Renew, float.PositiveInfinity, avatarModifiers);
	}

	private static AvatarModifierPackage AssembleGodzillaLaserBurnModifierPackage(AvatarModifierPackageType godzillaType)
	{
		AvatarModifierPackageType laserBurnModifierPackageType = GodzillaModifier.constants[(GodzillaModifier.GodzillaModifierPackageType)godzillaType].laserBurnModifierPackageType;
		float sizeModifier = GodzillaModifier.constants[(GodzillaModifier.GodzillaModifierPackageType)godzillaType].sizeModifier;
		AvatarModifierPackage.AvatarModifier[] avatarModifiers = new AvatarModifierPackage.AvatarModifier[1]
		{
			new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Override, AvatarModifierEffect.FlamerDamagePrSec, Const(0.25f * sizeModifier))
		};
		Dictionary<AvatarModifierPackageType, ModifierActions> dictionary = new Dictionary<AvatarModifierPackageType, ModifierActions>();
		dictionary.Add(laserBurnModifierPackageType, ModifierActions.Renew);
		Dictionary<AvatarModifierPackageType, ModifierActions> actionsToTakeVsTypes = dictionary;
		return new AvatarModifierPackage(laserBurnModifierPackageType, AvatarModifierPackageAdditionPolicy.Renew, 2f, avatarModifiers, actionsToTakeVsTypes);
	}

	public static AvatarModifierPackage GetPackage(AvatarModifierPackageType packageType)
	{
		AvatarModifierPackage result = protoPackages[packageType];
		result.Renew();
		return result;
	}
}
