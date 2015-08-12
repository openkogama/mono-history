using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MVWaterPlane : MVLogicObject
{
	private const string prefabPath = "Prefabs/Logic/WaterPlane";

	protected WaterPlaneManager waterManager;

	private Bounds localBounds;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVWaterPlane(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Logic/WaterPlane", worldObjects)
	{
		gameObject.GetComponent<Renderer>().material = new Material(gameObject.GetComponent<Renderer>().material);
		interactionFlags |= InteractionFlags.HasSettings;
		localBounds = gameObject.GetComponent<MeshRenderer>().bounds;
		localBounds.center -= gameObject.transform.position;
		gameObject.transform.localScale = Vector3.one;
		waterManager = Object.FindObjectOfType(typeof(WaterPlaneManager)) as WaterPlaneManager;
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

	private void OnOverwriteDialogResult(UXDialogBox dialog, MVGUIInventoryGroup.CanInsertDelegate canInsert)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			HashSet<MVWorldObjectClient> hashSet = new HashSet<MVWorldObjectClient>();
			hashSet.Add(waterManager.WaterPlanes.First());
			MVGameController.EditorController.Delete(hashSet);
			canInsert(canInsert: true);
		}
		else
		{
			canInsert(canInsert: false);
		}
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
			gameObject.GetComponent<Renderer>().material.SetColor("_MaskedColor", new Color(array[0], array[1], array[2]));
		}
	}

	public override void Destroy()
	{
		waterManager.RemoveWaterPlaneLogicCube(this);
		base.Destroy();
	}
}
