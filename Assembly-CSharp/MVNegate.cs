using System.Collections.Generic;

public class MVNegate : MVLogicObject, ILogicWorldObject
{
	private OutputSignalTransmitter outputSignalTransmitter;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.Negate;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	protected override bool HasVisualsInPlaymode => false;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVNegate(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVNegatePrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gameObject);
		InputSignalReceiver = LogicClientsideFactory.CreateInputSignalReceiver(this, defaultInput: false, SignalCallback);
		outputSignalTransmitter = new OutputSignalTransmitter(Id);
	}

	private void SignalCallback(bool b, bool wasHot, LogicObjectManager logicObjectManager)
	{
		outputSignalTransmitter.Send(!b);
	}
}
