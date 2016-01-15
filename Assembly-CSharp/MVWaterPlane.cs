using System.Collections.Generic;
using UnityEngine;

public class MVWaterPlane : MVLogicObject
{
	protected WaterPlaneManager waterManager;

	private MeshRenderer meshRenderer;

	private Bounds localBounds;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MeshRenderer MeshRenderer
	{
		get
		{
			if (meshRenderer == null)
			{
				meshRenderer = gameObject.GetComponent<MeshRenderer>();
			}
			return meshRenderer;
		}
	}

	public MVWaterPlane(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVWaterPlanePrefab, worldObjects)
	{
		MeshRenderer.material = new Material(MeshRenderer.material);
		localBounds = MeshRenderer.bounds;
		localBounds.center -= gameObject.transform.position;
		gameObject.transform.localScale = Vector3.one;
		waterManager = Object.FindObjectOfType(typeof(WaterPlaneManager)) as WaterPlaneManager;
		interactionFlags |= InteractionFlags.HasSettings;
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

	public override void CheckCanInsert(MVGUIInventoryGroup.CanInsertDelegate canInsert)
	{
		canInsert(canInsert: true);
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
			MeshRenderer.material.SetColor("_MaskedColor", new Color(array[0], array[1], array[2]));
		}
	}

	public override void Destroy()
	{
		waterManager.RemoveWaterPlaneLogicCube(this);
		base.Destroy();
	}
}
