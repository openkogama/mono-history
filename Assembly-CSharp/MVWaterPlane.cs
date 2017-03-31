using System.Collections.Generic;
using UnityEngine;

public class MVWaterPlane : MVLogicObject
{
	protected WaterPlaneManager waterManager;

	private Bounds localBounds;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.WaterPlane;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVWaterPlane(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVWaterPlanePrefab, worldObjects)
	{
		Component.MeshRenderers[0].material = new Material(Component.MeshRenderers[0].material);
		localBounds = Component.MeshRenderers[0].bounds;
		localBounds.center -= gameObject.transform.position;
		gameObject.transform.localScale = Vector3.one;
		waterManager = MVGameControllerBase.WaterPlaneManager;
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags &= ~InteractionFlags.CanClone;
	}

	public override bool IsSingletonObject()
	{
		return true;
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return localBounds;
	}

	public override void Initialize()
	{
		base.Initialize();
		waterManager.AddWaterPlaneLogicCube(this);
		if (!Data.ContainsKey("waterColor"))
		{
			Data["waterColor"] = new float[3] { 0.1f, 0.2f, 0.5f };
		}
		OnDataUpdate();
	}

	public override void OnDataUpdate()
	{
		if (Data.ContainsKey("waterColor"))
		{
			float[] array = (float[])Data["waterColor"];
			waterManager.WaterColor = new Color(array[0], array[1], array[2], 0.8f);
			Component.MeshRenderers[0].material.SetColor("_MaskedColor", new Color(array[0], array[1], array[2]));
		}
	}

	public override void Destroy()
	{
		waterManager.RemoveWaterPlaneLogicCube(this);
		base.Destroy();
	}
}
