using System;
using UnityEngine;

public struct AvatarModifierPackage(AvatarModifierPackageType avatarModifierPackageType, AvatarModifierPackageAdditionPolicy avatarModifierPackageAdditionPolicy, float duration, AvatarModifierPackage.AvatarModifier[] avatarModifiers)
{
	public struct AvatarModifier(AvatarModifierType avatarModifierType, AvatarModifierEffect avatarModifierEffect, Func<float> value)
	{
		public AvatarModifierType avatarModifierType = avatarModifierType;

		public AvatarModifierEffect avatarModifierEffect = avatarModifierEffect;

		public Func<float> value = value;
	}

	public int id = -1;

	public float duration = duration;

	public AvatarModifier[] avatarModifiers = avatarModifiers;

	private float timeStamp = Time.time;

	private AvatarModifierPackageType avatarModifierPackageType = avatarModifierPackageType;

	private AvatarModifierPackageAdditionPolicy avatarModifierPackageAdditionPolicy = avatarModifierPackageAdditionPolicy;

	public AvatarModifierPackageType AvatarModifierPackageType => avatarModifierPackageType;

	public AvatarModifierPackageAdditionPolicy AvatarModifierPackageAdditionPolicy => avatarModifierPackageAdditionPolicy;

	public bool IsExpired
	{
		get
		{
			if (duration > Time.time - timeStamp)
			{
				return false;
			}
			return true;
		}
		set
		{
			if (value)
			{
				timeStamp = (0f - duration) * 2f;
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
