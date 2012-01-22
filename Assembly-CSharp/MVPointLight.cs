using UnityEngine;

public class MVPointLight : MVLogicObject, WorldObjectWithSettings, WorldObjectWithLogicReset
{
	private GameObject lightObject;

	private Light lightComponent;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	protected override void CreateMVWOC(bool local)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		interactionFlags = InteractionFlags.Selectable;
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/DefaultPointLight"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		gameObject.layer = LayerMask.NameToLayer("Logic");
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

	public void EditSettings()
	{
	}
}
