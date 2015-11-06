using UnityEngine;

public class EditorController3D : EditorController
{
	private readonly DrawPlaneController drawPlaneController = new DrawPlaneController();

	public override DrawPlaneController DrawPlaneController => drawPlaneController;

	protected override void ResolveGUIElements()
	{
		base.ResolveGUIElements();
		GameObject gameObject = UXUtils.FindGUIObjectOfType<MVGUIEditor>().gameObject;
		drawPlaneController.ResolveCubeTools(gameObject);
	}

	public override void ToggleDrawPlane()
	{
		DrawPlaneController.ToggleDrawPlane();
		drawplaneToggle.drawplaneToggle.SetToggleState(drawPlaneController.IsDrawPlaneActive);
	}

	protected override void Show()
	{
		drawplaneToggle.View.Show();
		cubeModelingController.ShowCurrentSelectedMaterial();
		cubeModelingController.ShowEditorTools();
		editorToggles.View.Show();
		editorTools.View.Show();
		shopView.View.Show();
		publish.View.Show();
		playButton.View.Show();
		fullscreenToggle.View.Show();
		muteToggle.View.Show();
		if (MVGameControllerBase.Game.LocalPlayer.PlanetOwnershipTypeID == 2)
		{
			screenShot.View.Show();
		}
		gameInfo.View.Show();
		level.View.Show();
	}

	public override void LeaveCubeModelEdit()
	{
		Show();
		_isInEditMode = false;
	}
}
