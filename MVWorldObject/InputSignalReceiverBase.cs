using System;
using System.Collections.Generic;
using MV.WorldObject;

public abstract class InputSignalReceiverBase : IInputSignalReceiver
{
	protected const string runtimeDataIsHotKey = "iH";

	protected readonly LogicEvaluateSignalComponentBase logicEvaluateSignalComponentBase;

	protected LogicObjectManager logicObjectManager;

	private int woID = -1;

	public Action<bool, bool, LogicObjectManager> OnSignal;

	protected bool firstFrame = true;

	public abstract bool CurrentlyIsHot { get; set; }

	public bool DefaultInput { get; private set; }

	public void Reset()
	{
		firstFrame = true;
	}

	public InputSignalReceiverBase(MVWorldObject worldObject, LogicEvaluateSignalComponentBase logicEvaluateSignalComponentBase, bool defaultInput, LogicObjectManager logicObjectManager)
	{
		worldObject.OnInputLinkChanged = (Action<List<Link>>)Delegate.Combine(worldObject.OnInputLinkChanged, new Action<List<Link>>(HandleInputLinkChanged));
		woID = worldObject.Id;
		this.logicObjectManager = logicObjectManager;
		this.logicEvaluateSignalComponentBase = logicEvaluateSignalComponentBase;
		logicEvaluateSignalComponentBase.signalsToEvaluate = worldObject.InputLinkRefs.Count;
		if (logicEvaluateSignalComponentBase.signalsToEvaluate == 0)
		{
			logicObjectManager.AddLogicObjectToUpdate(woID, this);
		}
		DefaultInput = defaultInput;
	}

	private void HandleSignalToEvaluateChange(int newNumberOfSignals)
	{
		if (logicEvaluateSignalComponentBase.signalsToEvaluate == 0 && newNumberOfSignals != 0)
		{
			logicObjectManager.RemoveLogicObjectFromUpdate(woID);
		}
		if (newNumberOfSignals == 0 && logicEvaluateSignalComponentBase.signalsToEvaluate != 0)
		{
			logicObjectManager.AddLogicObjectToUpdate(woID, this);
		}
		logicEvaluateSignalComponentBase.signalsToEvaluate = newNumberOfSignals;
	}

	public void UpdateSignal(bool isHot)
	{
		if (logicEvaluateSignalComponentBase.signalsToEvaluate == 0)
		{
			SendSignal(isHot);
			return;
		}
		logicEvaluateSignalComponentBase.UpdateSignal(isHot);
		logicEvaluateSignalComponentBase.evaluatedSignals++;
		if (logicEvaluateSignalComponentBase.evaluatedSignals == logicEvaluateSignalComponentBase.signalsToEvaluate)
		{
			logicEvaluateSignalComponentBase.evaluatedSignals = 0;
			SendSignal(logicEvaluateSignalComponentBase.GetResult());
		}
	}

	public void HandleInputLinkChanged(List<Link> inputLinkRefs)
	{
		HandleSignalToEvaluateChange(inputLinkRefs.Count);
	}

	private void SendSignal(bool isHot)
	{
		bool arg = CurrentlyIsHot;
		CurrentlyIsHot = isHot;
		if (firstFrame)
		{
			arg = !isHot;
		}
		if (OnSignal != null)
		{
			OnSignal(isHot, arg, logicObjectManager);
		}
		if (firstFrame)
		{
			firstFrame = false;
		}
	}
}
