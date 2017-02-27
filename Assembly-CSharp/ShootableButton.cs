using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public class ShootableButton : MVLogicObject, ILogicWorldObject, IIsLogicObjectFiringEventHandler
{
	private const string durationValueKey = "duration";

	private const string currentTimeValueKey = "cT";

	private const int currentTimeDefaultValue = -1;

	private LogicInteractable interactable;

	private Collider targetCollider;

	private ShootableButtonObject buttonObject;

	private OutputSignalTransmitter outputSignalTransmitter;

	public override Vector3 WorldPivot => transform.position;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public override Vector3 OutputConnectorOffset => new Vector3(1.6f, 0f, 0f);

	private int Duration => (int)((float)Data["duration"] * 1000f);

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	private int CurrentTime
	{
		get
		{
			return (ObscuredInt)RunTimeData.GetObscuredType("cT");
		}
		set
		{
			RunTimeData.SetObscuredType("cT", (ObscuredInt)value);
		}
	}

	private bool IsActive
	{
		get
		{
			return CurrentTime != -1;
		}
		set
		{
			if (value)
			{
				CurrentTime = 0;
			}
			else
			{
				CurrentTime = -1;
			}
		}
	}

	public ShootableButton(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.ShootableButtonPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		PlayInteractionType = PlayInteractionType.HandlesHits;
		buttonObject = (ShootableButtonObject)component;
	}

	public override void Initialize()
	{
		base.Initialize();
		interactable = gameObject.AddComponent<LogicInteractable>();
		gameObject.AddComponent<ClientSideLogicInteractionHandler>();
		interactable.OnDamageEvent += Activate;
		if (MVGameControllerBase.IEditModeUI != null)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Combine(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			buttonObject.TargetCollider3D.enabled = false;
			targetCollider = buttonObject.TargetCollider2D;
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 90f, transform.localEulerAngles.z);
		}
		else
		{
			buttonObject.TargetCollider2D.enabled = false;
			targetCollider = buttonObject.TargetCollider3D;
		}
		collider = targetCollider;
		SetupCulling(buttonObject.VisualRoot);
		InputSignalReceiver = LogicClientsideFactory.CreateInputSignalReceiver(this, defaultInput: false, SignalCallback);
		outputSignalTransmitter = new OutputSignalTransmitter(Id);
		if (IsActive)
		{
			SetToDownState();
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		buttonObject.EditCollider.gameObject.SetActive(value: false);
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 1.5f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}

	private void SignalCallback(bool b, bool wasHot, LogicObjectManager logicObjectManager)
	{
		if (!IsActive)
		{
			outputSignalTransmitter.Send(isHot: false);
		}
		else if (CurrentTime > Duration)
		{
			outputSignalTransmitter.Send(isHot: false);
			CurrentTime = -1;
			SetToUpState();
		}
		else
		{
			outputSignalTransmitter.Send(isHot: true);
			CurrentTime += 100;
		}
	}

	public void OnIsFiringChanged(bool isFiring)
	{
		IsActive = isFiring;
		SetToDownState();
	}

	public override void OnDataUpdate()
	{
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	public override void Reset()
	{
		CurrentTime = -1;
		SetToUpState();
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(Vector3.zero, new Vector3(2.001f, 2.001f, 0.701f));
	}

	public void Activate(object sender, TakeDamageEventArgs e)
	{
		MVGameControllerBase.OperationRequests.LogicActivateRequest(Id, activate: true);
	}

	public void SetToDownState()
	{
		targetCollider.enabled = false;
		buttonObject.GreyOutObject.GreyOut();
	}

	public void SetToUpState()
	{
		targetCollider.enabled = true;
		buttonObject.GreyOutObject.GreyIn();
	}

	public override void Destroy()
	{
		if (MVGameControllerBase.IEditModeUI != null)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Remove(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		base.Destroy();
	}

	public void OnEditModeChange(EditModeChangeArgs arg)
	{
		buttonObject.EditCollider.enabled = true;
		targetCollider.enabled = false;
		if (arg.playInEditor)
		{
			buttonObject.EditCollider.enabled = false;
			targetCollider.enabled = true;
		}
	}
}
