using UnityEngine;

public class PaintCursor
{
	private CellCursor paintCursor;

	public PaintCursor(Vector3[] cubeCorners)
	{
		paintCursor = new CellCursor(1, 0.03f, "Materials/CellCursorMaterial", 1f, cubeCorners);
	}

	public void UpdateCursor(CubePickingInfo selectedCube, MVCubeModelBase targetCubeModel, bool isPainting)
	{
		if (selectedCube != null)
		{
			paintCursor.Active = true;
			paintCursor.SetCursor(selectedCube.iLocalPos, targetCubeModel.GameObject);
			if (isPainting)
			{
				MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(0.5f);
			}
			MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.UpdatePosition(selectedCube.point);
		}
		else
		{
			paintCursor.Active = false;
		}
	}

	public void Remove()
	{
		paintCursor.Remove();
	}
}
