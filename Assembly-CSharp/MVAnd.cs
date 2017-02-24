using System.Collections.Generic;

public class MVAnd : MVLogicObject, ILogicWorldObject
{
	private OutputSignalTransmitter outputSignalTransmitter;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVAnd(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVAndPrefab, worldObjects)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gameObject);
		InputSignalReceiver = LogicClientsideFactory.CreateInputSignalReceiverAnd(this, defaultSignal: false, SignalCallback);
		outputSignalTransmitter = new OutputSignalTransmitter(Id);
	}

	private void SignalCallback(bool b, bool wasHot, LogicObjectManager logicObjectManager)
	{
		outputSignalTransmitter.Send(b);
	}
}
