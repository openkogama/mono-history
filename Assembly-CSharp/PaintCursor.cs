using UnityEngine;

public class PaintCursor
{
	private CellCursor paintCursor;

	public PaintCursor(Vector3[] cubeCorners)
	{
		paintCursor = new CellCursor(1, 0.03f, PrefabPool.Instance.CellCursorMaterial, 1f, cubeCorners);
	}

	public void UpdateCursor(CubePickingInfo selectedCube, MVCubeModelBase targetCubeModel, bool isPainting)
	{
		if (selectedCube != null)
		{
			paintCursor.Active = true;
			paintCursor.SetCursor(selectedCube.iLocalPos, targetCubeModel.GameObject);
			if (isPainting)
			{
				MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.ActivateLaserForDuration(0.5f);
			}
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.UpdatePosition(selectedCube.point);
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
