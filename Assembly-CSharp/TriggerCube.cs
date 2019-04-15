using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class TriggerCube : MVLogicObject, IIsLogicObjectFiringEventHandler, ILogicWorldObject
{
	private TriggerCubePrefab objPrefab;

	private bool isDown;

	private OutputSignalTransmitter outputSignalTransmitter;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.TriggerCube;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public override Vector3 OutputConnectorOffset => new Vector3(1.5f, 0f, 0f);

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public TriggerCube(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.TriggerCubePrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
		InteractionFlags |= InteractionFlags.HasSettings;
		objPrefab = (TriggerCubePrefab)component;
	}

	public override void Initialize()
	{
		base.Initialize();
		objPrefab.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		objPrefab.TriggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		InputSignalReceiver = LogicClientsideFactory.CreateInputSignalReceiver(this, defaultInput: true, Callback);
		outputSignalTransmitter = new OutputSignalTransmitter(Id);
		isDown = (ObscuredBool)RunTimeData.GetObscuredType("triggerBoxState");
		SetupCulling(objPrefab.gameObject);
		SetScale();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		objPrefab.TriggerBoxEvents.gameObject.SetActive(value: false);
	}

	public override void OnDataUpdate()
	{
		SetScale();
	}

	private void SetScale()
	{
		float num = (float)Data["scaleX"];
		float num2 = (float)Data["scaleY"];
		float num3 = (float)Data["scaleZ"];
		objPrefab.SetScale(new Vector3(num, num2, num3));
		cullingSubscriberBase.Destroy();
		float radius = Mathf.Max(num, num2, num3, 2f);
		cullingSubscriberBase = new CullingSubscriberBase(radius, WorldPosition, OnStateChanged);
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		int woIDWithLocalOwnerHighestInHierarchy = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(e.instigatorWOID);
		if (woIDWithLocalOwnerHighestInHierarchy == -1)
		{
			Debug.LogError("Pressure plate entered by object which is not owned locally");
		}
		else
		{
			MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, woIDWithLocalOwnerHighestInHierarchy);
		}
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		int woIDWithLocalOwnerHighestInHierarchy = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(e.instigatorWOID);
		if (woIDWithLocalOwnerHighestInHierarchy == -1)
		{
			Debug.LogError("Pressure plated exited by object which is not owned locally. This might be ok?");
		}
		else
		{
			MVGameControllerBase.OperationRequests.TriggerBoxExit(Id, woIDWithLocalOwnerHighestInHierarchy);
		}
	}

	private void Callback(bool b, bool wasHot, LogicObjectManager logicObjectManager)
	{
		outputSignalTransmitter.Send(isDown);
	}

	public void OnIsFiringChanged(bool isFiring)
	{
		isDown = isFiring;
	}
}
