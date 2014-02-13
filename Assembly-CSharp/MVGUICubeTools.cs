using System;
using UnityEngine;

public class MVGUICubeTools : UXViewScript
{
	public UXToggleIconButton editCubeButton;

	public UXToggleIconButton deleteCubeButton;

	public UXToggleIconButton paintCubeButton;

	public UXToggleIconButton sprayPaintCubeButton;

	public override void OnInitialize()
	{
		UXToggleIconButton uXToggleIconButton = editCubeButton;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool toggle) =>
		{
			if (!toggle)
			{
				editCubeButton.SetToggleState(toggle: true);
			}
			else
			{
				deleteCubeButton.SetToggleState(toggle: false);
				paintCubeButton.SetToggleState(toggle: false);
				sprayPaintCubeButton.SetToggleState(toggle: false);
			}
		}));
		UXToggleIconButton uXToggleIconButton2 = deleteCubeButton;
		uXToggleIconButton2.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton2.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool toggle) =>
		{
			if (!toggle)
			{
				deleteCubeButton.SetToggleState(toggle: true);
			}
			else
			{
				editCubeButton.SetToggleState(toggle: false);
				paintCubeButton.SetToggleState(toggle: false);
				sprayPaintCubeButton.SetToggleState(toggle: false);
			}
		}));
		UXToggleIconButton uXToggleIconButton3 = paintCubeButton;
		uXToggleIconButton3.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton3.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool toggle) =>
		{
			if (!toggle)
			{
				paintCubeButton.SetToggleState(toggle: true);
			}
			else
			{
				editCubeButton.SetToggleState(toggle: false);
				deleteCubeButton.SetToggleState(toggle: false);
				sprayPaintCubeButton.SetToggleState(toggle: false);
			}
		}));
		UXToggleIconButton uXToggleIconButton4 = sprayPaintCubeButton;
		uXToggleIconButton4.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton4.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool toggle) =>
		{
			if (!toggle)
			{
				sprayPaintCubeButton.SetToggleState(toggle: true);
			}
			else
			{
				editCubeButton.SetToggleState(toggle: false);
				deleteCubeButton.SetToggleState(toggle: false);
				paintCubeButton.SetToggleState(toggle: false);
			}
		}));
	}

	public override void OnShow()
	{
		base.OnShow();
		((Component)editCubeButton).gameObject.SetActiveRecursively(true);
		((Component)deleteCubeButton).gameObject.SetActiveRecursively(true);
		((Component)paintCubeButton).gameObject.SetActiveRecursively(true);
		((Component)sprayPaintCubeButton).gameObject.SetActiveRecursively(true);
	}

	public override void OnHide()
	{
		base.OnHide();
		((Component)editCubeButton).gameObject.SetActiveRecursively(false);
		((Component)deleteCubeButton).gameObject.SetActiveRecursively(false);
		((Component)paintCubeButton).gameObject.SetActiveRecursively(false);
		((Component)sprayPaintCubeButton).gameObject.SetActiveRecursively(false);
	}
}
