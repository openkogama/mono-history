public class DrawPlaneController2D : DrawPlaneController
{
	public void SetToTerrain(bool active)
	{
		worldEditorDrawPlane.Active = active;
		worldEditorDrawPlane.DrawPlaneVisualization.SetActive(value: false);
		Orientation = DrawPlaneAxis.Z;
		worldEditorDrawPlane.SetDrawPlaneHeight(0.5f);
		workplaneArrows.View.Hide();
		InputEnabled = false;
	}
}
