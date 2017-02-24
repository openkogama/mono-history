using System.Collections.Generic;

public class MVBattery : MVLogicObject
{
	private OutputSignalTransmitter outputSignalTransmitter;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVBattery(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVBatteryPrefab, worldObjects)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		InputSignalReceiver = LogicClientsideFactory.CreateInputSignalReceiver(this, defaultInput: true, SignalCallback);
		outputSignalTransmitter = new OutputSignalTransmitter(Id);
	}

	private void SignalCallback(bool b, bool wasHot, LogicObjectManager logicObjectManager)
	{
		outputSignalTransmitter.Send(isHot: true);
	}
}
