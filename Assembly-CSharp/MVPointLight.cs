using System.Collections.Generic;
using UnityEngine;

public class MVPointLight : MVLogicObject
{
	private MVPointLightObject lightObject;

	private Light lightComponent;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

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
		OnDataUpdate();
		if (InputLinkRefs.Count == 0)
		{
			lightComponent.enabled = true;
		}
		else
		{
			OnInputStateChanged();
		}
	}

	public override void OnInputLinkChanged()
	{
		if (InputLinkRefs.Count == 0)
		{
			lightComponent.enabled = true;
		}
		else
		{
			OnInputStateChanged();
		}
	}

	public override void OnInputStateChanged()
	{
		if (InputState)
		{
			lightComponent.enabled = true;
		}
		else
		{
			lightComponent.enabled = false;
		}
	}

	public override void OnDataUpdate()
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
