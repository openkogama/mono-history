using System.Collections.Generic;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using UnityEngine;

public class GameSetupOptions : MonoBehaviour
{
	[SerializeField]
	private ToggleButtonAnimation reviveToggleButton;

	private static bool isReviveEnabled = true;

	public static bool IsReviveEnabled
	{
		get
		{
			MVGameOptionDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameOptionDataObject>();
			List<KogamaSettingValueWrapperBase> getOptions = singletonWorldObject.GameOptionSettingsManager.GetOptions;
			foreach (KogamaSettingValueWrapperBase item in getOptions)
			{
				if (item is KogamaSettingBoolBase)
				{
					KogamaSettingBoolBase kogamaSettingBoolBase = (KogamaSettingBoolBase)item;
					if (kogamaSettingBoolBase.Key == "AllowRevive")
					{
						isReviveEnabled = kogamaSettingBoolBase.ValueBool;
						return isReviveEnabled;
					}
				}
			}
			return isReviveEnabled;
		}
	}

	private void Start()
	{
		isReviveEnabled = IsReviveEnabled;
		if (isReviveEnabled)
		{
			reviveToggleButton.ToggleOn();
		}
		else
		{
			reviveToggleButton.ToggleOff();
		}
	}

	public void ToggleRevive()
	{
		isReviveEnabled = !isReviveEnabled;
		MVGameOptionDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameOptionDataObject>();
		List<KogamaSettingValueWrapperBase> getOptions = singletonWorldObject.GameOptionSettingsManager.GetOptions;
		foreach (KogamaSettingValueWrapperBase item in getOptions)
		{
			Debug.Log(item);
			if (item is KogamaSettingBoolBase)
			{
				KogamaSettingBoolBase kogamaSettingBoolBase = (KogamaSettingBoolBase)item;
				if (kogamaSettingBoolBase.Key == "AllowRevive")
				{
					kogamaSettingBoolBase.ValueBool = isReviveEnabled;
					singletonWorldObject.UpdateSetting(kogamaSettingBoolBase);
					break;
				}
			}
		}
		singletonWorldObject.Submit();
		Debug.Log("Revive enabled: " + isReviveEnabled);
	}
}
