using System.Collections.Generic;
using UnityEngine;

public class MVSkybox : MVLogicObject
{
	protected SkyboxManager skybox;

	protected Color skyboxColor = SkyboxManager.defaultColor;

	protected float sunAngle = 80f;

	protected float fogDensity = 0.007f;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public Color SkyboxColor => skyboxColor;

	public float SunAngle => sunAngle;

	public float FogDensity => fogDensity;

	public bool SkyboxActive
	{
		get
		{
			if (InputLinkRefs.Count == 0)
			{
				return true;
			}
			return InputState;
		}
	}

	public MVSkybox(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSkyboxPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		skybox = Object.FindObjectOfType(typeof(SkyboxManager)) as SkyboxManager;
	}

	public override void Initialize()
	{
		base.Initialize();
		skybox.mvSkyboxes.Add(this);
		gameObject.transform.localScale = Vector3.one;
		OnDataUpdate();
	}

	public override void OnInputLinkChanged()
	{
		Debug.Log("OnInputLinkChanged");
		OnInputStateChanged();
	}

	public override void OnInputStateChanged()
	{
		skybox.RefreshColor();
	}

	public override void OnDataUpdate()
	{
		if (Data.ContainsKey("color"))
		{
			float[] array = (float[])Data["color"];
			skyboxColor = new Color(array[0], array[1], array[2]);
		}
		if (Data.ContainsKey("sunAngle"))
		{
			sunAngle = (float)Data["sunAngle"];
		}
		if (Data.ContainsKey("fogDensity"))
		{
			fogDensity = (float)Data["fogDensity"];
		}
		skybox.RefreshColor();
	}

	public override void Destroy()
	{
		skybox.mvSkyboxes.Remove(this);
		skybox.RefreshColor();
		base.Destroy();
	}
}
