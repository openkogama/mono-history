using System.Collections.Generic;
using System.Linq;

public class AvatarModifierPackages
{
	public delegate void OnModifierExpiredDelegate(AvatarModifierPackage modifier);

	public OnModifierExpiredDelegate OnModifierExpired;

	private List<AvatarModifierPackage> packages = new List<AvatarModifierPackage>();

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
		foreach (AvatarModifierPackage package in packages)
		{
			if (OnModifierExpired != null)
			{
				OnModifierExpired(package);
			}
		}
		packages.Clear();
	}

	public bool HasModifier(AvatarModifierPackageType type)
	{
		return packages.Exists((AvatarModifierPackage p) => p.AvatarModifierPackageType == type);
	}

	private void AddModifierPackage(AvatarModifierPackage modifierPackage, int id)
	{
		modifierPackage.id = id;
		if (modifierPackage.AvatarModifierPackageAdditionPolicy == AvatarModifierPackageAdditionPolicy.Renew)
		{
			int num = packages.FindIndex((AvatarModifierPackage x) => modifierPackage.IsEqualTo(x));
			if (num != -1)
			{
				AvatarModifierPackage value = packages[num];
				value.Renew();
				packages[num] = value;
			}
			else
			{
				packages.Add(modifierPackage);
			}
		}
		else
		{
			packages.Add(modifierPackage);
		}
	}

	public void AddModifier(AvatarModifierPackageType modifierPackageType, int id = -1, AvatarModifierPackage.AvatarModifier[] additionalModifers = null)
	{
		if (modifierPackageType != AvatarModifierPackageType.None)
		{
			AvatarModifierPackage package = AvatarModifierPackageFactory.GetPackage(modifierPackageType);
			if (additionalModifers != null)
			{
				package.avatarModifiers = package.avatarModifiers.Union(additionalModifers).ToArray();
			}
			AddModifierPackage(package, id);
		}
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
