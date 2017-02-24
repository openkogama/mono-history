using System;
using MV.WorldObject;

public static class LogicClientsideFactory
{
	public static IInputSignalReceiver CreateInputSignalReceiverAnd(MVWorldObject worldObject, bool defaultSignal, Action<bool, bool, LogicObjectManager> signalCallback)
	{
		InputSignalReceiverClient inputSignalReceiverClient = new InputSignalReceiverClient(worldObject, new LogicEvaluateInputSignalsAnd(), defaultSignal, MVGameControllerBase.Game.LogicObjectManager);
		inputSignalReceiverClient.OnSignal = (Action<bool, bool, LogicObjectManager>)Delegate.Combine(inputSignalReceiverClient.OnSignal, signalCallback);
		return inputSignalReceiverClient;
	}

	public static IInputSignalReceiver CreateInputSignalReceiver(MVWorldObject worldObject, bool defaultInput, Action<bool, bool, LogicObjectManager> signalCallback)
	{
		InputSignalReceiverClient inputSignalReceiverClient = new InputSignalReceiverClient(worldObject, new LogicEvaluateInputSignalsOr(), defaultInput, MVGameControllerBase.Game.LogicObjectManager);
		inputSignalReceiverClient.OnSignal = (Action<bool, bool, LogicObjectManager>)Delegate.Combine(inputSignalReceiverClient.OnSignal, signalCallback);
		return inputSignalReceiverClient;
	}

	public static IInputSignalReceiver CreateStateChangeInputSignalReceiver(MVWorldObject worldObject, bool defaultInput, Action<bool, bool, LogicObjectManager> signalCallback, Action<LogicInputState, LogicObjectManager> inputStateUpdateCallback)
	{
		InputSignalReceiverClient inputSignalReceiverBase = new InputSignalReceiverClient(worldObject, new LogicEvaluateInputSignalsOr(), defaultInput, MVGameControllerBase.Game.LogicObjectManager);
		SignalReceiverStateChangeCallbacks signalReceiverStateChangeCallbacks = new SignalReceiverStateChangeCallbacks(inputSignalReceiverBase);
		if (signalCallback != null)
		{
			signalReceiverStateChangeCallbacks.OnSignal = (Action<bool, bool, LogicObjectManager>)Delegate.Combine(signalReceiverStateChangeCallbacks.OnSignal, signalCallback);
		}
		if (inputStateUpdateCallback != null)
		{
			signalReceiverStateChangeCallbacks.OnInputStateUpdate = (Action<LogicInputState, LogicObjectManager>)Delegate.Combine(signalReceiverStateChangeCallbacks.OnInputStateUpdate, inputStateUpdateCallback);
		}
		return signalReceiverStateChangeCallbacks;
	}
}
