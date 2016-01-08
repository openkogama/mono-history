using UnityEngine;

public class ModelCursor2d : ModelCursor
{
	public ModelCursor2d(Vector3[] cubeCorners)
		: base(cubeCorners)
	{
		faceCursor = new FaceCursor(PrefabPool.Instance.Cursor2dEdgeMaterial, PrefabPool.Instance.Cursor2dCornerMaterial, PrefabPool.Instance.CursorNoneMaterial);
	}

	public void UpdateCursor(CubePickingInfo movingEdgeCube, CubePickingInfo selectedCube, GameObject targetGameObject, BuildState buildState, bool addCube)
	{
		if (movingEdgeCube != null)
		{
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
		}
		errorCursor.UpdateCursor();
		HandleLaser(movingEdgeCube, selectedCube, targetGameObject, buildState, addCube);
	}
}
