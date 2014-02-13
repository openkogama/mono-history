using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVPointLight : MVLogicObject
{
	private const string prefabPath = "Prefabs/DefaultPointLight";

	private GameObject lightObject;

	private Light lightComponent;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVPointLight(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/DefaultPointLight", worldObjects)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		interactionFlags |= InteractionFlags.HasSettings;
		lightComponent = gameObject.GetComponent<Light>();
		((Behaviour)lightComponent).enabled = false;
		OnDataUpdate();
		gameObject.transform.localScale = Vector3.one;
	}

	public override void Initialize()
	{
		base.Initialize();
		OnDataUpdate();
		if (InputLinkRefs.Count == 0)
		{
			((Behaviour)lightComponent).enabled = true;
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
			((Behaviour)lightComponent).enabled = true;
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
			((Behaviour)lightComponent).enabled = true;
		}
		else
		{
			((Behaviour)lightComponent).enabled = false;
		}
	}

	public override void OnDataUpdate()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
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
			Debug.LogWarning((object)"'OLD' light object discovered...updating the Data field to include light settings");
		}
	}
}
