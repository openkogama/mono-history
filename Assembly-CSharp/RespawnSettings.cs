using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RespawnSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider respawnTimeSlider;

	[SerializeField]
	private SettingsInputFieldSlider respawnTimeInputField;

	[SerializeField]
	private GameObject respawnTimeSettingsUI;

	[SerializeField]
	private GameObject respawnActiveCheckmark;

	[SerializeField]
	private Toggle activeToggle;

	private int woID;

	private bool isInitialized;

	private bool isRespawnActive;

	private const string respawnString = "respawnTime";

	private const int respawnMaxTime = 1800;

	private const int respawnMinTime = 30;

	public void Initialize(int woID, GameObject root)
	{
		this.woID = woID;
		settingsBase.Initialize(woID, root, "Respawn");
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		int value = 30;
		isRespawnActive = data.ContainsKey("respawnTime");
		if (isRespawnActive)
		{
			value = (int)data["respawnTime"];
		}
		respawnTimeSlider.Initialize("respawnTime", value, 30, 1800);
		respawnTimeInputField.Initialize("respawnTime", value);
		SetRespawnUIVisibility(isRespawnActive);
		isInitialized = true;
	}

	public void OnSettingChanged(string key, object value)
	{
		if (key == "respawnTime")
		{
			int num = Mathf.FloorToInt((float)value);
			respawnTimeInputField.SetText(num.ToString());
			settingsBase.OnSettingChanged("respawnTime", num);
		}
	}

	public void HandleActiveToggle()
	{
		if (isInitialized)
		{
			if (isRespawnActive)
			{
				RemoveRespawnTime();
				isRespawnActive = false;
			}
			else
			{
				AddRespawnTime();
				isRespawnActive = true;
			}
		}
	}

	private void AddRespawnTime()
	{
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		if (!data.ContainsKey("respawnTime"))
		{
			data.Add("respawnTime", 30);
		}
		settingsBase.OnSettingChanged("respawnTime", 30);
		SetRespawnUIVisibility(isRespawnActive: true);
	}

	private void RemoveRespawnTime()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("respawnTime", 0);
		MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(woID, dictionary);
		settingsBase.RemoveData("respawnTime");
		SetRespawnUIVisibility(isRespawnActive: false);
	}

	private void SetRespawnUIVisibility(bool isRespawnActive)
	{
		respawnTimeSettingsUI.gameObject.SetActive(isRespawnActive);
		respawnActiveCheckmark.gameObject.SetActive(isRespawnActive);
		activeToggle.isOn = isRespawnActive;
	}
}
