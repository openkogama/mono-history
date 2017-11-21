using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotatorSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider slider;

	[SerializeField]
	private SettingsInputFieldSlider inputField;

	private Dictionary<object, object> blueprintData;

	private int woID;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, TM._("Rotator"));
		if (woID == -1)
		{
			throw new NotImplementedException();
		}
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		this.woID = woID;
		blueprintData = (Dictionary<object, object>)data["BlueprintData"];
		float value = (float)blueprintData["AngularSpeed"];
		slider.Initialize("BlueprintData\\AngularSpeed", value, 0f, 5f);
		inputField.Initialize("BlueprintData\\AngularSpeed", value);
	}

	public void OnSettingChanged(string key, object value)
	{
		if (key != null)
		{
			MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(woID, key, Convert.ToSingle(value));
		}
	}
}
