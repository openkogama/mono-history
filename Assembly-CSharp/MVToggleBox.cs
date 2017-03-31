using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class MVToggleBox : MVLogicObject, ILogicWorldObject
{
	private const string toggledValueKey = "toggled";

	private const string onceValueKey = "once";

	private OutputSignalTransmitter outputSignalTransmitter;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.ToggleBox;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public bool Toggled
	{
		get
		{
			return (ObscuredBool)RunTimeData.GetObscuredType("toggled");
		}
		set
		{
			RunTimeData.SetObscuredType("toggled", (ObscuredBool)value);
		}
	}

	public bool Once
	{
		get
		{
			return (bool)Data["once"];
		}
		set
		{
			Data["once"] = value;
		}
	}

	public MVToggleBox(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVToggleBoxPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags |= InteractionFlags.CanResetLogic;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gameObject);
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: false, null, InputStateUpdateCallback);
		outputSignalTransmitter = new OutputSignalTransmitter(Id);
	}

	private void InputStateUpdateCallback(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (Toggled && Once)
		{
			outputSignalTransmitter.Send(isHot: true);
			return;
		}
		if (logicInputState == LogicInputState.FromColdToHot)
		{
			Toggled = !Toggled;
		}
		outputSignalTransmitter.Send(Toggled);
	}

	public override void Reset()
	{
		Toggled = false;
	}

	public override void OnDataUpdate()
	{
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}
}
