using System;
using System.Collections.Generic;
using MV.WorldObject;

public class MVObjectEnabler : MVLogicObject, ILogicWorldObject
{
	private bool isInitialized;

	private ObjectEnabler goObjectEnabler;

	private bool showingOutline = true;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.ModelToggle;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public override bool HasObjectConnector => true;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public bool ShowingOutline => showingOutline;

	public MVObjectEnabler(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVObjectEnablerPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
		interactionFlags |= InteractionFlags.HasSettings;
		goObjectEnabler = ((MVObjectEnablerObject)component).ObjectEnabler;
		goObjectEnabler.woObjectEnabler = this;
	}

	public override bool ValidateObjectLinkTarget(MVWorldObjectClient wo)
	{
		return wo is MVCubeModelInstance;
	}

	public override void Initialize()
	{
		base.Initialize();
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, InputStateUpdateCallback);
		OnDataUpdate();
		UpdateShowObjects();
		SetupCulling(goObjectEnabler.gameObject);
		goObjectEnabler.Initialize();
		isInitialized = true;
		if (MVGameControllerBase.EditModeUI != null)
		{
			IEditModeUI editModeUI = MVGameControllerBase.EditModeUI;
			editModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Combine(editModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
	}

	private void InputStateUpdateCallback(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState == LogicInputState.FromColdToHot || logicInputState == LogicInputState.FromHotToCold)
		{
			UpdateShowObjects();
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		if (MVGameControllerBase.EditModeUI != null)
		{
			IEditModeUI editModeUI = MVGameControllerBase.EditModeUI;
			editModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Remove(editModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
	}

	private void OnEditModeChange(EditModeChangeArgs arg)
	{
		UpdateShowObjects();
	}

	public override void PlayModeInitialize()
	{
		UpdateShowObjects();
	}

	public override void Reset()
	{
		UpdateShowObjects();
	}

	private void UpdateShowObjects()
	{
		bool currentlyIsHot = InputSignalReceiver.CurrentlyIsHot;
		ShowObjects(currentlyIsHot);
		goObjectEnabler.IsDrawingEnabled = currentlyIsHot;
	}

	public override void OnObjectLinkChanged()
	{
		if (isInitialized)
		{
			UpdateShowObjects();
		}
	}

	public override void OnDataUpdate()
	{
		showingOutline = (bool)Data["showOutline"];
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
			MVWorldObjectClient worldObjectClient2 = MVGameControllerBase.WOCM.GetWorldObjectClient(worldObjectClient.GroupId);
			if (worldObjectClient2 is MVMovable)
			{
				((MVMovable)worldObjectClient2).Visible = flag;
				(worldObjectClient as MVCubeModelBase).ObjectLinkChanged(flag);
			}
			else if (worldObjectClient is MVCubeModelBase)
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
