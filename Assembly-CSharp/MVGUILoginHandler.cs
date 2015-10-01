using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVGUILoginHandler : UXViewScript
{
	public enum DevServerTarget
	{
		Dev,
		Test,
		Local
	}

	public UXTextButton buildButton;

	public UXTextButton playButton;

	public UXTextButton CEButton;

	public UXToggleIconButton embeddedGameToggleButton;

	public UXComboBox devServerTargetCombobox;

	public UXTextField serverTextField;

	public UXTextField profileIdTextField;

	public UXTextField planetIdTextField;

	private Dictionary<string, object> gameSessionData = new Dictionary<string, object>
	{
		{ "newToken", "clientDefinedNewToken." },
		{ "newPlanetName", "game-server-test" },
		{ "isSoftLaunch", false }
	};

	private bool first = true;

	private string _profileId;

	private string _serverip;

	private string _planetId;

	private static string GetIPFromDevServerTarget(DevServerTarget devTarget)
	{
		return devTarget switch
		{
			DevServerTarget.Local => "127.0.0.1:5055", 
			DevServerTarget.Dev => "95.211.162.200:5055", 
			DevServerTarget.Test => "37.48.122.14:5055", 
			_ => string.Empty, 
		};
	}

	private void InitializeListeners()
	{
		UXTextButton uXTextButton = buildButton;
		uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, new UXBaseButton.OnClickDelegate(EditIslandOnClick));
		UXTextButton uXTextButton2 = playButton;
		uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, new UXBaseButton.OnClickDelegate(JoinIslandOnClick));
		UXTextButton cEButton = CEButton;
		cEButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(cEButton.OnClick, new UXBaseButton.OnClickDelegate(CEOnClick));
		gameSessionData["embedded"] = embeddedGameToggleButton.ToggleState;
		UXToggleIconButton uXToggleIconButton = embeddedGameToggleButton;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool value) =>
		{
			gameSessionData["embedded"] = value;
		}));
		UXTextField uXTextField = serverTextField;
		uXTextField.OnValueChanged = (UXTextInputElement.OnValueChangedDelegate)Delegate.Combine(uXTextField.OnValueChanged, (UXTextInputElement.OnValueChangedDelegate)((string value) =>
		{
			gameSessionData["serverIP"] = value;
		}));
		string[] names = Enum.GetNames(typeof(DevServerTarget));
		foreach (string text in names)
		{
			devServerTargetCombobox.Add(text);
		}
		UXComboBox uXComboBox = devServerTargetCombobox;
		uXComboBox.OnComboBoxItemSelect = (UXComboBox.OnComboBoxItemSelectDelegate)Delegate.Combine(uXComboBox.OnComboBoxItemSelect, (UXComboBox.OnComboBoxItemSelectDelegate)((int index) =>
		{
			string iPFromDevServerTarget = GetIPFromDevServerTarget((DevServerTarget)index);
			serverTextField.Text = iPFromDevServerTarget;
			PlayerPrefs.SetInt("serverIndex", index);
		}));
		int currentItem = PlayerPrefs.GetInt("serverIndex", 0);
		devServerTargetCombobox.SetCurrentItem(currentItem);
		profileIdTextField.Text = ((int)gameSessionData["profileID"]).ToString();
		UXTextField uXTextField2 = profileIdTextField;
		uXTextField2.OnValueChanged = (UXTextInputElement.OnValueChangedDelegate)Delegate.Combine(uXTextField2.OnValueChanged, (UXTextInputElement.OnValueChangedDelegate)((string value) =>
		{
			if (int.TryParse(value, out var result))
			{
				gameSessionData["profileID"] = result;
			}
			gameSessionData["token"] = value;
		}));
		planetIdTextField.Text = ((int)gameSessionData["planetID"]).ToString();
		UXTextField uXTextField3 = planetIdTextField;
		uXTextField3.OnValueChanged = (UXTextInputElement.OnValueChangedDelegate)Delegate.Combine(uXTextField3.OnValueChanged, (UXTextInputElement.OnValueChangedDelegate)((string value) =>
		{
			if (int.TryParse(value, out var result))
			{
				gameSessionData["planetID"] = Convert.ToInt32(result);
			}
		}));
	}

	public override void OnShow()
	{
		base.OnShow();
		if (first)
		{
			SetValuesToPrefOrDefault();
			InitializeListeners();
			first = false;
		}
	}

	public void Update()
	{
		if (View.isVisible)
		{
			UpdatePrefValuesIfChanged();
		}
	}

	private void JoinIslandOnClick()
	{
		StartGame(MVGameMode.Play);
	}

	private void EditIslandOnClick()
	{
		StartGame(MVGameMode.Edit);
	}

	private void CEOnClick()
	{
		StartGame(MVGameMode.CharacterEditor);
	}

	private void StartGame(MVGameMode gameMode)
	{
		gameSessionData["gameMode"] = gameMode;
		gameSessionData["language"] = "en_US";
		MVGameController.StartGame(new GameSessionData(gameSessionData));
		View.Hide();
	}

	private void SetValuesToPrefOrDefault()
	{
		gameSessionData["profileID"] = Convert.ToInt32(GetPrefOrDefault("Dev_profileId", "-1"));
		gameSessionData["token"] = GetPrefOrDefault("Dev_profileId", "-1");
		gameSessionData["serverIP"] = GetPrefOrDefault("Dev_serverip", string.Empty);
		gameSessionData["planetID"] = Convert.ToInt32(GetPrefOrDefault("Dev_planetId", "-1"));
	}

	private void UpdatePrefValuesIfChanged()
	{
		_profileId = UpdateIfChanged("Dev_profileId", _profileId, ((int)gameSessionData["profileID"]).ToString());
		_serverip = UpdateIfChanged("Dev_serverip", _serverip, (string)gameSessionData["serverIP"]);
		_planetId = UpdateIfChanged("Dev_planetId", _planetId, ((int)gameSessionData["planetID"]).ToString());
	}

	private string GetPrefOrDefault(string prefName, string target)
	{
		string text = PlayerPrefs.GetString(prefName, string.Empty);
		return (!(text != string.Empty)) ? target : text;
	}

	private string UpdateIfChanged(string prefName, string current, string target)
	{
		if (current != target)
		{
			current = target;
			PlayerPrefs.SetString(prefName, current);
		}
		return current;
	}
}
