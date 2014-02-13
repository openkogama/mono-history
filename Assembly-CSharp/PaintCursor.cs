public class PaintCursor
{
	private CellCursor paintCursor;

	public PaintCursor()
	{
		paintCursor = new CellCursor(1, 0.03f, "Materials/CellCursorMaterial", 1f);
	}

	public void UpdateCursor(CubePickingInfo selectedCube, MVCubeModelBase targetCubeModel, bool isPainting)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (selectedCube != null)
		{
			paintCursor.Active = true;
			paintCursor.SetCursor(selectedCube.iLocalPos, targetCubeModel.GameObject);
			if (isPainting)
			{
				MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(0.5f);
			}
			MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.UpdatePosition(selectedCube.point);
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
