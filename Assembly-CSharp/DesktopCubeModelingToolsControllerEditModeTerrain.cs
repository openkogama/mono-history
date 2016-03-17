using MV.Common;

public class DesktopCubeModelingToolsControllerEditModeTerrain : DesktopCubeModelingToolsController
{
	public override void SetupButtons()
	{
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			editCube.onClick.AddListener(() =>
			{
				SetToolActive(CubeModelingEvent.EditCubes);
			});
		}
		else
		{
			editCube.onClick.AddListener(() =>
			{
				SetToolActive(CubeModelingEvent.EditCubes2D);
			});
		}
		deletecube.onClick.AddListener(() =>
		{
			SetToolActive(CubeModelingEvent.DeleteCubes);
		});
		paintCube.onClick.AddListener(() =>
		{
			SetToolActive(CubeModelingEvent.PaintCubes);
		});
	}
}
