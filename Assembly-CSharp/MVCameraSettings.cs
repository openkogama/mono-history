using System;
using System.Collections.Generic;

public class MVCameraSettings : MVLogicObject
{
	private bool isPreview;

	private bool needToUnsubscribeToSettingsCallback;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.CameraSettings;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVCameraSettings(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVCameraSettingsPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags &= ~InteractionFlags.CanClone;
	}

	public override void Initialize()
	{
		base.Initialize();
		OnDataUpdate();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		isPreview = true;
	}

	public override void OnDataUpdate()
	{
		if (MainCameraManager.HasSetting(MVGameControllerBase.Game.GameType))
		{
			ICameraSettings settings = MainCameraManager.GetSettings(MVGameControllerBase.Game.GameType);
			settings.UpdateFromCameraSettings(Data);
		}
		else
		{
			MainCameraManager.OnCameraSettingAdded = (Action)Delegate.Combine(MainCameraManager.OnCameraSettingAdded, new Action(OnCameraSettingAdded));
			needToUnsubscribeToSettingsCallback = true;
		}
	}

	public override bool IsSingletonObject()
	{
		return true;
	}

	public override void Destroy()
	{
		if (!isPreview && MainCameraManager.HasSetting(MVGameControllerBase.Game.GameType))
		{
			ICameraSettings settings = MainCameraManager.GetSettings(MVGameControllerBase.Game.GameType);
			settings.SetDefaultSettings();
		}
		if (needToUnsubscribeToSettingsCallback)
		{
			MainCameraManager.OnCameraSettingAdded = (Action)Delegate.Remove(MainCameraManager.OnCameraSettingAdded, new Action(OnCameraSettingAdded));
		}
		base.Destroy();
	}

	private void OnCameraSettingAdded()
	{
		if (MainCameraManager.HasSetting(MVGameControllerBase.Game.GameType))
		{
			ICameraSettings settings = MainCameraManager.GetSettings(MVGameControllerBase.Game.GameType);
			settings.UpdateFromCameraSettings(Data);
			needToUnsubscribeToSettingsCallback = false;
		}
	}
}
