using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamRequirementSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
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
	private Text buttonTeamBlueName;

	[SerializeField]
	private Text buttonTeamRedName;

	[SerializeField]
	private Text buttonTeamGreenName;

	[SerializeField]
	private Text buttonTeamYellowName;

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
		settingsBase.Initialize(woID, root, TM._("Team Requirement"));
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		if (!data.ContainsKey("team"))
		{
			data["team"] = Convert.ToInt32(MVTeam.None);
		}
		currentOutline = outlineTeamNone;
		int num = Convert.ToInt32(data["team"]);
		OnSettingChanged("team", num);
		Dictionary<MVTeam, string> teamNames = MVGameControllerBase.Game.TeamManager.GetTeamNames();
		buttonTeamBlueName.text = teamNames[MVTeam.Blue];
		buttonTeamRedName.text = teamNames[MVTeam.Red];
		buttonTeamGreenName.text = teamNames[MVTeam.Green];
		buttonTeamYellowName.text = teamNames[MVTeam.Yellow];
		buttonTeamNone.Initialize("team", Convert.ToInt32(MVTeam.None));
		buttonTeamBlue.Initialize("team", Convert.ToInt32(MVTeam.Blue));
		buttonTeamRed.Initialize("team", Convert.ToInt32(MVTeam.Red));
		buttonTeamGreen.Initialize("team", Convert.ToInt32(MVTeam.Green));
		buttonTeamYellow.Initialize("team", Convert.ToInt32(MVTeam.Yellow));
	}

	public void OnSettingChanged(string key, object value)
	{
		currentOutline.enabled = false;
		switch ((MVTeam)value)
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
