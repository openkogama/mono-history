using System.Collections.Generic;
using MV.WorldObject;

public class MVObjectEnabler : MVLogicObject
{
	private ObjectEnabler goObjectEnabler;

	private bool showingOutline = true;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public override bool HasObjectConnector => true;

	public bool ShowingOutline => showingOutline;

	public MVObjectEnabler(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVObjectEnablerPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		goObjectEnabler = ((MVObjectEnablerObject)component).ObjectEnabler;
		goObjectEnabler.woObjectEnabler = this;
		OnDataUpdate();
	}

	public static void BuildSettingsDialog()
	{
	}

	public override bool ValidateObjectLinkTarget(MVWorldObjectClient wo)
	{
		return wo is MVCubeModelInstance;
	}

	public override void Initialize()
	{
		base.Initialize();
		OnInputStateChanged();
		SetupCulling(gameObject);
	}

	public override void PlayModeInitialize()
	{
		OnInputStateChanged();
	}

	public override void Reset()
	{
		OnInputStateChanged();
	}

	public override void OnInputLinkChanged()
	{
		OnInputStateChanged();
	}

	public override void OnInputStateChanged()
	{
		bool flag = InputLinkRefs.Count == 0 || InputState;
		ShowObjects(flag);
		goObjectEnabler.IsDrawingEnabled = flag;
	}

	public override void OnObjectLinkChanged()
	{
		OnInputStateChanged();
	}

	public override void OnDataUpdate()
	{
		if (Data.ContainsKey("showOutline"))
		{
			showingOutline = (bool)Data["showOutline"];
		}
	}

	private void ShowObjects(bool visible)
	{
		bool flag = visible;
		if (!MVGameControllerBase.Game.IsPlaying && MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			flag = true;
		}
		foreach (ObjectLink objectLinkRef in ObjectLinkRefs)
		{
			objectLinkRef.isSet = flag;
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(objectLinkRef.objectWOID);
			if (worldObjectClient is MVCubeModelBase)
			{
				(worldObjectClient as MVCubeModelBase).ObjectLinkChanged(flag);
			}
			else
			{
				worldObjectClient.Visible = flag;
			}
		}
	}
}
