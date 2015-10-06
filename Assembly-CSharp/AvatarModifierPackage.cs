using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public struct AvatarModifierPackage(AvatarModifierPackageType avatarModifierPackageType, AvatarModifierPackageAdditionPolicy avatarModifierPackageAdditionPolicy, float duration, AvatarModifierPackage.AvatarModifier[] avatarModifiers, Dictionary<AvatarModifierPackageType, ModifierActions> actionsToTakeVsTypes = null)
{
	public struct AvatarModifier(AvatarModifierType avatarModifierType, AvatarModifierEffect avatarModifierEffect, Func<float> value)
	{
		public AvatarModifierType avatarModifierType = avatarModifierType;

		public AvatarModifierEffect avatarModifierEffect = avatarModifierEffect;

		public Func<float> value = value;
	}

	public int id = -1;

	public ObscuredFloat duration = duration;

	public AvatarModifier[] avatarModifiers = avatarModifiers;

	public Dictionary<AvatarModifierPackageType, ModifierActions> actionsToTakeVsTypes = actionsToTakeVsTypes;

	private ObscuredFloat timeStamp = Time.time;

	private AvatarModifierPackageType avatarModifierPackageType = avatarModifierPackageType;

	private AvatarModifierPackageAdditionPolicy avatarModifierPackageAdditionPolicy = avatarModifierPackageAdditionPolicy;

	public AvatarModifierPackageType AvatarModifierPackageType => avatarModifierPackageType;

	public AvatarModifierPackageAdditionPolicy AvatarModifierPackageAdditionPolicy => avatarModifierPackageAdditionPolicy;

	public bool IsExpired
	{
		get
		{
			if ((float)duration > Time.time - (float)timeStamp)
			{
				return false;
			}
			return true;
		}
		set
		{
			if (value)
			{
				timeStamp = (0f - (float)duration) * 2f;
			}
		}
	}

	public bool IsEqualTo(AvatarModifierPackage other)
	{
		return other.AvatarModifierPackageType == AvatarModifierPackageType && other.id == id;
	}

	public void Renew()
	{
		timeStamp = Time.time;
	}
}
