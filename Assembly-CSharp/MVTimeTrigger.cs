using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class MVTimeTrigger : MVLogicObject, ILogicWorldObject
{
	private const string timeValueKey = "time";

	private const string durationValueKey = "duration";

	private const string currentTimeValueKey = "cT";

	private const int currentTimeDefaultValue = -1;

	private OutputSignalTransmitter outputSignalTransmitter;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public int DelayTime => (int)((float)Data["time"] * 1000f);

	public int ActiveDurationTime => (int)((float)Data["duration"] * 1000f);

	public int CurrentTime
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

	public MVTimeTrigger(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVTimeTriggerPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
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
		if (logicInputState == LogicInputState.FromColdToHot && CurrentTime == -1)
		{
			CurrentTime = 0;
		}
		if (CurrentTime == -1)
		{
			outputSignalTransmitter.Send(isHot: false);
		}
		else if (CurrentTime > ActiveDurationTime + DelayTime)
		{
			CurrentTime = -1;
			outputSignalTransmitter.Send(isHot: false);
		}
		else
		{
			bool isHot = CurrentTime > DelayTime && CurrentTime <= ActiveDurationTime + DelayTime;
			outputSignalTransmitter.Send(isHot);
			CurrentTime += 100;
		}
	}

	public override void OnDataUpdate()
	{
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	public override void Reset()
	{
		CurrentTime = -1;
	}
}
