using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVCubeModelInstance : MVCubeModelBase
{
	private int lodId;

	public MVCubeModelInstance(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
		: base(data, worldObjects, prototypes)
	{
		interactionFlags |= InteractionFlags.Selectable | InteractionFlags.HasCubeModel | InteractionFlags.CanRotateY | InteractionFlags.CanEdit | InteractionFlags.CanClone;
		SetLod(1);
		if (MVGameController.Instance.Game.LocalPlayer.ProfileID == PrototypeCubeModel.AuthorProfileID && (interactionFlags & InteractionFlags.CanAddToInventory) != 0)
		{
			interactionFlags |= InteractionFlags.CanAddToInventory;
		}
		else
		{
			interactionFlags &= ~InteractionFlags.CanAddToInventory;
		}
	}

	public override bool CompareWithKoGaMaPackage(MVWorldObjectClient wo, KoGaMaPackageClient koGaMaPackageClient, ref int insertedByProfileId)
	{
		insertedByProfileId = PrototypeCubeModel.AuthorProfileID;
		RuntimePrototypeCubeModel rpcm = koGaMaPackageClient.prototypes[(int)wo.Data["protoTypeID"]];
		return PrototypeCubeModel.CompareGeometry(rpcm);
	}

	public override void Compare(MVWorldObjectClient wo, bool visibleCubesOnly, ref int matchingCubeCount, ref int investigatedCubeCount)
	{
		if (wo.WorldObjectType == WorldObjectType)
		{
			RuntimePrototypeCubeModel rpcm = ((MVCubeModelInstance)wo).PrototypeCubeModel;
			PrototypeCubeModel.CompareGeometryDetailed(rpcm, visibleCubesOnly, ref matchingCubeCount, ref investigatedCubeCount);
		}
	}

	public override void ChangeLOD(float distance)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (!ReactsToLODChanges)
		{
			return;
		}
		float num = MVQualitySettings.CurrentLodData[lodId].activateDistance * Scale.x;
		float num2 = float.PositiveInfinity;
		bool flag = lodId + 1 < MVQualitySettings.CurrentLodData.Length;
		bool flag2 = lodId - 1 >= 0;
		if (flag)
		{
			num2 = MVQualitySettings.CurrentLodData[lodId + 1].activateDistance * Scale.x;
		}
		if (!(distance < num2) || !(distance >= num))
		{
			if (distance > num2 && flag)
			{
				SetLod(lodId + 1);
			}
			if (distance < num && flag2)
			{
				SetLod(lodId - 1);
			}
		}
	}

	private void SetLod(int nextlodId)
	{
		lodId = nextlodId;
		if (!MVQualitySettings.CurrentLodData[lodId].isVisible)
		{
			foreach (KeyValuePair<IntVector, GameObject> chunkInstance in chunkInstances)
			{
				if (chunkInstance.Value.renderer.enabled)
				{
					chunkInstance.Value.renderer.enabled = false;
				}
			}
			return;
		}
		foreach (KeyValuePair<IntVector, GameObject> chunkInstance2 in chunkInstances)
		{
			if (!chunkInstance2.Value.renderer.enabled)
			{
				chunkInstance2.Value.renderer.enabled = true;
			}
		}
	}

	public override void Destroy()
	{
		prototypeCubeModel.RemoveInstance(id);
		base.Destroy();
	}

	public override void Select(Color color)
	{
		AddSelectionBox();
		Selected = true;
	}

	public override void AddSelectionBox()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		SelectionBox selectionBox = gameObject.GetComponentInChildren<SelectionBox>();
		if ((Object)(object)selectionBox == (Object)null)
		{
			GameObject val = CreateBox("SelectionBox", 1.001f);
			selectionBox = val.AddComponent<SelectionBox>();
		}
		Bounds meshBounds = GetMeshBounds();
		((Component)selectionBox).transform.localPosition = meshBounds.center;
		selectionBox.FadeIn(0.2f, "Materials/SelectBoxMaterial", GetCorners(meshBounds));
	}

	public override void AddPreviewBox()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		PreviewBox previewBox = gameObject.GetComponentInChildren<PreviewBox>();
		if ((Object)(object)previewBox == (Object)null)
		{
			GameObject val = CreateBox("PreviewBox", 1.005f);
			previewBox = val.AddComponent<PreviewBox>();
		}
		Bounds meshBounds = GetMeshBounds();
		((Component)previewBox).transform.localPosition = meshBounds.center;
		previewBox.Show("Materials/PreviewBoxMaterial", GetCorners(meshBounds));
	}

	private Vector3[] GetCorners(Bounds bounds)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		num = ((!AEditController.IsGridSnap()) ? 0.0625f : 1f);
		Vector3 val = Vector3.one * 0.5f;
		Vector3 val2 = bounds.min + val;
		Vector3 val3 = bounds.max + val;
		float y = gameObject.transform.localScale.y;
		float num2 = num / y;
		Vector3 vector = MathFunctions.TruncateVector(val2 / num2, 5);
		Vector3 vector2 = MathFunctions.TruncateVector(val3 / num2, 5);
		vector = MathFunctions.FloorVector(vector);
		vector2 = MathFunctions.CeilVector(vector2);
		Vector3 min = vector * num2 - val - bounds.center;
		Vector3 max = vector2 * num2 - val - bounds.center;
		return SharedCubeFunctions.GetCorners(min, max);
	}

	public override void DeSelect()
	{
		RemoveSelectionBox();
		Selected = false;
	}

	public GameObject GetChunkInstance(IntVector chunkPos)
	{
		return chunkInstances[chunkPos];
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(this);
		e.Event = EditorEvent.EditCubes;
		return true;
	}
}
