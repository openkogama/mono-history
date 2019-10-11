using System;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePointCalculators;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePrototypeSettings;

public static class AttributePrototypeSettingsManager
{
	private static readonly Dictionary<AttributeSettingWoType, KogamaSettingWrapperBase> attributeSettingRoots = new Dictionary<AttributeSettingWoType, KogamaSettingWrapperBase>
	{
		{
			AttributeSettingWoType.Avatar,
			CreateAvatarPrototypes()
		},
		{
			AttributeSettingWoType.HoverCraft,
			CreateHoverCraftPrototypes()
		}
	};

	public static KogamaSettingWrapperBase GetRoot(AttributeSettingWoType attributeSettingWoType)
	{
		return attributeSettingRoots[attributeSettingWoType];
	}

	private static KogamaSettingWrapperBase CreateAvatarPrototypes()
	{
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = new KogamaSettingsCollectionBase("AvatarSettings", null);
		AttributeSettingInt kogamaSetting = new AttributeSettingInt("SuperSpeed", 100, 20, 200, new APIntCalcZeroValueLinear(100, 0.5f), AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting);
		KogamaSettingBoolBase kogamaSetting2 = new AttributeSettingBool("DoubleJump", value: true, 100, AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting2);
		KogamaSettingBoolBase kogamaSetting3 = new AttributeSettingBool("EndlessAmmo", value: true, 100, AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting3);
		AttributeSettingFloat kogamaSetting4 = new AttributeSettingFloat("OxygenSupply", 20f, 0f, 500f, new APFloatCalcZeroValueLinear(20f, 0.1f, 0.5f), AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting4);
		AttributeSettingInt kogamaSetting5 = new AttributeSettingInt("MaxHealth", 100, 1, 200, new APIntCalcZeroValueLinear(100, 0.5f, 1f), AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting5);
		AttributeSettingInt kogamaSetting6 = new AttributeSettingInt("DamageReduction", 20, 10, 80, new APIntCalcLinear(3f), AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting6);
		AttributeSettingInt kogamaSetting7 = new AttributeSettingInt("JumpHeight", 100, 20, 200, new APIntCalcZeroValueLinear(100, 0.5f), AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting7);
		AttributeSettingInt kogamaSetting8 = new AttributeSettingInt("SlowFall", 50, 20, 80, new APIntCalcLinear(0.5f), AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting8);
		KogamaSettingBoolBase kogamaSetting9 = new AttributeSettingBool("CanWallJumpAnySurface", value: true, 25, AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting9);
		AttributeSettingInt kogamaSetting10 = new AttributeSettingInt("FrictionMultiplier", 50, 25, 100, new APIntCalcLinear(-0.1f), AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting10);
		KogamaSettingBoolBase kogamaSetting11 = new AttributeSettingBool("BreathesWater", value: true, 0, AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting11);
		KogamaSettingBoolBase kogamaSetting12 = new AttributeSettingBool("UnableToEquipWeapons", value: true, -50, AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting12);
		KogamaSettingBoolBase kogamaSetting13 = new AttributeSettingBool("UnableToCollectModifierPickups", value: true, -30, AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting13);
		return kogamaSettingsCollectionBase;
	}

	private static KogamaSettingWrapperBase CreateHoverCraftPrototypes()
	{
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = new KogamaSettingsCollectionBase("HoverSettings", null);
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase2 = new KogamaSettingsCollectionBase("HoverSubSettings", kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSettingsCollectionBase2);
		AttributeSettingInt kogamaSetting = new AttributeSettingInt("HoverSpeed", 5, 1, 10, new APIntCalcLinear(10f), AttributeSettingsExclusivityFlag.Jump, kogamaSettingsCollectionBase2);
		AttributeSettingBool kogamaSetting2 = new AttributeSettingBool("HoverJump", value: true, 100, AttributeSettingsExclusivityFlag.None, kogamaSettingsCollectionBase2);
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		dictionary.Add(0, 10);
		dictionary.Add(1, 100);
		dictionary.Add(2, 1000);
		AttributeSettingEnum kogamaSetting3 = new AttributeSettingEnum("RocketType", 0, dictionary, (int)Enum.GetValues(typeof(RocketType)).Cast<RocketType>().Min(), (int)Enum.GetValues(typeof(RocketType)).Cast<RocketType>().Max(), AttributeSettingsExclusivityFlag.Jump, kogamaSettingsCollectionBase2);
		kogamaSettingsCollectionBase2.AddChild(kogamaSetting);
		kogamaSettingsCollectionBase2.AddChild(kogamaSetting2);
		kogamaSettingsCollectionBase2.AddChild(kogamaSetting3);
		return kogamaSettingsCollectionBase;
	}
}
