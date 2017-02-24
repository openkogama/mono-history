using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class MVPulseBox : MVLogicObject, ILogicWorldObject
{
	private const string currentStartTimeKey = "currentStartTime";

	private OutputSignalTransmitter outputSignalTransmitter;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public int CurrentStartTime
	{
		get
		{
			return (ObscuredInt)RunTimeData.GetObscuredType("currentStartTime");
		}
		set
		{
			RunTimeData.SetObscuredType("currentStartTime", (ObscuredInt)value);
		}
	}

	public MVPulseBox(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVPulseBoxPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gameObject);
		InputSignalReceiver = LogicClientsideFactory.CreateInputSignalReceiver(this, defaultInput: true, OnSignal);
		outputSignalTransmitter = new OutputSignalTransmitter(Id);
	}

	private void OnSignal(bool isHot, bool wasHot, LogicObjectManager logicObjectManager)
	{
		if (!isHot)
		{
			outputSignalTransmitter.Send(isHot: false);
			return;
		}
		int num = (int)((float)Data["intervalOn"] * 1000f);
		int num2 = (int)((float)Data["intervalOff"] * 1000f);
		int num3 = (logicObjectManager.TimeStamp - CurrentStartTime) % (num + num2);
		if (num3 >= num)
		{
			outputSignalTransmitter.Send(isHot: false);
		}
		else
		{
			outputSignalTransmitter.Send(isHot: true);
		}
	}

	public override void OnDataUpdate()
	{
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	public override void Reset()
	{
		base.Reset();
		CurrentStartTime = MVGameControllerBase.Game.LogicObjectManager.TimeStamp;
		Debug.Log("CurrentTime " + CurrentStartTime);
	}
}
