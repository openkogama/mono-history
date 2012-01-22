using MV.WorldObject;
using UnityEngine;

public class ModelCursor
{
	private IndentArea indentArea;

	private FaceCursor faceCursor;

	private CellCursor cellCursor;

	private CellCursor errorCursor;

	public IndentArea IndentArea => indentArea;

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

	public void UpdateCursor(CubePickingInfo movingEdgeCube, CubePickingInfo selectedCube, GameObject targetGameObject)
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
	}

	public void SetErrorCursor(IntVector iPos, GameObject targetGameObject)
	{
		errorCursor.SetCursor(iPos, targetGameObject);
	}

	public void Destroy()
	{
		indentArea.Destroy();
		faceCursor.Destroy();
		errorCursor.Destroy();
	}
}
