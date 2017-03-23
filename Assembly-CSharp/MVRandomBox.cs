using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject;

public class MVRandomBox : MVLogicObject, ILogicWorldObject
{
	private const string currentRandomValuesKey = "currentRandomValues";

	private const string currentValueKey = "currentValue";

	private const int currentValueDefault = 0;

	private RandomGenerator randomGenerator;

	private OutputSignalTransmitterSpecific _outputSignalTransmitter;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	private int CurrentValue
	{
		get
		{
			return (ObscuredInt)RunTimeData.GetObscuredType("currentValue");
		}
		set
		{
			RunTimeData.SetObscuredType("currentValue", (ObscuredInt)value);
		}
	}

	private ObscuredInt[] CurrentRandomValues => (ObscuredInt[])RunTimeData.GetObscuredType("currentRandomValues");

	public MVRandomBox(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVRandomBoxPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gameObject);
		randomGenerator = new RandomGenerator((uint)(int)CurrentRandomValues[0], (uint)(int)CurrentRandomValues[1], (uint)(int)CurrentRandomValues[2]);
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: false, null, InputStateUpdateCallback);
		_outputSignalTransmitter = new OutputSignalTransmitterSpecific(Id);
	}

	public void SetRandomIndex(int randomIndex)
	{
		CurrentValue = randomIndex;
	}

	private void InputStateUpdateCallback(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState == LogicInputState.FromColdToHot)
		{
			if (OutputLinkRefs.Count == 0)
			{
				CurrentValue = 0;
			}
			else
			{
				CurrentValue = randomGenerator.Range(0, OutputLinkRefs.Count);
			}
		}
		if (logicInputState == LogicInputState.FromColdToHot || logicInputState == LogicInputState.Hot)
		{
			_outputSignalTransmitter.Send(CurrentValue);
		}
		else
		{
			_outputSignalTransmitter.Send(-1);
		}
	}

	public override void Reset()
	{
		CurrentValue = 0;
		base.Reset();
	}
}
