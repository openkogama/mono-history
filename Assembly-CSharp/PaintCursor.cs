public class PaintCursor
{
	private CellCursor paintCursor;

	public PaintCursor()
	{
		paintCursor = new CellCursor(1, 0.03f, "Materials/CellCursorMaterial", 1f);
	}

	public void UpdateCursor(CubePickingInfo selectedCube, MVCubeModelBase targetCubeModel, bool isPainting)
	{
		if (selectedCube != null)
		{
			paintCursor.Active = true;
			paintCursor.SetCursor(selectedCube.iLocalPos, targetCubeModel.GameObject);
			if (isPainting)
			{
				MVGameController.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(0.5f);
			}
			MVGameController.WOCM.AvatarLocal.LaserPointer.UpdatePosition(selectedCube.point);
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
