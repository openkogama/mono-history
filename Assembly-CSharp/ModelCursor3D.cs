using UnityEngine;

public class ModelCursor3D : ModelCursor
{
	private IndentArea indentArea;

	public IndentArea IndentArea => indentArea;

	public override bool CursorVisible
	{
		get
		{
			return base.CursorVisible;
		}
		set
		{
			base.CursorVisible = value;
			indentArea.GameObject.SetActive(value);
		}
	}

	public ModelCursor3D(Vector3[] cubeCorners)
		: base(cubeCorners)
	{
		indentArea = new IndentArea();
		faceCursor = new FaceCursor("Materials/CursorMaterial", "Materials/CursorMaterialCorner", "Materials/CursorMaterialNone");
	}

	public void SetIndentAreaSize(float size)
	{
		indentArea.Size = size;
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

	public override void Remove()
	{
		base.Remove();
		indentArea.Remove();
	}
}
