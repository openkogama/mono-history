using System.Collections;
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

	public MVWaterPlane(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Logic/WaterPlane", worldObjects)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected Obj, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		gameObject.renderer.material = new Material(gameObject.renderer.material);
		interactionFlags |= InteractionFlags.HasSettings;
		localBounds = ((Renderer)gameObject.GetComponent<MeshRenderer>()).bounds;
		ref Bounds reference = ref localBounds;
		reference.center -= gameObject.transform.position;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
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
			MVGameController.Instance.EditorController.Delete(hashSet);
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
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (Data.ContainsKey("waterColor"))
		{
			float[] array = (float[])Data["waterColor"];
			waterManager.WaterColor = new Color(array[0], array[1], array[2], 0.8f);
			gameObject.renderer.material.SetColor("_MaskedColor", new Color(array[0], array[1], array[2]));
		}
	}

	public override void Destroy()
	{
		waterManager.RemoveWaterPlaneLogicCube(this);
		base.Destroy();
	}
}
