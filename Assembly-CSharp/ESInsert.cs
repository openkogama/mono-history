using System;
using System.Collections.Generic;
using UnityEngine;

internal class ESInsert : ESStateBase
{
	private const float returnSpeed = 5f;

	private const float moveSpeed = 20f;

	private const float offsetLerpSpeed = 10f;

	private const float minimumDistance = 5f;

	private float distanceInFreeSpace = 5f;

	private Vector3 insertPosition = Vector3.zero;

	private Vector3 insertOffset = Vector3.zero;

	private ILaserPointer laser;

	private Vector3 pivotToOrigin;

	private InsertCursor insertCursor;

	private Material previewMaterial;

	private MeshFilter[] previewMeshes;

	private bool isNewPrototype;

	private HashSet<int> woIgnoreList = new HashSet<int>();

	public override void Enter(EditorStateMachine e)
	{
		laser = MVGameControllerBase.WOCM.AvatarLocal.LaserPointer;
		laser.ChangeState(LaserPointerState.Inserting);
		laser.LaserActive = true;
		if (!previewMaterial)
		{
			previewMaterial = PrefabPool.Instance.InsertPreviewMaterial;
		}
		float num = e.SingleSelectedWO.ComputeObjectRadius();
		float num2 = Camera.main.fieldOfView * 0.5f * 0.8f;
		distanceInFreeSpace = Mathf.Max(5f, num / Mathf.Tan(num2 * ((float)Math.PI / 180f)));
		Debug.Log("Insert free distance = " + distanceInFreeSpace);
		insertCursor = UnityEngine.Object.Instantiate(PrefabPool.Instance.InsertCursor);
		insertOffset = Vector3.zero;
		insertPosition = Camera.main.transform.position + Camera.main.transform.forward * distanceInFreeSpace;
		Cursor.visible = false;
		if (!e.NetworkSelector.RequestOwnership(e.SelectedIDs))
		{
			e.PopState();
			return;
		}
		Vector3 worldPivot = e.SingleSelectedWO.WorldPivot;
		pivotToOrigin = e.SingleSelectedWO.WorldPosition - worldPivot;
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
		Vector3 worldPosition = ComputeSnapPosition(e.SingleSelectedWO, ray.GetPoint(distanceInFreeSpace) + pivotToOrigin);
		e.SingleSelectedWO.WorldPosition = worldPosition;
		e.SingleSelectedWO.Visible = false;
		previewMeshes = e.SingleSelectedWO.GameObject.GetComponentsInChildren<MeshFilter>();
		isNewPrototype = e.Data.ContainsKey("IsNewPrototype");
		woIgnoreList = ((!(e.SingleSelectedWO is MVGroup)) ? new HashSet<int> { e.SingleSelectedWO.Id } : (e.SingleSelectedWO as MVGroup).GetHierarchyWorldObjectIDs());
		if (e.SingleSelectedWO.GameObject.layer == LayerUtil.GetLayerNumber(LayerFlags.Logic))
		{
			MVGameControllerBase.CameraController.IsLogicRendered = true;
		}
		Debug.LogWarning("Block button pressing when dragging object");
	}

	public override void Execute(EditorStateMachine e)
	{
		previewMeshes = e.SingleSelectedWO.GameObject.GetComponentsInChildren<MeshFilter>();
		base.Execute(e);
		Vector3 position = Vector3.zero;
		Vector3 rawPosition = Vector3.zero;
		Vector3 normal = Vector3.up;
		if (DrawPlanePick(e.SingleSelectedWO, ref position, ref rawPosition, ref normal))
		{
			insertPosition = position;
			insertCursor.transform.position = rawPosition;
			insertCursor.transform.rotation = Quaternion.LookRotation(normal);
			insertCursor.enabled = true;
		}
		else if (WorldPick(e.SingleSelectedWO, ref position, ref rawPosition, ref normal))
		{
			insertPosition = position;
			insertCursor.transform.position = rawPosition;
			insertCursor.transform.rotation = Quaternion.LookRotation(normal);
			insertCursor.enabled = true;
		}
		else
		{
			insertCursor.enabled = false;
			insertOffset = Vector3.Lerp(insertOffset, Vector3.zero, Time.deltaTime * 10f);
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
			Vector3 vector = ComputeSnapPosition(e.SingleSelectedWO, ray.GetPoint(distanceInFreeSpace) + pivotToOrigin);
			Vector3 vector2 = insertPosition - ray.origin;
			Vector3 vector3 = vector - ray.origin;
			insertPosition = ray.origin + vector3.normalized * Mathf.Lerp(vector2.magnitude, vector3.magnitude, Time.deltaTime * 5f);
			rawPosition = insertPosition - pivotToOrigin;
		}
		laser.UpdatePosition(rawPosition);
		Vector3 b = ComputeSnapPosition(e.SingleSelectedWO, insertPosition);
		e.SingleSelectedWO.SyncPos = Vector3.Lerp(e.SingleSelectedWO.WorldPosition, b, Time.deltaTime * 20f);
		e.SingleSelectedWO.Visible = false;
		DrawObject(e.SingleSelectedWO.GameObject);
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			if (isNewPrototype)
			{
				MVGameControllerBase.CameraController.CurCamera.FocusOnObject(e.SingleSelectedWO);
				e.Event = EditorEvent.EditCubes;
			}
			else
			{
				e.Event = EditorEvent.ESTerrainEdit;
			}
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		Cursor.visible = true;
		insertCursor.enabled = false;
		UnityEngine.Object.Destroy(insertCursor.gameObject);
		laser.ChangeState(LaserPointerState.Idle);
		laser.LaserActive = false;
		e.SingleSelectedWO.Visible = true;
		Vector3 position = ComputeSnapPosition(e.SingleSelectedWO, insertPosition);
		e.SingleSelectedWO.GameObject.transform.position = position;
		e.SingleSelectedWO.SyncPos = e.SingleSelectedWO.WorldPosition;
		e.NetworkSelector.RequestReleaseOwnership(e.SelectedIDs);
	}

	private Vector3 ComputeObjectOffset(MVWorldObjectClient wo, Vector3 surfaceNormal)
	{
		Vector3[] boundsCornersLocal = wo.GetBoundsCornersLocal(BoundsContext.Insert);
		Vector3 lhs = Vector3.zero;
		float num = 0f;
		Vector3[] array = boundsCornersLocal;
		foreach (Vector3 a in array)
		{
			Vector3 vector = a.Multiply(wo.Scale) + pivotToOrigin;
			float num2 = Vector3.Dot(surfaceNormal, vector.normalized);
			if (num2 > num)
			{
				lhs = vector;
				num = num2;
			}
		}
		return Vector3.Dot(lhs, surfaceNormal) * surfaceNormal - pivotToOrigin;
	}

	private bool DrawPlanePick(MVWorldObjectClient wo, ref Vector3 position, ref Vector3 rawPosition, ref Vector3 normal)
	{
		if (DrawPlane.IsDrawPlaneActive)
		{
			Vector3 hit = Vector3.zero;
			if (DrawPlane.Pick(ref hit))
			{
				Vector3 vector = ((!(DrawPlane.Pos.y < MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position.y)) ? Vector3.up : (-Vector3.up));
				Vector3 vector2 = ComputeObjectOffset(wo, vector);
				position = hit - vector2;
				rawPosition = hit;
				normal = -vector;
				return true;
			}
		}
		return false;
	}

	private bool WorldPick(MVWorldObjectClient wo, ref Vector3 position, ref Vector3 rawPosition, ref Vector3 normal)
	{
		VoxelHit hit = default;
		if (EditModeObjectPicker.Pick(ref hit, woIgnoreList))
		{
			Vector3 b = ComputeObjectOffset(wo, -hit.normal);
			insertOffset = Vector3.Lerp(insertOffset, b, Time.deltaTime * 10f);
			position = hit.point - insertOffset;
			rawPosition = hit.point;
			normal = hit.normal;
			return true;
		}
		return false;
	}

	private Vector3 ComputeSnapPosition(MVWorldObjectClient wo, Vector3 originalPos)
	{
		float gridSize = ((!MVGameControllerBase.IEditModeUI.IsGridSnap()) ? 0.0625f : 1f);
		return wo.GetClosestGridPoint(gridSize, originalPos);
	}

	private void DrawObject(GameObject go)
	{
		MeshFilter[] array = previewMeshes;
		foreach (MeshFilter meshFilter in array)
		{
			for (int j = 0; j < meshFilter.sharedMesh.subMeshCount; j++)
			{
				Graphics.DrawMesh(meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix, previewMaterial, LayerMask.NameToLayer("Default"), Camera.main, j);
			}
		}
	}
}
