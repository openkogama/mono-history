using System.Collections.Generic;

public class MVCameraSettings : MVLogicObject
{
	private bool isPreview;

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
		ICameraSettings settings = MVCameraController.GetSettings(MVGameControllerBase.Game.GameType);
		settings.UpdateFromCameraSettings(Data);
	}

	public override bool IsSingletonObject()
	{
		return true;
	}

	public override void Destroy()
	{
		if (!isPreview)
		{
			ICameraSettings settings = MVCameraController.GetSettings(MVGameControllerBase.Game.GameType);
			settings.SetDefaultSettings();
		}
		base.Destroy();
	}
}
