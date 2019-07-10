using UnityEngine;

public class DeleteCursor
{
	private CellCursor deleteCursor;

	private float deleteCubeTime;

	private float deleteCubeLaserOnTime = 0.2f;

	public DeleteCursor(Vector3[] cubeCorners)
	{
		deleteCursor = new CellCursor(1, 0.03f, PrefabPool.Instance.CellCursorErrorMaterial, 1f, cubeCorners);
	}

	public void UpdateCursor(CubePickingInfo selectedCube, MVCubeModelBase targetCubeModel, bool deletedCube)
	{
		if (deletedCube)
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.ActivateLaserForDuration(deleteCubeLaserOnTime);
			deleteCubeTime = Time.time;
		}
		if (Time.time - deleteCubeTime < deleteCubeLaserOnTime)
		{
		}
		if (selectedCube != null)
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.UpdatePosition(selectedCube.point);
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
