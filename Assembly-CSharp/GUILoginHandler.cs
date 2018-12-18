using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MV.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUILoginHandler : MonoBehaviour
{
	private struct PlanetData(int planet, int profile)
	{
		public int planetID = planet;

		public int profileID = profile;
	}

	private enum DevServerTarget
	{
		Dev,
		Test,
		Local
	}

	[SerializeField]
	private KoGaMaSettingsContainer kogamaSettings;

	[SerializeField]
	private Dropdown serverDropdown;

	[SerializeField]
	private InputField serverIp;

	[SerializeField]
	private InputField profileIdTextField;

	[SerializeField]
	private InputField planetIdTextField;

	[SerializeField]
	private Toggle disableCacheToggle;

	[SerializeField]
	private Dropdown planetDropdown;

	[SerializeField]
	private Dropdown profileDropdown;

	[SerializeField]
	private Button buildButton;

	[SerializeField]
	private Button avatarEditButton;

	private string _profileId;

	private string _serverip;

	private string _planetId;

	private Dictionary<string, PlanetData> defaultPlanetData = new Dictionary<string, PlanetData>
	{
		{
			"WOTest",
			new PlanetData(48887, 1)
		},
		{
			"Default",
			new PlanetData(48888, 13)
		}
	};

	private Dictionary<string, object> gameSessionData = new Dictionary<string, object>
	{
		{ "newToken", "clientDefinedNewToken." },
		{ "newPlanetName", "game-server-test" },
		{ "isSoftLaunch", false },
		{ "embedded", false }
	};

	private string playerPrefKey = "Dev_profileId";

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

	protected void Awake()
	{
		playerPrefKey = "Dev_profileId_standalone";
		SetValuesToPrefOrDefault();
		int num = PlayerPrefs.GetInt("serverIndex", 0);
		SetupServerDropdown(num);
		serverIp.text = GetIPFromDevServerTarget((DevServerTarget)num);
		SetupProfileID();
		SetupPlanetID();
		disableCacheToggle.isOn = PlayerPrefs.GetInt("cachingEnabled") <= 0;
	}

	private void Start()
	{
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

	public void OnProfileDropdownChanged()
	{
		Dropdown.OptionData optionData = profileDropdown.options[profileDropdown.value];
		string text = Regex.Replace(optionData.text, "[^0-9]+", string.Empty);
		profileIdTextField.text = text;
	}

	public void OnPlanetDropdownChanged()
	{
		Dropdown.OptionData optionData = planetDropdown.options[planetDropdown.value];
		if (defaultPlanetData.ContainsKey(optionData.text))
		{
			planetIdTextField.text = defaultPlanetData[optionData.text].planetID.ToString();
			profileIdTextField.text = defaultPlanetData[optionData.text].profileID.ToString();
		}
	}

	private void SetupPlanetID()
	{
		planetIdTextField.text = ((int)gameSessionData["planetID"]/*cast due to constrained. prefix*/).ToString();
		planetIdTextField.onValueChanged.AddListener((string value) =>
		{
			if (int.TryParse(value, out var result))
			{
				gameSessionData["planetID"] = Convert.ToInt32(result);
			}
		});
	}

	private void SetupProfileID()
	{
		profileIdTextField.text = ((int)gameSessionData["profileID"]/*cast due to constrained. prefix*/).ToString();
		profileIdTextField.onValueChanged.AddListener((string value) =>
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
		Debug.Log("ServerIP " + iPFromDevServerTarget);
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

	public void Caching()
	{
		PlayerPrefs.SetInt("cachingEnabled", (!disableCacheToggle.isOn) ? 1 : 0);
	}

	private void StartGame(MVGameMode gameMode)
	{
		this.gameSessionData["gameMode"] = gameMode;
		this.gameSessionData["language"] = "en_US";
		this.gameSessionData["referrer"] = "gsm";
		GameSessionData gameSessionData = new GameSessionData(this.gameSessionData);
		StringBuilder stringBuilder = new StringBuilder(256);
		stringBuilder.Append("GameSessionData:\n");
		foreach (KeyValuePair<string, object> gameSessionDatum in this.gameSessionData)
		{
			stringBuilder.AppendFormat("{0} - {1}\n", gameSessionDatum.Key, gameSessionDatum.Value);
		}
		Debug.Log(stringBuilder);
		MVGameControllerBase.SetGameSessionData(gameSessionData);
		gameObject.SetActive(value: false);
		SceneManager.LoadScene("DesktopBase");
		if (disableCacheToggle.isOn)
		{
			Debug.LogWarning("Cache disabled.");
			kogamaSettings.InvalidateStreamingAssetsCache();
		}
	}

	private void SetValuesToPrefOrDefault()
	{
		gameSessionData["profileID"] = Convert.ToInt32(GetPrefOrDefault(playerPrefKey, "-1"));
		gameSessionData["token"] = GetPrefOrDefault(playerPrefKey, "-1");
		gameSessionData["serverIP"] = GetPrefOrDefault("Dev_serverip", string.Empty);
		gameSessionData["planetID"] = Convert.ToInt32(GetPrefOrDefault("Dev_planetId", "-1"));
	}

	private void UpdatePrefValuesIfChanged()
	{
		_profileId = UpdateIfChanged(playerPrefKey, _profileId, ((int)gameSessionData["profileID"]/*cast due to constrained. prefix*/).ToString());
		_serverip = UpdateIfChanged("Dev_serverip", _serverip, (string)gameSessionData["serverIP"]);
		_planetId = UpdateIfChanged("Dev_planetId", _planetId, ((int)gameSessionData["planetID"]/*cast due to constrained. prefix*/).ToString());
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
