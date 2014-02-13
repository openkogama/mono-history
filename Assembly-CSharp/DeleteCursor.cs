using UnityEngine;

public class DeleteCursor
{
	private CellCursor deleteCursor;

	private float deleteCubeTime;

	private float deleteCubeLaserOnTime = 0.2f;

	public DeleteCursor()
	{
		deleteCursor = new CellCursor(1, 0.03f, "Materials/CellCursorErrorMaterial", 1f);
	}

	public void UpdateCursor(CubePickingInfo selectedCube, MVCubeModelBase targetCubeModel, bool deletedCube)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (deletedCube)
		{
			MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(deleteCubeLaserOnTime);
			deleteCubeTime = Time.time;
		}
		if (Time.time - deleteCubeTime < deleteCubeLaserOnTime)
		{
		}
		if (selectedCube != null)
		{
			MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.UpdatePosition(selectedCube.point);
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
