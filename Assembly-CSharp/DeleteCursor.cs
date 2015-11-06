using UnityEngine;

public class DeleteCursor
{
	private CellCursor deleteCursor;

	private float deleteCubeTime;

	private float deleteCubeLaserOnTime = 0.2f;

	public DeleteCursor(Vector3[] cubeCorners)
	{
		deleteCursor = new CellCursor(1, 0.03f, "Materials/CellCursorErrorMaterial", 1f, cubeCorners);
	}

	public void UpdateCursor(CubePickingInfo selectedCube, MVCubeModelBase targetCubeModel, bool deletedCube)
	{
		if (deletedCube)
		{
			MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(deleteCubeLaserOnTime);
			deleteCubeTime = Time.time;
		}
		if (Time.time - deleteCubeTime < deleteCubeLaserOnTime)
		{
		}
		if (selectedCube != null)
		{
			MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.UpdatePosition(selectedCube.point);
			deleteCursor.Active = true;
			deleteCursor.SetCursor(selectedCube.iLocalPos, targetCubeModel.GameObject);
		}
		else
		{
			deleteCursor.Active = false;
		}
	}

	public void Remove()
	{
		deleteCursor.Remove();
	}
}
