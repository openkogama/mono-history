public class DesktopCubeModelingToolsControllerEditModeTerrain : DesktopCubeModelingToolsController
{
	public override void SetupButtons()
	{
		editCube.onClick.AddListener(() =>
		{
			SetToolActive(CubeModelingEvent.EditCubes);
		});
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
