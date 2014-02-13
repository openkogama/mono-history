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

	public ESInsert()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Enter(EditorStateMachine e)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		laser = MVGameController.Instance.WOCM.AvatarLocal.LaserPointer;
		laser.ChangeState(LaserPointerState.Inserting);
		laser.LaserActive = true;
		if (!Object.op_Implicit((Object)(object)previewMaterial))
		{
			Object val = Resources.Load("Materials/InsertPreviewMaterial");
			previewMaterial = (Material)(object)((val is Material) ? val : null);
		}
		float num = e.SingleSelectedWO.ComputeObjectRadius();
		float num2 = Camera.main.fieldOfView * 0.5f * 0.8f;
		distanceInFreeSpace = Mathf.Max(5f, num / Mathf.Tan(num2 * ((float)Math.PI / 180f)));
		Debug.Log((object)("Insert free distance = " + distanceInFreeSpace));
		insertCursor = Object.FindObjectOfType(typeof(InsertCursor)) as InsertCursor;
		insertOffset = Vector3.zero;
		insertPosition = ((Component)Camera.main).transform.position + ((Component)Camera.main).transform.forward * distanceInFreeSpace;
		Screen.showCursor = false;
		if (!e.NetworkSelector.RequestOwnership(e.SelectedIDs))
		{
			e.PopState();
			return;
		}
		Vector3 worldPivot = e.SingleSelectedWO.WorldPivot;
		pivotToOrigin = e.SingleSelectedWO.WorldPosition - worldPivot;
		Ray val2 = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		Vector3 worldPosition = ComputeSnapPosition(e.SingleSelectedWO, val2.GetPoint(distanceInFreeSpace) + pivotToOrigin);
		e.SingleSelectedWO.WorldPosition = worldPosition;
		e.SingleSelectedWO.Visible = false;
		previewMeshes = e.SingleSelectedWO.GameObject.GetComponentsInChildren<MeshFilter>();
		isNewPrototype = e.Data.Contains("IsNewPrototype");
		woIgnoreList = ((!(e.SingleSelectedWO is MVGroup)) ? new HashSet<int> { e.SingleSelectedWO.Id } : (e.SingleSelectedWO as MVGroup).GetHierarchyWorldObjectIDs());
		UXUtils.FindGUIObjectOfType<UXInputDispatcher>().BlockGUIInput = true;
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		previewMeshes = e.SingleSelectedWO.GameObject.GetComponentsInChildren<MeshFilter>();
		base.Execute(e);
		Vector3 position = Vector3.zero;
		Vector3 rawPosition = Vector3.zero;
		Vector3 normal = Vector3.up;
		if (DrawPlanePick(e.SingleSelectedWO, ref position, ref rawPosition, ref normal))
		{
			insertPosition = position;
			((Component)insertCursor).transform.position = rawPosition;
			((Component)insertCursor).transform.rotation = Quaternion.LookRotation(normal);
			((Behaviour)insertCursor).enabled = true;
		}
		else if (WorldPick(e.SingleSelectedWO, ref position, ref rawPosition, ref normal))
		{
			insertPosition = position;
			((Component)insertCursor).transform.position = rawPosition;
			((Component)insertCursor).transform.rotation = Quaternion.LookRotation(normal);
			((Behaviour)insertCursor).enabled = true;
		}
		else
		{
			((Behaviour)insertCursor).enabled = false;
			insertOffset = Vector3.Lerp(insertOffset, Vector3.zero, Time.deltaTime * 10f);
			Ray val = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
			Vector3 val2 = ComputeSnapPosition(e.SingleSelectedWO, val.GetPoint(distanceInFreeSpace) + pivotToOrigin);
			Vector3 val3 = insertPosition - val.origin;
			Vector3 val4 = val2 - val.origin;
			insertPosition = val.origin + val4.normalized * Mathf.Lerp(val3.magnitude, val4.magnitude, Time.deltaTime * 5f);
			rawPosition = insertPosition - pivotToOrigin;
		}
		laser.UpdatePosition(rawPosition);
		Vector3 val5 = ComputeSnapPosition(e.SingleSelectedWO, insertPosition);
		e.SingleSelectedWO.SyncPos = Vector3.Lerp(e.SingleSelectedWO.WorldPosition, val5, Time.deltaTime * 20f);
		e.SingleSelectedWO.Visible = false;
		DrawObject(e.SingleSelectedWO.GameObject);
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			if (isNewPrototype)
			{
				MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(e.SingleSelectedWO);
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Screen.showCursor = true;
		((Behaviour)insertCursor).enabled = false;
		laser.ChangeState(LaserPointerState.Idle);
		laser.LaserActive = false;
		e.SingleSelectedWO.Visible = true;
		Vector3 position = ComputeSnapPosition(e.SingleSelectedWO, insertPosition);
		e.SingleSelectedWO.GameObject.transform.position = position;
		e.SingleSelectedWO.SyncPos = e.SingleSelectedWO.WorldPosition;
		e.NetworkSelector.RequestReleaseOwnership(e.SelectedIDs);
		UXUtils.FindGUIObjectOfType<UXInputDispatcher>().BlockGUIInput = false;
	}

	private Vector3 ComputeObjectOffset(MVWorldObjectClient wo, Vector3 surfaceNormal)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] boundsCornersLocal = wo.GetBoundsCornersLocal(BoundsContext.Insert);
		Vector3 val = Vector3.zero;
		float num = 0f;
		Vector3[] array = boundsCornersLocal;
		foreach (Vector3 a in array)
		{
			Vector3 val2 = a.Multiply(wo.Scale) + pivotToOrigin;
			float num2 = Vector3.Dot(surfaceNormal, val2.normalized);
			if (num2 > num)
			{
				val = val2;
				num = num2;
			}
		}
		return Vector3.Dot(val, surfaceNormal) * surfaceNormal - pivotToOrigin;
	}

	private bool DrawPlanePick(MVWorldObjectClient wo, ref Vector3 position, ref Vector3 rawPosition, ref Vector3 normal)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (MVGameController.Instance.EditController.IsDrawPlaneActive)
		{
			Vector3 hit = Vector3.zero;
			if (MVGameController.Instance.EditController.WorldEditorDrawPlane.Pick(ref hit))
			{
				Vector3 val = ((!(MVGameController.Instance.EditController.WorldEditorDrawPlane.Pos.y < MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position.y)) ? Vector3.up : (-Vector3.up));
				Vector3 val2 = ComputeObjectOffset(wo, val);
				position = hit - val2;
				rawPosition = hit;
				normal = -val;
				return true;
			}
		}
		return false;
	}

	private bool WorldPick(MVWorldObjectClient wo, ref Vector3 position, ref Vector3 rawPosition, ref Vector3 normal)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit, woIgnoreList))
		{
			Vector3 val = ComputeObjectOffset(wo, -hit.normal);
			insertOffset = Vector3.Lerp(insertOffset, val, Time.deltaTime * 10f);
			position = hit.point - insertOffset;
			rawPosition = hit.point;
			normal = hit.normal;
			return true;
		}
		return false;
	}

	private Vector3 ComputeSnapPosition(MVWorldObjectClient wo, Vector3 originalPos)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		float gridSize = ((!AEditController.IsGridSnap()) ? 0.0625f : 1f);
		return wo.GetClosestGridPoint(gridSize, originalPos);
	}

	private void DrawObject(GameObject go)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		MeshFilter[] array = previewMeshes;
		foreach (MeshFilter val in array)
		{
			for (int j = 0; j < val.sharedMesh.subMeshCount; j++)
			{
				Graphics.DrawMesh(val.sharedMesh, ((Component)val).transform.localToWorldMatrix, previewMaterial, LayerMask.NameToLayer("Default"), Camera.main, j);
			}
		}
	}
}
