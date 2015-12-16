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
		MVMaterial material = MVGameControllerBase.Game.MaterialRepository.GetMaterial(MVGameControllerLegacyUI.EditorController.EditorStateMachine.CubeModelingStateMachine.CurrentMaterialId);
		if (material.IsDestructible)
		{
			MVGameControllerLegacyUI.EditorController.EditorStateMachine.CubeModelingStateMachine.CurrentMaterialId = 21;
		}
		SetMaterial(MVGameControllerLegacyUI.EditorController.EditorStateMachine.CubeModelingStateMachine.CurrentMaterial, MVGameControllerBase.Game.MaterialRepository.GetMaterial(MVGameControllerLegacyUI.EditorController.EditorStateMachine.CubeModelingStateMachine.CurrentMaterialId));
		UXFullscreenColliderBox.Instance.AddBlockingObject(this);
	}

	private void SetMaterial(Material material, MVMaterial mvMaterial)
	{
		smallCube.GetComponent<Renderer>().sharedMaterial = material;
		mediumCube.GetComponent<Renderer>().sharedMaterial = material;
		largeCube.GetComponent<Renderer>().sharedMaterial = material;
		smallCube.GetComponent<MeshFilter>().sharedMesh = mvMaterial.mesh;
		mediumCube.GetComponent<MeshFilter>().sharedMesh = mvMaterial.mesh;
		largeCube.GetComponent<MeshFilter>().sharedMesh = mvMaterial.mesh;
	}

	private void CreateAndHide(float size)
	{
		MVGameControllerLegacyUI.EditorController.EditorWorldObjectCreation.OnAddNewPrototype(string.Empty, size);
		View.Hide();
	}
}
