using MV.Common;
using UnityEngine;

public class EditorController2D : EditorController
{
	private readonly DrawPlaneController2D drawPlaneController = new DrawPlaneController2D();

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
		if (!_isInEditMode)
		{
			drawPlaneController.SetToTerrain(drawPlaneController.IsDrawPlaneActive);
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		drawPlaneController.SetToTerrain(active: true);
		if (!editorToggles.gridSnapToggle.ToggleState)
		{
			editorToggles.gridSnapToggle.Toggle();
		}
	}

	protected override void Show()
	{
		Debug.Log("Show2D");
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

	public override void EnterCubeModelEdit(float scale)
	{
		Debug.Log("EnterCubeModelEdit");
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit);
		drawPlaneController.InputEnabled = true;
		base.EnterCubeModelEdit(scale);
	}

	public override void LeaveCubeModelEdit()
	{
		Show();
		_isInEditMode = false;
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit2D);
		MVGameControllerBase.CameraController.StartTransitionCam(0.5f);
		drawPlaneController.SetToTerrain(active: true);
	}
}
