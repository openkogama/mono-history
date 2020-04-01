using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public class UseLever : MVLogicObject, IIsLogicObjectFiringEventHandler, ILogicWorldObject
{
	private UseLeverObject useLeverObject;

	private float minY = -0.25f;

	private float speed = 1.8f;

	private const string beginActivateValueKey = "beginActivated";

	private const string isActivateValueKey = "a";

	private bool requestSend;

	private bool localIsDown;

	private OutputSignalTransmitter outputSignalTransmitter;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.Lever;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	protected override bool HasVisualsInPlaymode => true;

	public override Vector3 WorldPivot => transform.position;

	public override Vector3 OutputConnectorOffset => new Vector3(1.2f, 0f, 0f);

	private bool BeginActivated => (bool)Data["beginActivated"];

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	private bool IsActivated
	{
		get
		{
			return (ObscuredBool)RunTimeData.GetObscuredType("a");
		}
		set
		{
			RunTimeData.SetObscuredType("a", (ObscuredBool)value);
		}
	}

	public UseLever(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.UseLeverPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		interactionFlags |= InteractionFlags.CanUseStars;
		interactionFlags |= InteractionFlags.CanUseGameRank;
		interactionFlags |= InteractionFlags.CanResetLogic;
		useLeverObject = (UseLeverObject)component;
		useLeverObject.UseInteractor = new UseInteractor(this, useLeverObject.useInteractionRotator, reset: false, useLeverObject.LeverCollider, Use);
		useLeverObject.TriggerBoxEvents.TriggerEnter += useLeverObject.UseInteractor.triggerBoxEvents_TriggerEnter;
		useLeverObject.TriggerBoxEvents.TriggerExit += useLeverObject.UseInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(useLeverObject.useInteractionRotator, new Vector3(0.5f, 1f, 0f));
		useLeverObject.UseInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(useLeverObject.useInteractionRotator);
		useLeverObject.UseInteractor.AddRequirement(useRequirement2);
		StarRequirement useRequirement3 = new StarRequirement(useLeverObject.useInteractionRotator);
		useLeverObject.UseInteractor.AddRequirement(useRequirement3);
		GameRankRequirement useRequirement4 = new GameRankRequirement(useLeverObject.useInteractionRotator, this);
		useLeverObject.UseInteractor.AddRequirement(useRequirement4);
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}

	public override void Initialize()
	{
		base.Initialize();
		if (MVGameControllerBase.EditModeUI != null)
		{
			IEditModeUI editModeUI = MVGameControllerBase.EditModeUI;
			editModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Combine(editModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			OnEditModeChange(new EditModeChangeArgs(state: false));
		}
		useLeverObject.UseInteractor.UpdateData(Data);
		SetupCulling(useLeverObject.VisualRoot);
		InputSignalReceiver = LogicClientsideFactory.CreateInputSignalReceiver(this, defaultInput: true, SignalCallback);
		outputSignalTransmitter = new OutputSignalTransmitter(Id);
		localIsDown = IsActivated;
	}

	private void SignalCallback(bool b, bool wasHot, LogicObjectManager logicObjectManager)
	{
		outputSignalTransmitter.Send(IsActivated);
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (localIsDown)
		{
			if (useLeverObject.PlateButtonTransform.localPosition.z > minY)
			{
				float num = Mathf.Min(speed * Time.smoothDeltaTime, useLeverObject.PlateButtonTransform.localPosition.z - minY);
				useLeverObject.PlateButtonTransform.localPosition = new Vector3(useLeverObject.PlateButtonTransform.localPosition.x, useLeverObject.PlateButtonTransform.localPosition.y, useLeverObject.PlateButtonTransform.localPosition.z - num);
			}
		}
		else if (!localIsDown && useLeverObject.PlateButtonTransform.localPosition.z < 0f)
		{
			float num2 = Mathf.Min(speed * Time.smoothDeltaTime, 0f - useLeverObject.PlateButtonTransform.localPosition.z);
			useLeverObject.PlateButtonTransform.localPosition = new Vector3(useLeverObject.PlateButtonTransform.localPosition.x, useLeverObject.PlateButtonTransform.localPosition.y, useLeverObject.PlateButtonTransform.localPosition.z + num2);
		}
	}

	public bool Use(int userWoID)
	{
		if (requestSend)
		{
			return true;
		}
		requestSend = true;
		MVGameControllerBase.OperationRequests.LogicActivateRequest(Id, !IsActivated);
		localIsDown = !IsActivated;
		return true;
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		useLeverObject.EditCollider.gameObject.SetActive(value: false);
	}

	public override void SetupTierInventory()
	{
		useLeverObject.EditCollider.gameObject.SetActive(value: false);
		base.SetupTierInventory();
	}

	public override void UnSetupTierInventory()
	{
		useLeverObject.EditCollider.gameObject.SetActive(value: true);
		base.UnSetupTierInventory();
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(Vector3.zero, new Vector3(1.2f, 1.2f, 0.2f));
	}

	public override void Reset()
	{
		IsActivated = BeginActivated;
		requestSend = false;
		localIsDown = IsActivated;
	}

	public override void OnDataUpdate()
	{
		useLeverObject.UseInteractor.UpdateData(Data);
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	public override void Destroy()
	{
		if (MVGameControllerBase.EditModeUI != null)
		{
			IEditModeUI editModeUI = MVGameControllerBase.EditModeUI;
			editModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Remove(editModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		useLeverObject.TriggerBoxEvents.TriggerEnter -= useLeverObject.UseInteractor.triggerBoxEvents_TriggerEnter;
		useLeverObject.TriggerBoxEvents.TriggerExit -= useLeverObject.UseInteractor.triggerBoxEvents_TriggerExit;
		useLeverObject.UseInteractor.OnDestroy(Data);
		base.Destroy();
	}

	public void OnEditModeChange(EditModeChangeArgs arg)
	{
		useLeverObject.EditCollider.enabled = true;
		useLeverObject.LeverCollider.enabled = false;
		if (arg.playInEditor)
		{
			useLeverObject.EditCollider.enabled = false;
			useLeverObject.LeverCollider.enabled = true;
		}
	}

	public void OnIsFiringChanged(bool isFiring)
	{
		IsActivated = isFiring;
		requestSend = false;
		localIsDown = isFiring;
	}
}
