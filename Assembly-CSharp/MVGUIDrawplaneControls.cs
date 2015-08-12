using System;
using UnityEngine;

public class MVGUIDrawplaneControls : UXViewScript
{
	public float FullScreenScale = 1f;

	public float WindowedScale = 0.6f;

	public Transform ScaleParent;

	public UXIconButton upArrow;

	public UXIconButton downArrow;

	public UXToggleIconButton switchX;

	public UXToggleIconButton switchY;

	public UXToggleIconButton switchZ;

	public UXText altitudeText;

	private AEditController editController;

	private bool first = true;

	public override void OnInitialize()
	{
		UXIconButton uXIconButton = upArrow;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			MoveDrawPlane(1);
		}));
		UXIconButton uXIconButton2 = downArrow;
		uXIconButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			MoveDrawPlane(-1);
		}));
		UXToggleIconButton uXToggleIconButton = switchX;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool toggle) =>
		{
			SelectOrientation(DrawPlaneAxis.X, switchX);
		}));
		UXToggleIconButton uXToggleIconButton2 = switchY;
		uXToggleIconButton2.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton2.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool toggle) =>
		{
			SelectOrientation(DrawPlaneAxis.Y, switchY);
		}));
		UXToggleIconButton uXToggleIconButton3 = switchZ;
		uXToggleIconButton3.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton3.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool toggle) =>
		{
			SelectOrientation(DrawPlaneAxis.Z, switchZ);
		}));
		UXScreen uXScreen = UXUtils.UXScreen;
		uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Combine(uXScreen.OnFullScreenChange, new UXScreen.OnFullScreenChangeDelegate(ScaleIcons));
		ScaleIcons(uXScreen.Fullscreen);
	}

	private void ScaleIcons(bool full)
	{
		float num = ((!full) ? WindowedScale : FullScreenScale);
		ScaleParent.localScale = new Vector3(num, num, num);
	}

	public new void OnDestroy()
	{
		base.OnDestroy();
		if (UXUtils.UXScreen != null)
		{
			UXScreen uXScreen = UXUtils.UXScreen;
			uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Remove(uXScreen.OnFullScreenChange, new UXScreen.OnFullScreenChangeDelegate(ScaleIcons));
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		if (first)
		{
			editController = MVGameController.EditController;
			CubeModelingController cubeModelingController = editController.CubeModelingController;
			cubeModelingController.OnDrawplaneAltitudeChanged = (CubeModelingController.OnWorkPlaneAltitudeChangedDelegate)Delegate.Combine(cubeModelingController.OnDrawplaneAltitudeChanged, new CubeModelingController.OnWorkPlaneAltitudeChangedDelegate(UpdateAltitudeText));
			UpdateAltitudeText(editController.WorldEditorDrawPlane.Altitude);
			first = false;
		}
	}

	private void SelectOrientation(DrawPlaneAxis axis, UXToggleIconButton toggle)
	{
		if (!toggle.ToggleState)
		{
			toggle.SetToggleState(toggle: true);
			return;
		}
		switchX.SetToggleState(toggle: false);
		switchY.SetToggleState(toggle: false);
		switchZ.SetToggleState(toggle: false);
		toggle.SetToggleState(toggle: true);
		editController.WorldEditorDrawPlane.Orientation = axis;
	}

	private void MoveDrawPlane(int dir)
	{
		editController.WorldEditorDrawPlane.MoveDrawPlane(dir);
	}

	private void UpdateAltitudeText(int altitude)
	{
		altitudeText.Text = string.Empty + altitude;
	}
}
