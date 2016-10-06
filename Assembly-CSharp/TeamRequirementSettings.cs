using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamRequirementSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsButton buttonTeamNone;

	[SerializeField]
	private SettingsButton buttonTeamBlue;

	[SerializeField]
	private SettingsButton buttonTeamRed;

	[SerializeField]
	private SettingsButton buttonTeamGreen;

	[SerializeField]
	private SettingsButton buttonTeamYellow;

	[SerializeField]
	private Outline outlineTeamNone;

	[SerializeField]
	private Outline outlineTeamRed;

	[SerializeField]
	private Outline outlineTeamBlue;

	[SerializeField]
	private Outline outlineTeamGreen;

	[SerializeField]
	private Outline outlineTeamYellow;

	private Outline currentOutline;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("team", Convert.ToInt32(MVTeam.None));
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		if (!dictionary2.ContainsKey("team"))
		{
			dictionary2["team"] = Convert.ToInt32(MVTeam.None);
		}
		currentOutline = outlineTeamNone;
		int num = Convert.ToInt32(dictionary2["team"]);
		OnSettingChanged("team", num);
		buttonTeamNone.Initialize("team", Convert.ToInt32(MVTeam.None));
		buttonTeamBlue.Initialize("team", Convert.ToInt32(MVTeam.Blue));
		buttonTeamRed.Initialize("team", Convert.ToInt32(MVTeam.Red));
		buttonTeamGreen.Initialize("team", Convert.ToInt32(MVTeam.Green));
		buttonTeamYellow.Initialize("team", Convert.ToInt32(MVTeam.Yellow));
	}

	public void OnSettingChanged(string key, object value)
	{
		currentOutline.enabled = false;
		switch ((MVTeam)(int)value)
		{
		case MVTeam.Blue:
			currentOutline = outlineTeamBlue;
			break;
		case MVTeam.Red:
			currentOutline = outlineTeamRed;
			break;
		case MVTeam.Green:
			currentOutline = outlineTeamGreen;
			break;
		case MVTeam.Yellow:
			currentOutline = outlineTeamYellow;
			break;
		default:
			currentOutline = outlineTeamNone;
			break;
		}
		currentOutline.enabled = true;
		settingsBase.OnSettingChanged(key, Convert.ToInt32(value));
	}
}
