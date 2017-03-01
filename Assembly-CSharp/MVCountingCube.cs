using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class MVCountingCube : MVLogicObject, ILogicWorldObject
{
	private const float ConnectorOffset = 1.5f;

	private const string currentValueKey = "currentValue";

	private const string startingValueKey = "startingValue";

	private const string resetValueKey = "reset";

	private Vector3 ObjectSize = new Vector3(2f, 1.2f, 0.35f);

	private MVCountingCubeObject cubeObject;

	private OutputSignalTransmitter outputSignalTransmitter;

	private bool isHot;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public override Vector3 OutputConnectorOffset => Vector3.right * 1.5f;

	public override Vector3 InputConnectorOffset => Vector3.left * 1.5f;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public int CurrentValue
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

	public int StartingValue
	{
		get
		{
			return (int)Data["startingValue"];
		}
		set
		{
			Data["startingValue"] = value;
		}
	}

	public bool ResetDataValue
	{
		get
		{
			return (bool)Data["reset"];
		}
		set
		{
			Data["reset"] = value;
		}
	}

	public MVCountingCube(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVCountingCubePrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
		cubeObject = (MVCountingCubeObject)component;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(cubeObject.VisualObject);
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: false, null, InputStateUpdateCallback);
		outputSignalTransmitter = new OutputSignalTransmitter(Id);
		interactionFlags |= InteractionFlags.HasSettings;
		SetText();
		if (CurrentValue == 0)
		{
			isHot = true;
		}
	}

	private void InputStateUpdateCallback(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState == LogicInputState.FromColdToHot)
		{
			if (CurrentValue == 1)
			{
				isHot = true;
			}
			if (CurrentValue == 0 && ResetDataValue)
			{
				CurrentValue = StartingValue;
			}
			else if (CurrentValue > 0)
			{
				CurrentValue--;
			}
			SetText();
			PlaySound();
		}
		outputSignalTransmitter.Send(isHot);
		if (CurrentValue != 0)
		{
			isHot = false;
		}
	}

	public override void OnDataUpdate()
	{
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(Vector3.zero, ObjectSize);
	}

	public override void Reset()
	{
		base.Reset();
		CurrentValue = StartingValue;
		isHot = false;
		SetText();
	}

	private void SetText()
	{
		cubeObject.DigitManager.Number = CurrentValue;
	}

	private void PlaySound()
	{
		cubeObject.AudioSource.Play();
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 vector = default;
		vector.x *= 2f;
		vector.y = 1.1f;
		vector.z = 0.3f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, vector);
	}
}
