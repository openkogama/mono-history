using UnityEngine;

internal class ESInsert2D : ESStateBase
{
	private Vector3 insertPosition = Vector3.zero;

	private ILaserPointer laser;

	private Vector3 pivotToOrigin;

	private InsertCursor insertCursor;

	private Material previewMaterial;

	private MeshFilter[] previewMeshes;

	private bool isNewPrototype;

	private bool drawPlaneIsActive;

	public override void Enter(EditorStateMachine e)
	{
		((MVAvatarLocal.EditorAvatarMode2D)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).EnableBehindDrawPlaneMode();
		SetupDrawPlane();
		laser = MVGameControllerBase.WOCM.AvatarLocal.LaserPointer;
		laser.ChangeState(LaserPointerState.Inserting);
		laser.LaserActive = true;
		insertCursor = Object.Instantiate(PrefabPool.Instance.InsertCursor);
		if (!previewMaterial)
		{
			previewMaterial = PrefabPool.Instance.InsertPreviewMaterial;
		}
		Cursor.visible = false;
		if (!e.NetworkSelector.RequestOwnership(e.SelectedIDs))
		{
			e.PopState();
			return;
		}
		Vector3 worldPivot = e.SingleSelectedWO.WorldPivot;
		pivotToOrigin = e.SingleSelectedWO.WorldPosition - worldPivot;
		e.SingleSelectedWO.Visible = false;
		previewMeshes = e.SingleSelectedWO.GameObject.GetComponentsInChildren<MeshFilter>();
		isNewPrototype = e.Data.ContainsKey("IsNewPrototype");
	}

	public override void Execute(EditorStateMachine e)
	{
		previewMeshes = e.SingleSelectedWO.GameObject.GetComponentsInChildren<MeshFilter>();
		base.Execute(e);
		Vector3 rawPosition = Vector3.zero;
		Vector3 normal = Vector3.up;
		if (!DrawPlane.IsDrawPlaneActive)
		{
			DrawPlane.SetToTerrain(active: true);
		}
		if (DrawPlanePick(ref rawPosition, ref normal))
		{
			insertCursor.transform.position = rawPosition;
			insertCursor.transform.rotation = Quaternion.LookRotation(normal);
			insertCursor.enabled = true;
		}
		else
		{
			Debug.LogError("Failed to do drawplan pick");
		}
		laser.UpdatePosition(rawPosition);
		insertPosition = rawPosition + pivotToOrigin + Vector3.back * 0.5f;
		e.SingleSelectedWO.SyncPos = ComputeSnapPosition(e.SingleSelectedWO, insertPosition);
		e.SingleSelectedWO.Visible = false;
		DrawObject();
		HandleInput(e);
	}

	private void SetupDrawPlane()
	{
		drawPlaneIsActive = DrawPlane.IsDrawPlaneActive;
		DrawPlane.SetToTerrain(active: true);
	}

	private void ResetDrawPlane()
	{
		if (drawPlaneIsActive != DrawPlane.IsDrawPlaneActive)
		{
			DrawPlane.ToggleDrawPlane();
		}
	}

	private void HandleInput(EditorStateMachine e)
	{
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
		ResetDrawPlane();
		Cursor.visible = true;
		insertCursor.enabled = false;
		Object.Destroy(insertCursor);
		laser.ChangeState(LaserPointerState.Idle);
		laser.LaserActive = false;
		e.SingleSelectedWO.Visible = true;
		e.NetworkSelector.RequestReleaseOwnership(e.SelectedIDs);
		((MVAvatarLocal.EditorAvatarMode2D)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).DisableBehindDrawPlaneMode();
	}

	private static bool DrawPlanePick(ref Vector3 rawPosition, ref Vector3 normal)
	{
		if (DrawPlane.IsDrawPlaneActive)
		{
			Vector3 hit = Vector3.zero;
			if (DrawPlane.Pick(ref hit))
			{
				Vector3 vector = ((!(DrawPlane.Pos.y < MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position.y)) ? Vector3.up : (-Vector3.up));
				rawPosition = hit;
				normal = -vector;
				return true;
			}
		}
		return false;
	}

	private static Vector3 ComputeSnapPosition(MVWorldObjectClient wo, Vector3 originalPos)
	{
		float gridSize = ((!MVGameControllerBase.IEditModeUI.IsGridSnap()) ? 0.0625f : 1f);
		return wo.GetClosestGridPoint(gridSize, originalPos);
	}

	private void DrawObject()
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
