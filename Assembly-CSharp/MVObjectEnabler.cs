using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;

public class MVObjectEnabler : MVLogicObject
{
	private const string prefabPath = "Prefabs/Logic/ObjectEnabler";

	private ObjectEnabler goObjectEnabler;

	private bool showingOutline = true;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public override bool HasObjectConnector => true;

	public bool ShowingOutline => showingOutline;

	public MVObjectEnabler(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Logic/ObjectEnabler", worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		goObjectEnabler = GameObject.GetComponentInChildren<ObjectEnabler>();
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
	}

	public override void PlayModeInitialize()
	{
		Initialize();
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
		if (MVGameController.Instance.Game.GameMode == MVGameMode.Edit && (MVGameController.Instance.EditController == null || !MVGameController.Instance.EditController.PlayInEditor))
		{
			flag = true;
		}
		foreach (ObjectLink objectLinkRef in ObjectLinkRefs)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(objectLinkRef.objectWOID);
			if (worldObjectClient is MVCubeModelBase)
			{
				(worldObjectClient as MVCubeModelBase).Enable(flag);
			}
			else
			{
				MVGameController.Instance.WOCM.GetWorldObjectClient(objectLinkRef.objectWOID).Visible = flag;
			}
		}
	}
}
