using System.Collections.Generic;
using UnityEngine;

public class MVWaterPlane : MVLogicObject
{
	protected WaterPlaneManager waterManager;

	private Bounds localBounds;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVWaterPlane(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVWaterPlanePrefab, worldObjects)
	{
		MeshRenderer componentInChildren = gameObject.GetComponentInChildren<MeshRenderer>();
		componentInChildren.material = new Material(componentInChildren.material);
		localBounds = componentInChildren.bounds;
		localBounds.center -= transform.position;
		transform.localScale = Vector3.one;
		waterManager = Object.FindObjectOfType(typeof(WaterPlaneManager)) as WaterPlaneManager;
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags &= ~InteractionFlags.CanClone;
	}

	public override bool IsSingletonObject()
	{
		return true;
	}

	public override void ChangeLOD(float distance)
	{
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
			gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_MaskedColor", new Color(array[0], array[1], array[2]));
		}
	}

	public override void Destroy()
	{
		waterManager.RemoveWaterPlaneLogicCube(this);
		base.Destroy();
	}
}
