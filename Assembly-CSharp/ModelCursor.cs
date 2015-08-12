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
			faceCursor.GameObject.SetActive(value);
			errorCursor.Active = value;
			indentArea.GameObject.SetActive(value);
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
		Vector3[] faceVerticesWorld = Cube.GetFaceVerticesWorld(targetGameObject, movingEdgeCube.cube, movingEdgeCube.pickedFace, movingEdgeCube.iLocalPos);
		Vector3 vector = (faceVerticesWorld[0] + faceVerticesWorld[1] + faceVerticesWorld[2] + faceVerticesWorld[3]) / 4f;
		bool flag = false;
		Vector3 vector2 = default;
		if (movingEdgeCube.pickedEdge == Edge.None)
		{
			vector2 = vector;
			Debug.DrawLine(vector2, vector2 + Vector3.up, Color.gray);
		}
		else
		{
			Vector3[] edgeVerticesWorld = Cube.GetEdgeVerticesWorld(targetGameObject, movingEdgeCube.cube, movingEdgeCube.pickedFace, movingEdgeCube.pickedEdge, movingEdgeCube.iLocalPos);
			flag = true;
			if (!movingEdgeCube.pickedEdgeIndex0)
			{
				vector2 = ((!movingEdgeCube.pickedEdgeIndex1) ? ((edgeVerticesWorld[0] + edgeVerticesWorld[1]) / 2f) : edgeVerticesWorld[1]);
			}
			else
			{
				vector2 = edgeVerticesWorld[0];
				Debug.DrawLine(edgeVerticesWorld[0], edgeVerticesWorld[0] + Vector3.up, Color.gray);
			}
		}
		if (flag)
		{
			vector2 += (vector - vector2) * 0.2f;
		}
		MVGameController.WOCM.AvatarLocal.LaserPointer.UpdatePosition(vector2);
	}

	private void HandleLaser(CubePickingInfo movingEdgeCube, CubePickingInfo selectedCube, GameObject targetGameObject, BuildState buildState, bool addCube)
	{
		if (buildState == BuildState.PaintCubes)
		{
			Vector3 hit = default;
			MVGameController.EditController.WorldEditorDrawPlane.Pick(ref hit);
			MVGameController.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(addCubeLaserOnTime);
			MVGameController.WOCM.AvatarLocal.LaserPointer.UpdatePosition(hit);
		}
		else if (movingEdgeCube != null)
		{
			HandleLaserMovingEdge(movingEdgeCube, targetGameObject);
			MVGameController.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(addCubeLaserOnTime);
		}
		else if (addCube)
		{
			MVGameController.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(addCubeLaserOnTime);
		}
		else if (selectedCube != null)
		{
			MVGameController.WOCM.AvatarLocal.LaserPointer.UpdatePosition(selectedCube.point);
		}
	}

	public void UpdateCursor(CubePickingInfo movingEdgeCube, CubePickingInfo selectedCube, GameObject targetGameObject, BuildState buildState, bool addCube)
	{
		if (movingEdgeCube != null)
		{
			indentArea.UpdateIndentArea(movingEdgeCube, targetGameObject);
			faceCursor.GameObject.SetActive(value: true);
			faceCursor.UpdateCursor(movingEdgeCube, targetGameObject);
		}
		else if (selectedCube != null)
		{
			faceCursor.GameObject.SetActive(value: true);
			faceCursor.UpdateCursor(selectedCube, targetGameObject);
		}
		else
		{
			faceCursor.GameObject.SetActive(value: false);
			indentArea.GameObject.SetActive(value: false);
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
