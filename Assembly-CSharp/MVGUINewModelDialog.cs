using System;
using UnityEngine;

public class MVGUINewModelDialog : UXViewScript
{
	public GameObject smallButton;

	public GameObject mediumButton;

	public GameObject largeButton;

	public GameObject smallCube;

	public GameObject mediumCube;

	public GameObject largeCube;

	public UXWindow newModelWindow;

	public override void OnInitialize()
	{
		UXMouseClickObject component = smallButton.GetComponent<UXMouseClickObject>();
		component.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject click, Vector3 pos) =>
		{
			CreateAndHide(0.25f);
		}));
		UXMouseClickObject component2 = mediumButton.GetComponent<UXMouseClickObject>();
		component2.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component2.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject click, Vector3 pos) =>
		{
			CreateAndHide(0.5f);
		}));
		UXMouseClickObject component3 = largeButton.GetComponent<UXMouseClickObject>();
		component3.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component3.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject click, Vector3 pos) =>
		{
			CreateAndHide(1f);
		}));
		UXWindow uXWindow = newModelWindow;
		uXWindow.OnExitButtonClick = (UXWindow.OnExitButtonClickDelegate)Delegate.Combine(uXWindow.OnExitButtonClick, (UXWindow.OnExitButtonClickDelegate)(() =>
		{
			View.Hide();
		}));
	}

	public override void OnHide()
	{
		base.OnHide();
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(this);
	}

	public override void OnShow()
	{
		base.OnShow();
		SetMaterial(MVGameController.Instance.EditController.EditorStateMachine.CubeModelingStateMachine.CurrentMaterial);
		UXFullscreenColliderBox.Instance.AddBlockingObject(this);
	}

	private void SetMaterial(Material material)
	{
		smallCube.renderer.material = material;
		mediumCube.renderer.material = material;
		largeCube.renderer.material = material;
	}

	private void CreateAndHide(float size)
	{
		MVGameController.Instance.EditController.EditorWorldObjectCreation.OnAddNewPrototype(string.Empty, size);
		View.Hide();
	}
}
