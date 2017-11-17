using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovablesSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider slider;

	[SerializeField]
	private SettingsInputFieldSlider inputField;

	private Dictionary<object, object> blueprintData;

	private MVMovingPlatformGroup platformGroup;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.MovingPlatform);
		if (woID == -1)
		{
			Debug.LogError("Wo not found");
			return;
		}
		platformGroup = MVGameControllerBase.WOCM.GetWorldObjectClient(woID) as MVMovingPlatformGroup;
		MVMovingPlatform platform = platformGroup.Platform;
		float magnitude = platform.Velocity.magnitude;
		slider.Initialize("BlueprintData\\Velocity", magnitude, 0.3f, 3f);
		inputField.Initialize("BlueprintData\\Velocity", magnitude);
	}

	public void OnSettingChanged(string key, object value)
	{
		if (key != null)
		{
			float num = Convert.ToSingle(value);
			MVMovingPlatform platform = platformGroup.Platform;
			Vector3 normalized = platform.Velocity.normalized;
			float num2 = num;
			Vector3 vec = normalized * num2;
			MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(platform.Id, key, vec.ToSerializeString());
		}
	}
}
