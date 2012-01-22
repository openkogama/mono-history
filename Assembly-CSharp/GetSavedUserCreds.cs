using UnityEngine;

public class GetSavedUserCreds : MonoBehaviour
{
	public MVGameController controller;

	private string _username;

	private string _password;

	private string _serverip;

	private string _planet;

	private void Start()
	{
		controller.username = GetPrefOrDefault("Dev_username", controller.username);
		controller.password = GetPrefOrDefault("Dev_password", controller.password);
		controller.ip = GetPrefOrDefault("Dev_serverip", controller.ip);
		controller.planetName = GetPrefOrDefault("Dev_planet", controller.planetName);
	}

	private void Update()
	{
		_username = UpdateIfChanged("Dev_username", _username, controller.username);
		_password = UpdateIfChanged("Dev_password", _password, controller.password);
		_serverip = UpdateIfChanged("Dev_serverip", _serverip, controller.ip);
		_planet = UpdateIfChanged("Dev_planet", _planet, controller.planetName);
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
