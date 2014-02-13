using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVSkybox : MVLogicObject
{
	private const string prefabPath = "Prefabs/Logic/Skybox";

	protected SkyboxManager skybox;

	protected Color skyboxColor = SkyboxManager.defaultColor;

	protected float sunAngle = 80f;

	protected float fogDensity = 0.007f;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public Color SkyboxColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return skyboxColor;
		}
	}

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

	public MVSkybox(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Logic/Skybox", worldObjects)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		interactionFlags |= InteractionFlags.HasSettings;
		skybox = Object.FindObjectOfType(typeof(SkyboxManager)) as SkyboxManager;
	}

	public override void Initialize()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		skybox.mvSkyboxes.Add(this);
		gameObject.transform.localScale = Vector3.one;
		OnDataUpdate();
	}

	public override void OnInputLinkChanged()
	{
		Debug.Log((object)"OnInputLinkChanged");
		OnInputStateChanged();
	}

	public override void OnInputStateChanged()
	{
		skybox.RefreshColor();
	}

	public override void OnDataUpdate()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
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
