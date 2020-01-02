using System.Collections.Generic;
using UnityEngine;

public class MVPointLight : MVLogicObject, ILogicWorldObject
{
	private MVPointLightObject lightObject;

	private Light lightComponent;

	private float minumumScale = 0.03f;

	private float rangeMaxValue = 10f;

	private float scaleRestriction = 2f;

	private bool inputVisible;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.PointLight;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVPointLight(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVPointLightPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
		interactionFlags |= InteractionFlags.HasSettings;
		lightObject = (MVPointLightObject)component;
		lightComponent = lightObject.PointLight;
		lightComponent.enabled = false;
		gameObject.transform.localScale = Vector3.one;
	}

	public override void Initialize()
	{
		base.Initialize();
		if (Data.ContainsKey("hide"))
		{
			inputVisible = !(bool)Data["hide"];
		}
		SetupCulling(lightObject.VisualObject);
		SetLightToData();
		UpdateTexture();
		UpdateVisible();
		UpdateColorForLightSphere();
		SetupLightCulling(lightComponent.range);
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, OnInputStateUpdate);
		lightComponent.enabled = InputSignalReceiver.CurrentlyIsHot;
	}

	protected void SetupLightCulling(float radius)
	{
		cullingSubscriberBase.Destroy();
		cullingSubscriberBase = new CullingSubscriberBase(radius, WorldPosition, OnStateChanged);
	}

	private void OnInputStateUpdate(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState == LogicInputState.FromColdToHot)
		{
			lightComponent.enabled = true;
			inputVisible = true;
			UpdateVisible();
		}
		if (logicInputState == LogicInputState.FromHotToCold)
		{
			lightComponent.enabled = false;
			inputVisible = false;
			UpdateVisible();
		}
	}

	public override void OnDataUpdate()
	{
		SetLightToData();
		UpdateVisible();
		UpdateTexture();
		UpdateColorForLightSphere();
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	private bool IsVisible()
	{
		if (!Data.ContainsKey("hide") || !inputVisible)
		{
			return false;
		}
		return !(bool)Data["hide"];
	}

	private void UpdateVisible()
	{
		lightObject.PointLightPlaneMesh.enabled = IsVisible();
	}

	private void UpdateTexture()
	{
		if (Data.ContainsKey("halo"))
		{
			string url = lightObject.StreamedTexture.Url;
			url = url.Remove(url.Length - 9);
			url = url + (int)Data["halo"] + ".unity3d";
			lightObject.StreamedTexture.Url = url;
			lightObject.StreamedTexture.ReDownload();
		}
	}

	private void UpdateColorForLightSphere()
	{
		float[] array = (float[])Data["color"];
		lightObject.PointLightPlaneMesh.materials[0].SetColor("_TintColor", new Color(array[0], array[1], array[2], 1f));
	}

	private void SetLightToData()
	{
		if (Data.ContainsKey("color"))
		{
			float[] array = (float[])Data["color"];
			lightComponent.color = new Color(array[0], array[1], array[2]);
		}
		if (Data.ContainsKey("intensity") && Data.ContainsKey("range"))
		{
			float num = (float)Data["range"];
			float intensity = (float)Data["intensity"];
			lightComponent.intensity = intensity;
			lightComponent.range = num;
			float num2 = num / rangeMaxValue;
			num2 /= scaleRestriction;
			if (num2 < minumumScale)
			{
				num2 = minumumScale;
			}
			lightObject.PointLightPlaneTransform.localScale = new Vector3(num2, num2, num2);
			cullingSubscriberBase.Radius = num;
		}
	}
}
