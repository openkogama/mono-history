using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVCubeModelInstance : MVCubeModelBase
{
	private bool isVisible;

	private int lodDistance = 300;

	public MVCubeModelInstance(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
		: base(data, worldObjects, prototypes)
	{
		interactionFlags |= InteractionFlags.Selectable | InteractionFlags.HasCubeModel | InteractionFlags.CanRotateY | InteractionFlags.CanEdit | InteractionFlags.CanClone;
		if (MVGameController.Game.LocalPlayer.ProfileID == PrototypeCubeModel.AuthorProfileID && (interactionFlags & InteractionFlags.CanAddToInventory) != 0)
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
		if (ReactsToLODChanges)
		{
			float num = (float)lodDistance * Scale.x;
			if (distance > num && isVisible)
			{
				SetLod(enabled: false);
			}
			if (distance <= num && !isVisible)
			{
				SetLod(enabled: true);
			}
		}
	}

	private void SetLod(bool enabled)
	{
		foreach (KeyValuePair<IntVector, GameObject> item in (IEnumerable)chunkInstances)
		{
			item.Value.GetComponent<Renderer>().enabled = enabled;
		}
		isVisible = enabled;
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
		SelectionBox selectionBox = base.gameObject.GetComponentInChildren<SelectionBox>();
		if (selectionBox == null)
		{
			GameObject gameObject = CreateBox("SelectionBox", 1.001f);
			selectionBox = gameObject.AddComponent<SelectionBox>();
		}
		Bounds meshBounds = GetMeshBounds();
		selectionBox.transform.localPosition = meshBounds.center;
		selectionBox.FadeIn(0.2f, "Materials/SelectBoxMaterial", GetCorners(meshBounds));
	}

	public override void AddPreviewBox()
	{
		PreviewBox previewBox = base.gameObject.GetComponentInChildren<PreviewBox>();
		if (previewBox == null)
		{
			GameObject gameObject = CreateBox("PreviewBox", 1.005f);
			previewBox = gameObject.AddComponent<PreviewBox>();
		}
		Bounds meshBounds = GetMeshBounds();
		previewBox.transform.localPosition = meshBounds.center;
		previewBox.Show("Materials/PreviewBoxMaterial", GetCorners(meshBounds));
	}

	private Vector3[] GetCorners(Bounds bounds)
	{
		float num = 0f;
		num = ((MVGameController.GameMode != MVGameMode.Edit || !MVGameController.EditorController.IsGridSnap()) ? 0.0625f : 1f);
		Vector3 vector = Vector3.one * 0.5f;
		Vector3 vector2 = bounds.min + vector;
		Vector3 vector3 = bounds.max + vector;
		float y = gameObject.transform.localScale.y;
		float num2 = num / y;
		Vector3 vector4 = MathFunctions.TruncateVector(vector2 / num2, 5);
		Vector3 vector5 = MathFunctions.TruncateVector(vector3 / num2, 5);
		vector4 = MathFunctions.FloorVector(vector4);
		vector5 = MathFunctions.CeilVector(vector5);
		Vector3 min = vector4 * num2 - vector - bounds.center;
		Vector3 max = vector5 * num2 - vector - bounds.center;
		return SharedCubeFunctions.GetCorners(min, max);
	}

	public override void DeSelect()
	{
		RemoveSelectionBox();
		Selected = false;
	}

	public GameObject GetChunkInstance(IntVector chunkPos)
	{
		return chunkInstances.GetChunk(chunkPos);
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVGameController.Game.CameraController.CurCamera.FocusOnObject(this);
		e.Event = EditorEvent.EditCubes;
		return true;
	}
}
