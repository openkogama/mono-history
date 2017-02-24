using System.Collections.Generic;
using UnityEngine;

public class MVPointLight : MVLogicObject, ILogicWorldObject
{
	private MVPointLightObject lightObject;

	private Light lightComponent;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVPointLight(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVPointLightPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		lightObject = (MVPointLightObject)component;
		lightComponent = lightObject.PointLight;
		lightComponent.enabled = false;
		gameObject.transform.localScale = Vector3.one;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(lightObject.VisualObject);
		SetLightToData();
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, OnInputStateUpdate);
		lightComponent.enabled = InputSignalReceiver.CurrentlyIsHot;
	}

	private void OnInputStateUpdate(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState == LogicInputState.FromColdToHot)
		{
			lightComponent.enabled = true;
		}
		if (logicInputState == LogicInputState.FromHotToCold)
		{
			lightComponent.enabled = false;
		}
	}

	public override void OnDataUpdate()
	{
		SetLightToData();
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	private void SetLightToData()
	{
		if (Data.ContainsKey("color"))
		{
			float[] array = (float[])Data["color"];
			float intensity = (float)Data["intensity"];
			float num = (float)Data["range"];
			lightComponent.color = new Color(array[0], array[1], array[2]);
			lightComponent.range = num;
			lightComponent.intensity = intensity;
			cullingSubscriberBase.Radius = num;
		}
		else
		{
			Debug.LogWarning("'OLD' light object discovered...updating the Data field to include light settings");
		}
	}
}
