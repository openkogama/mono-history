using System;

namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore;

public abstract class KogamaSettingBase : IKogamaSetting
{
	private object value;

	public object Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
			if (OnValueChange != null)
			{
				OnValueChange(this);
			}
		}
	}

	public abstract KogamaSettingValueType KogamaSettingValueType { get; }

	public event Action<IKogamaSetting> OnValueChange;
}
