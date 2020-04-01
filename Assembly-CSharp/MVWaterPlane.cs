using System.Collections.Generic;
using UnityEngine;

public class MVWaterPlane : MVLogicObject
{
	protected WaterPlaneManager waterManager;

	private Bounds localBounds;

	private bool addedToWPManager;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.WaterPlane;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	protected override bool HasVisualsInPlaymode => true;

	public override Vector3 WorldPivot => transform.position;

	public MVWaterPlane(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVWaterPlanePrefab, worldObjects)
	{
		Component.MeshRenderers[0].material = new Material(Component.MeshRenderers[0].material);
		localBounds = Component.MeshRenderers[0].bounds;
		localBounds.center -= gameObject.transform.position;
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
		addedToWPManager = true;
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
		if (addedToWPManager)
		{
			waterManager.RemoveWaterPlaneLogicCube(this);
		}
		base.Destroy();
	}
}
