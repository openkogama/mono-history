using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public class TeamEditorSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsInputField blueTeam;

	[SerializeField]
	private SettingsInputField redTeam;

	[SerializeField]
	private SettingsInputField greenTeam;

	[SerializeField]
	private SettingsInputField yellowTeam;

	[SerializeField]
	private GameObject blueTeamButton;

	[SerializeField]
	private GameObject redTeamButton;

	[SerializeField]
	private GameObject greenTeamButton;

	[SerializeField]
	private GameObject yellowTeamButton;

	private int woID = -1;

	private Dictionary<object, object> teamData;

	public void Initialize(int woID, GameObject root)
	{
		this.woID = woID;
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.TeamEditor);
		teamData = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		InitInputfield(blueTeam, blueTeamButton, MVTeam.Blue);
		InitInputfield(redTeam, redTeamButton, MVTeam.Red);
		InitInputfield(greenTeam, greenTeamButton, MVTeam.Green);
		InitInputfield(yellowTeam, yellowTeamButton, MVTeam.Yellow);
		SetActiveButtons(teamData);
		teamData = null;
	}

	private void InitInputfield(SettingsInputField inputField, GameObject button, MVTeam team)
	{
		string key = team.ToString();
		if (teamData.ContainsKey(key))
		{
			inputField.Initialize(key, (string)teamData[key]);
		}
		else
		{
			inputField.Initialize(key, string.Empty);
		}
	}

	public void OnSettingChanged(string key, object value)
	{
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		if (string.IsNullOrEmpty((string)value) || ((string)value).Trim().Length == 0)
		{
			data.Remove(key);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add(key, string.Empty);
			MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(woID, dictionary);
			MVGameControllerBase.Game.TeamManager.SetTeamNameToDefault((MVTeam)Enum.Parse(typeof(MVTeam), key, ignoreCase: true));
			SetActiveButtons(data);
			settingsBase.RemoveData(key);
		}
		else
		{
			data[key] = (string)value;
			SetActiveButtons(data);
			settingsBase.OnSettingChanged(key, value);
		}
	}

	private void SetActiveButtons(Dictionary<object, object> teamData)
	{
		blueTeamButton.SetActive(teamData.ContainsKey("Blue"));
		redTeamButton.SetActive(teamData.ContainsKey("Red"));
		greenTeamButton.SetActive(teamData.ContainsKey("Green"));
		yellowTeamButton.SetActive(teamData.ContainsKey("Yellow"));
	}
}
