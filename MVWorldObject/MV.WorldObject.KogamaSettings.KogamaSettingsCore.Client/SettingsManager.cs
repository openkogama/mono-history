using System.Collections.Generic;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore.Client;

public class SettingsManager
{
	private readonly SettingsReporter settingsReporter;

	public SettingsManager(SettingsReporter settingsReporter)
	{
		this.settingsReporter = settingsReporter;
	}

	public void UpdateSetting(KogamaSettingWrapperBase obj)
	{
		Dictionary<object, object> subTree = KogamaSettingTools.GetSubTree(obj);
		settingsReporter.OnValueChange(subTree);
	}

	public void RemoveSetting(KogamaSettingWrapperBase obj)
	{
		Dictionary<object, object> subTree = KogamaSettingTools.GetSubTree(obj);
		settingsReporter.OnValueRemoved(subTree);
	}

	public void Submit()
	{
		settingsReporter.Submit();
	}
}
