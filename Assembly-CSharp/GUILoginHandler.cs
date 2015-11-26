using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class GUILoginHandler : MonoBehaviour
{
	public enum DevServerTarget
	{
		Dev,
		Test,
		Local
	}

	[SerializeField]
	private Dropdown serverDropdown;

	[SerializeField]
	private InputField serverIp;

	[SerializeField]
	private InputField profileIdTextField;

	[SerializeField]
	private InputField planetIdTextField;

	private Dictionary<string, object> gameSessionData = new Dictionary<string, object>
	{
		{ "newToken", "clientDefinedNewToken." },
		{ "newPlanetName", "game-server-test" },
		{ "isSoftLaunch", false },
		{ "embedded", false }
	};

	private string _profileId;

	private string _serverip;

	private string _planetId;

	private static string GetIPFromDevServerTarget(DevServerTarget devTarget)
	{
		return devTarget switch
		{
			DevServerTarget.Local => "127.0.0.1:5055", 
			DevServerTarget.Dev => "95.211.162.200:5055", 
			DevServerTarget.Test => "85.17.123.71:5055", 
			_ => string.Empty, 
		};
	}

	private void Awake()
	{
		SetValuesToPrefOrDefault();
		int num = PlayerPrefs.GetInt("serverIndex", 0);
		SetupServerDropdown(num);
		serverIp.text = GetIPFromDevServerTarget((DevServerTarget)num);
		SetupProfileID();
		SetupPlanetID();
	}

	private void SetupServerDropdown(int savedChoice)
	{
		string[] names = Enum.GetNames(typeof(DevServerTarget));
		foreach (string text in names)
		{
			serverDropdown.options.Add(new Dropdown.OptionData(text));
		}
		serverDropdown.value = savedChoice;
		string text2 = Enum.GetNames(typeof(DevServerTarget))[savedChoice];
		serverDropdown.captionText.text = text2;
		ComboboxChanged(savedChoice);
	}

	private void SetupPlanetID()
	{
		planetIdTextField.text = ((int)gameSessionData["planetID"]).ToString();
		planetIdTextField.onValueChange.AddListener((string value) =>
		{
			if (int.TryParse(value, out var result))
			{
				gameSessionData["planetID"] = Convert.ToInt32(result);
			}
		});
	}

	private void SetupProfileID()
	{
		profileIdTextField.text = ((int)gameSessionData["profileID"]).ToString();
		profileIdTextField.onValueChange.AddListener((string value) =>
		{
			if (int.TryParse(value, out var result))
			{
				gameSessionData["profileID"] = result;
			}
			gameSessionData["token"] = value;
		});
	}

	private void Update()
	{
		UpdatePrefValuesIfChanged();
	}

	public void ComboboxChanged(int index)
	{
		string iPFromDevServerTarget = GetIPFromDevServerTarget((DevServerTarget)index);
		serverIp.text = iPFromDevServerTarget;
		PlayerPrefs.SetInt("serverIndex", index);
		gameSessionData["serverIP"] = iPFromDevServerTarget;
	}

	public void UIDropdownChanged(int index)
	{
		PlayerPrefs.SetInt("uiIndex", index);
	}

	public void EnterPlayOnClick()
	{
		StartGame(MVGameMode.Play);
	}

	public void EnterEditOnClick()
	{
		StartGame(MVGameMode.Edit);
	}

	public void EnterCharacterEditOnClick()
	{
		StartGame(MVGameMode.CharacterEditor);
	}

	public void Embedded(bool isEmbedded)
	{
		gameSessionData["embedded"] = isEmbedded;
	}

	private void StartGame(MVGameMode gameMode)
	{
		gameSessionData["gameMode"] = gameMode;
		gameSessionData["language"] = "en_US";
		GameSessionData message = new GameSessionData(gameSessionData);
		MVGameControllerBase.SetGameSessionData(message);
		gameObject.SetActive(value: false);
		Debug.Log(message);
		Application.LoadLevel("BaseLegacyUI");
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
