using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVCubeModelInstance : MVCubeModelBase, WorldObjectWithEdit, WorldObjectWithClone, WorldObjectWithAddToInventory
{
	private int lodId;

	protected override void CreateMVWOC(bool local)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.CreateMVWOC(local);
		interactionFlags = InteractionFlags.Selectable | InteractionFlags.HasCubeModel;
		if (OwnerActorNr != 0 && OwnerActorNr != MVGameController.Instance.WOCM.LocalPlayerActorNumber)
		{
			Select(Color.blue);
		}
		SetLod(1);
	}

	public override void ChangeLOD(float distance)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
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
			SharedMeshData meshData = prototypeCubeModel.Chunks[chunkInstance2.Key].GetMeshData(MVQualitySettings.CurrentLodData[lodId].mipMeshSetting);
			if (meshData.mesh.vertexCount > 0)
			{
				chunkInstance2.Value.GetComponent<MeshFilter>().sharedMesh = meshData.mesh;
				chunkInstance2.Value.renderer.sharedMaterials = meshData.materials;
			}
			if (!chunkInstance2.Value.renderer.enabled)
			{
				chunkInstance2.Value.renderer.enabled = true;
			}
		}
	}

	public override void Initialize()
	{
	}

	public override void Destroy()
	{
		prototypeCubeModel.RemoveInstance(id);
	}

	public override void Select(Color color)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected Obj, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform item in gameObject.transform)
		{
			Transform val = item;
			Material[] materials = ((Component)val).gameObject.renderer.materials;
			Material[] array = materials;
			foreach (Material val2 in array)
			{
				val2.color = color;
			}
			((Component)val).gameObject.renderer.materials = materials;
		}
		AddSelectionBox();
	}

	public override void AddSelectionBox()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected Obj, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		num = ((!MVGameController.Instance.EditorController.GridSnap) ? 0.0625f : 1f);
		SelectionBox selectionBox = gameObject.GetComponentInChildren<SelectionBox>();
		if ((Object)(object)selectionBox == (Object)null)
		{
			GameObject val = new GameObject("SelectionBox");
			val.transform.parent = gameObject.transform;
			val.transform.localPosition = Vector3.zero;
			val.transform.localRotation = Quaternion.identity;
			val.transform.localScale = Vector3.one * 1.001f;
			selectionBox = val.AddComponent<SelectionBox>();
		}
		IntVector min = default;
		IntVector max = default;
		SharedCollisionFunctions.GetVoxelBounds(ref min, ref max, GetMeshBounds());
		Bounds meshBounds = GetMeshBounds();
		Vector3 val2 = Vector3.one * 0.5f;
		Vector3 val3 = meshBounds.min + val2;
		Vector3 val4 = meshBounds.max + val2;
		float y = gameObject.transform.localScale.y;
		float num2 = num / y;
		Vector3 vector = val3 / num2;
		CeilVector(ref vector);
		Vector3 vector2 = val4 / num2;
		CeilVector(ref vector2);
		Vector3 min2 = vector * num2 - val2;
		Vector3 max2 = vector2 * num2 - val2;
		selectionBox.FadeIn(0.2f, "Materials/SelectBoxMaterial", GetCorners(min2, max2));
	}

	private void CeilVector(ref Vector3 vector)
	{
		vector.x = ((!(vector.x >= 0f)) ? Mathf.Floor(vector.x) : Mathf.Ceil(vector.x));
		vector.y = ((!(vector.y >= 0f)) ? Mathf.Floor(vector.y) : Mathf.Ceil(vector.y));
		vector.z = ((!(vector.z >= 0f)) ? Mathf.Floor(vector.z) : Mathf.Ceil(vector.z));
	}

	private Vector3[] GetCorners(Vector3 min, Vector3 max)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3[8]
		{
			new Vector3(min.x, max.y, min.z),
			new Vector3(max.x, max.y, min.z),
			new Vector3(max.x, max.y, max.z),
			new Vector3(min.x, max.y, max.z),
			new Vector3(min.x, min.y, max.z),
			new Vector3(max.x, min.y, max.z),
			new Vector3(max.x, min.y, min.z),
			new Vector3(min.x, min.y, min.z)
		};
	}

	public override void DeSelect()
	{
		prototypeCubeModel.ResetSharedMaterials(this);
		RemoveSelectionBox();
	}

	public GameObject GetChunkInstance(IntVector chunkPos)
	{
		return chunkInstances[chunkPos];
	}

	public void Edit()
	{
	}

	public void EditSettings()
	{
	}

	public void Clone()
	{
		MVGameController.Instance.EditorController.RequestInstance(Pid, MVGameController.Instance.EditorController.GetContextMenuSelectionWO().GameObject);
	}

	public void AddToInventory()
	{
		MVGameController.Instance.EditorController.AddToInventory(this);
	}
}
