using System.Collections.Generic;

public class MVCameraSettings : MVLogicObject
{
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

	public override void OnDataUpdate()
	{
		ICameraSettings settings = MVCameraController.GetSettings(MVGameControllerBase.Game.GameType);
		settings.UpdateFromCameraSettings(Data);
	}

	public override bool IsSingletonObject()
	{
		return true;
	}

	public override void Destroy()
	{
		ICameraSettings settings = MVCameraController.GetSettings(MVGameControllerBase.Game.GameType);
		settings.SetDefaultSettings();
		base.Destroy();
	}
}
