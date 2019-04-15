using System;
using System.Collections.Generic;
using System.Linq;

public class AvatarModifierPackages
{
	public delegate void OnModifierExpiredDelegate(AvatarModifierPackage modifier);

	public OnModifierExpiredDelegate OnModifierExpired;

	private List<AvatarModifierPackage> packages = new List<AvatarModifierPackage>();

	public event EventHandler<EventArgs> OnUnequipItemEvent;

	public event EventHandler<EventArgs> OnDisableVehiclesEvent;

	public void Update()
	{
		for (int num = packages.Count - 1; num >= 0; num--)
		{
			if (packages[num].IsExpired)
			{
				if (OnModifierExpired != null)
				{
					OnModifierExpired(packages[num]);
				}
				packages.RemoveAt(num);
			}
		}
	}

	public void ClearModifiers()
	{
		for (int num = packages.Count - 1; num >= 0; num--)
		{
			if (OnModifierExpired != null)
			{
				OnModifierExpired(packages[num]);
			}
			packages.RemoveAt(num);
		}
	}

	public void ClearNonPersistantModifiers()
	{
		for (int num = packages.Count - 1; num >= 0; num--)
		{
			if (!packages[num].persistant)
			{
				if (OnModifierExpired != null)
				{
					OnModifierExpired(packages[num]);
				}
				packages.RemoveAt(num);
			}
		}
	}

	public bool HasModifier(AvatarModifierPackageType type)
	{
		return packages.Exists((AvatarModifierPackage p) => p.AvatarModifierPackageType == type);
	}

	private void AddModifierPackage(AvatarModifierPackage modifierPackage, int id)
	{
		modifierPackage.id = id;
		if (!HandleNewAvatarModifierPackage(modifierPackage))
		{
			return;
		}
		if (modifierPackage.AvatarModifierPackageAdditionPolicy == AvatarModifierPackageAdditionPolicy.Renew)
		{
			int num = packages.FindIndex((AvatarModifierPackage x) => modifierPackage.IsEqualTo(x));
			if (num != -1)
			{
				AvatarModifierPackage value = packages[num];
				value.Renew();
				packages[num] = value;
				return;
			}
		}
		packages.Add(modifierPackage);
	}

	public void AddModifier(AvatarModifierPackageType modifierPackageType, int id = -1, AvatarModifierPackage.AvatarModifier[] additionalModifers = null)
	{
		if (modifierPackageType == AvatarModifierPackageType.None)
		{
			return;
		}
		AvatarModifierPackage package = AvatarModifierPackageFactory.GetPackage(modifierPackageType);
		if (additionalModifers != null)
		{
			package.avatarModifiers = package.avatarModifiers.Union(additionalModifers).ToArray();
		}
		AddModifierPackage(package, id);
		AvatarModifierPackage.AvatarModifier[] avatarModifiers = package.avatarModifiers;
		for (int i = 0; i < avatarModifiers.Length; i++)
		{
			AvatarModifierPackage.AvatarModifier avatarModifier = avatarModifiers[i];
			if ((avatarModifier.avatarModifierEffect == AvatarModifierEffect.DisablePickups || avatarModifier.avatarModifierEffect == AvatarModifierEffect.DisableWeapons) && OnUnequipItemEvent != null)
			{
				OnUnequipItemEvent(this, EventArgs.Empty);
			}
			if (avatarModifier.avatarModifierEffect == AvatarModifierEffect.DisableVehicles && OnDisableVehiclesEvent != null)
			{
				OnDisableVehiclesEvent(this, EventArgs.Empty);
			}
		}
	}

	public ModifierActions GetActionToTakeWithPackageType(AvatarModifierPackageType modifierPackageType)
	{
		foreach (AvatarModifierPackage package in packages)
		{
			if (package.actionsToTakeVsTypes == null)
			{
				continue;
			}
			foreach (AvatarModifierPackageType key in package.actionsToTakeVsTypes.Keys)
			{
				if (key == modifierPackageType)
				{
					return package.actionsToTakeVsTypes[key];
				}
			}
		}
		return ModifierActions.Add;
	}

	private bool HandleNewAvatarModifierPackage(AvatarModifierPackage newAvatarModifierPackage)
	{
		bool result = true;
		foreach (AvatarModifierPackage package in packages)
		{
			ModifierActions modifierActions = ModifierActions.Add;
			if (newAvatarModifierPackage.actionsToTakeVsTypes == null)
			{
				continue;
			}
			foreach (AvatarModifierPackageType key in newAvatarModifierPackage.actionsToTakeVsTypes.Keys)
			{
				if (key == package.AvatarModifierPackageType)
				{
					modifierActions = newAvatarModifierPackage.actionsToTakeVsTypes[key];
					break;
				}
			}
			switch (modifierActions)
			{
			case ModifierActions.Renew:
				package.Renew();
				break;
			case ModifierActions.Replace:
				RemoveModifier(package.AvatarModifierPackageType, package.id);
				break;
			case ModifierActions.CancelOut:
				RemoveModifier(package.AvatarModifierPackageType, package.id);
				result = false;
				break;
			}
		}
		return result;
	}

	public AvatarModifierPackageType GetPackageToActWith(AvatarModifierPackageType modifierPackageType, ModifierActions action)
	{
		AvatarModifierPackage package = AvatarModifierPackageFactory.GetPackage(modifierPackageType);
		if (package.actionsToTakeVsTypes != null)
		{
			foreach (AvatarModifierPackageType key in package.actionsToTakeVsTypes.Keys)
			{
				if (package.actionsToTakeVsTypes[key] == action)
				{
					return key;
				}
			}
		}
		return AvatarModifierPackageType.None;
	}

	private void RemoveModifierPackage(AvatarModifierPackage modifierPackage, int id)
	{
		modifierPackage.id = id;
		int num = packages.FindIndex((AvatarModifierPackage x) => modifierPackage.IsEqualTo(x));
		if (num != -1)
		{
			AvatarModifierPackage value = packages[num];
			value.IsExpired = true;
			packages[num] = value;
		}
	}

	public void RemoveModifier(AvatarModifierPackageType modifierPackageType, int id = -1)
	{
		if (modifierPackageType != AvatarModifierPackageType.None)
		{
			RemoveModifierPackage(AvatarModifierPackageFactory.GetPackage(modifierPackageType), id);
		}
	}

	public float HandleModifierEffect(AvatarModifierEffect modifierEffect, float baseValue)
	{
		List<AvatarModifierPackage.AvatarModifier> list = new List<AvatarModifierPackage.AvatarModifier>();
		foreach (AvatarModifierPackage package in packages)
		{
			AvatarModifierPackage.AvatarModifier[] avatarModifiers = package.avatarModifiers;
			for (int i = 0; i < avatarModifiers.Length; i++)
			{
				AvatarModifierPackage.AvatarModifier item = avatarModifiers[i];
				if (item.avatarModifierEffect == modifierEffect)
				{
					list.Add(item);
				}
			}
		}
		list.Sort((AvatarModifierPackage.AvatarModifier x, AvatarModifierPackage.AvatarModifier y) => x.avatarModifierType.CompareTo(y.avatarModifierType));
		float num = baseValue;
		for (int num2 = 0; num2 < list.Count; num2++)
		{
			switch (list[num2].avatarModifierType)
			{
			case AvatarModifierType.Multiply:
				num *= list[num2].value();
				break;
			case AvatarModifierType.Addition:
				num += list[num2].value();
				break;
			case AvatarModifierType.Override:
				num = list[num2].value();
				break;
			}
		}
		return num;
	}

	public bool HasModifierEffect(AvatarModifierEffect modifierEffect)
	{
		foreach (AvatarModifierPackage package in packages)
		{
			AvatarModifierPackage.AvatarModifier[] avatarModifiers = package.avatarModifiers;
			for (int i = 0; i < avatarModifiers.Length; i++)
			{
				AvatarModifierPackage.AvatarModifier avatarModifier = avatarModifiers[i];
				if (avatarModifier.avatarModifierEffect == modifierEffect)
				{
					return true;
				}
			}
		}
		return false;
	}

	public Dictionary<int, float> ComputeModifierEffectGroupedById(AvatarModifierEffect modifierEffect, float baseValue)
	{
		Dictionary<int, List<AvatarModifierPackage.AvatarModifier>> dictionary = new Dictionary<int, List<AvatarModifierPackage.AvatarModifier>>();
		foreach (AvatarModifierPackage package in packages)
		{
			AvatarModifierPackage.AvatarModifier[] avatarModifiers = package.avatarModifiers;
			for (int i = 0; i < avatarModifiers.Length; i++)
			{
				AvatarModifierPackage.AvatarModifier item = avatarModifiers[i];
				if (item.avatarModifierEffect == modifierEffect)
				{
					if (!dictionary.ContainsKey(package.id))
					{
						dictionary.Add(package.id, new List<AvatarModifierPackage.AvatarModifier>());
					}
					dictionary[package.id].Add(item);
				}
			}
		}
		Dictionary<int, float> dictionary2 = new Dictionary<int, float>();
		foreach (KeyValuePair<int, List<AvatarModifierPackage.AvatarModifier>> item2 in dictionary)
		{
			item2.Value.Sort((AvatarModifierPackage.AvatarModifier x, AvatarModifierPackage.AvatarModifier y) => x.avatarModifierType.CompareTo(y.avatarModifierType));
			float num = baseValue;
			for (int num2 = 0; num2 < item2.Value.Count; num2++)
			{
				switch (item2.Value[num2].avatarModifierType)
				{
				case AvatarModifierType.Multiply:
					num *= item2.Value[num2].value();
					break;
				case AvatarModifierType.Addition:
					num += item2.Value[num2].value();
					break;
				case AvatarModifierType.Override:
					num = item2.Value[num2].value();
					break;
				}
			}
			dictionary2.Add(item2.Key, num);
		}
		return dictionary2;
	}
}
