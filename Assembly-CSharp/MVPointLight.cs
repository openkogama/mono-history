using System.Collections.Generic;
using UnityEngine;

public class MVPointLight : MVLogicObject
{
	private GameObject lightObject;

	private Light lightComponent;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVPointLight(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVPointLightPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		lightComponent = gameObject.GetComponent<Light>();
		lightComponent.enabled = false;
		OnDataUpdate();
		gameObject.transform.localScale = Vector3.one;
	}

	public override void Initialize()
	{
		base.Initialize();
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
			float range = (float)Data["range"];
			lightComponent.color = new Color(array[0], array[1], array[2]);
			lightComponent.range = range;
			lightComponent.intensity = intensity;
		}
		else
		{
			Debug.LogWarning("'OLD' light object discovered...updating the Data field to include light settings");
		}
	}
}
