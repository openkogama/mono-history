public class DrawPlaneController2DUUI : DrawPlaneControllerUUI
{
	public void SetToTerrain(bool active)
	{
		worldEditorDrawPlane.Active = active;
		worldEditorDrawPlane.DrawPlaneVisualization.SetActive(active);
		Orientation = DrawPlaneAxis.Z;
		worldEditorDrawPlane.SetDrawPlaneHeight(0.5f);
		InputEnabled = false;
	}
}
