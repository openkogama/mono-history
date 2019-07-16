using System;

namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

public abstract class KogamaSettingValueWrapperBase : KogamaSettingWrapperBase
{
	public abstract IKogamaSetting KogamaSetting { get; }

	public event Action<KogamaSettingValueWrapperBase> OnValueChange;

	protected KogamaSettingValueWrapperBase(string key, KogamaSettingsCollectionBase parent)
		: base(key, parent)
	{
	}

	protected void KogamaSettingOnOnValueChange(IKogamaSetting obj)
	{
		if (OnValueChange != null)
		{
			OnValueChange(this);
		}
	}

	public override string ToString()
	{
		return KogamaSetting.ToString();
	}
}
