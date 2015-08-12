using System;

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
		editCubeButton.gameObject.SetActive(value: true);
		deleteCubeButton.gameObject.SetActive(value: true);
		paintCubeButton.gameObject.SetActive(value: true);
		sprayPaintCubeButton.gameObject.SetActive(value: true);
	}

	public override void OnHide()
	{
		base.OnHide();
		editCubeButton.gameObject.SetActive(value: false);
		deleteCubeButton.gameObject.SetActive(value: false);
		paintCubeButton.gameObject.SetActive(value: false);
		sprayPaintCubeButton.gameObject.SetActive(value: false);
	}
}
