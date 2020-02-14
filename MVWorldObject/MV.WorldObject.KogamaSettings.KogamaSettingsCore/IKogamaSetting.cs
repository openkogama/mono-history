using System;

namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore;

public interface IKogamaSetting
{
	object Value { get; set; }

	KogamaSettingValueType KogamaSettingValueType { get; }

	event Action<IKogamaSetting> OnValueChange;
}
