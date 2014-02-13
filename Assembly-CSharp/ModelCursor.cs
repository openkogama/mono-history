using MV.WorldObject;
using UnityEngine;

public class ModelCursor
{
	private IndentArea indentArea;

	private FaceCursor faceCursor;

	private CellCursor errorCursor;

	private float addCubeLaserOnTime = 0.2f;

	public IndentArea IndentArea => indentArea;

	public bool CursorVisible
	{
		get
		{
			return true;
		}
		set
		{
			faceCursor.GameObject.active = value;
			errorCursor.Active = value;
			indentArea.GameObject.active = value;
		}
	}

	public ModelCursor()
	{
		indentArea = new IndentArea();
		faceCursor = new FaceCursor();
		errorCursor = new CellCursor(1, 0.03f, "Materials/CellCursorErrorMaterial", 1f);
	}

	public void SetIndentAreaSize(float size)
	{
		indentArea.Size = size;
	}

	private void HandleLaserMovingEdge(CubePickingInfo movingEdgeCube, GameObject targetGameObject)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] faceVerticesWorld = Cube.GetFaceVerticesWorld(targetGameObject, movingEdgeCube.cube, movingEdgeCube.pickedFace, movingEdgeCube.iLocalPos);
		Vector3 val = (faceVerticesWorld[0] + faceVerticesWorld[1] + faceVerticesWorld[2] + faceVerticesWorld[3]) / 4f;
		bool flag = false;
		Vector3 val2 = default;
		if (movingEdgeCube.pickedEdge == Edge.None)
		{
			val2 = val;
			Debug.DrawLine(val2, val2 + Vector3.up, Color.gray);
		}
		else
		{
			Vector3[] edgeVerticesWorld = Cube.GetEdgeVerticesWorld(targetGameObject, movingEdgeCube.cube, movingEdgeCube.pickedFace, movingEdgeCube.pickedEdge, movingEdgeCube.iLocalPos);
			flag = true;
			if (!movingEdgeCube.pickedEdgeIndex0)
			{
				val2 = ((!movingEdgeCube.pickedEdgeIndex1) ? ((edgeVerticesWorld[0] + edgeVerticesWorld[1]) / 2f) : edgeVerticesWorld[1]);
			}
			else
			{
				val2 = edgeVerticesWorld[0];
				Debug.DrawLine(edgeVerticesWorld[0], edgeVerticesWorld[0] + Vector3.up, Color.gray);
			}
		}
		if (flag)
		{
			val2 += (val - val2) * 0.2f;
		}
		MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.UpdatePosition(val2);
	}

	private void HandleLaser(CubePickingInfo movingEdgeCube, CubePickingInfo selectedCube, GameObject targetGameObject, BuildState buildState, bool addCube)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (buildState == BuildState.PaintCubes)
		{
			Vector3 hit = default;
			MVGameController.Instance.EditController.WorldEditorDrawPlane.Pick(ref hit);
			MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(addCubeLaserOnTime);
			MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.UpdatePosition(hit);
		}
		else if (movingEdgeCube != null)
		{
			HandleLaserMovingEdge(movingEdgeCube, targetGameObject);
			MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(addCubeLaserOnTime);
		}
		else if (addCube)
		{
			MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(addCubeLaserOnTime);
		}
		else if (selectedCube != null)
		{
			MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.UpdatePosition(selectedCube.point);
		}
	}

	public void UpdateCursor(CubePickingInfo movingEdgeCube, CubePickingInfo selectedCube, GameObject targetGameObject, BuildState buildState, bool addCube)
	{
		if (movingEdgeCube != null)
		{
			indentArea.UpdateIndentArea(movingEdgeCube, targetGameObject);
			faceCursor.GameObject.active = true;
			faceCursor.UpdateCursor(movingEdgeCube, targetGameObject);
		}
		else if (selectedCube != null)
		{
			faceCursor.GameObject.active = true;
			faceCursor.UpdateCursor(selectedCube, targetGameObject);
		}
		else
		{
			faceCursor.GameObject.active = false;
			indentArea.GameObject.active = false;
		}
		errorCursor.UpdateCursor();
		HandleLaser(movingEdgeCube, selectedCube, targetGameObject, buildState, addCube);
	}

	public void SetErrorCursor(IntVector iPos, GameObject targetGameObject)
	{
		errorCursor.SetCursor(iPos, targetGameObject);
	}

	public void Remove()
	{
		indentArea.Remove();
		faceCursor.Remove();
		errorCursor.Remove();
	}
}
