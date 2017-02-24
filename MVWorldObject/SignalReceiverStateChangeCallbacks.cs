using System;

public class SignalReceiverStateChangeCallbacks : IInputSignalReceiver
{
	private readonly InputSignalReceiverBase _inputSignalReceiverBase;

	public Action<bool, bool, LogicObjectManager> OnSignal;

	public Action<LogicInputState, LogicObjectManager> OnInputStateUpdate;

	public bool CurrentlyIsHot => _inputSignalReceiverBase.CurrentlyIsHot;

	public bool DefaultInput => _inputSignalReceiverBase.DefaultInput;

	public void Reset()
	{
		_inputSignalReceiverBase.Reset();
	}

	public SignalReceiverStateChangeCallbacks(InputSignalReceiverBase _inputSignalReceiverBase)
	{
		this._inputSignalReceiverBase = _inputSignalReceiverBase;
		InputSignalReceiverBase inputSignalReceiverBase = this._inputSignalReceiverBase;
		inputSignalReceiverBase.OnSignal = (Action<bool, bool, LogicObjectManager>)Delegate.Combine(inputSignalReceiverBase.OnSignal, new Action<bool, bool, LogicObjectManager>(HandleOnSignal));
	}

	private void HandleOnSignal(bool isHot, bool wasHot, LogicObjectManager logicObjectManager)
	{
		LogicInputState arg = LogicInputState.Cold;
		if (isHot && !wasHot)
		{
			arg = LogicInputState.FromColdToHot;
		}
		if (isHot && wasHot)
		{
			arg = LogicInputState.Hot;
		}
		if (!isHot && wasHot)
		{
			arg = LogicInputState.FromHotToCold;
		}
		if (!isHot && !wasHot)
		{
			arg = LogicInputState.Cold;
		}
		if (OnInputStateUpdate != null)
		{
			OnInputStateUpdate(arg, logicObjectManager);
		}
		if (OnSignal != null)
		{
			OnSignal(isHot, wasHot, logicObjectManager);
		}
	}

	public void UpdateSignal(bool isHot)
	{
		_inputSignalReceiverBase.UpdateSignal(isHot);
	}
}
