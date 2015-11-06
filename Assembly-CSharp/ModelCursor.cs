using MV.WorldObject;
using UnityEngine;

public class ModelCursor
{
	protected FaceCursor faceCursor;

	protected CellCursor errorCursor;

	private float addCubeLaserOnTime = 0.2f;

	public virtual bool CursorVisible
	{
		get
		{
			return true;
		}
		set
		{
			faceCursor.GameObject.SetActive(value);
			errorCursor.Active = value;
		}
	}

	public ModelCursor(Vector3[] cubeCorners)
	{
		errorCursor = new CellCursor(1, 0.03f, "Materials/CellCursorErrorMaterial", 1f, cubeCorners);
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
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.UpdatePosition(vector2);
	}

	protected void HandleLaser(CubePickingInfo movingEdgeCube, CubePickingInfo selectedCube, GameObject targetGameObject, BuildState buildState, bool addCube)
	{
		if (buildState == BuildState.PaintCubes)
		{
			Vector3 hit = default;
			MVGameControllerLegacyUI.CubeModelingEditMode.DrawPlaneController.Pick(ref hit);
			MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(addCubeLaserOnTime);
			MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.UpdatePosition(hit);
		}
		else if (movingEdgeCube != null)
		{
			HandleLaserMovingEdge(movingEdgeCube, targetGameObject);
			MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(addCubeLaserOnTime);
		}
		else if (addCube)
		{
			MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(addCubeLaserOnTime);
		}
		else if (selectedCube != null)
		{
			MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.UpdatePosition(selectedCube.point);
		}
	}

	public void SetErrorCursor(IntVector iPos, GameObject targetGameObject)
	{
		errorCursor.SetCursor(iPos, targetGameObject);
	}

	public virtual void Remove()
	{
		faceCursor.Remove();
		errorCursor.Remove();
	}
}
