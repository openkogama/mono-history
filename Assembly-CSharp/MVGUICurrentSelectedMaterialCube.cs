using System;
using UnityEngine;

public class MVGUICurrentSelectedMaterialCube : UXViewScript
{
	public delegate void OnClickDelegate();

	public OnClickDelegate OnClick;

	private MeshRenderer[] meshRenderers;

	private UXMouseClickObject materialSelectionActivationClickObject;

	public Material CurrentMaterial
	{
		set
		{
			MeshRenderer[] array = meshRenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.material = value;
			}
		}
	}

	public override void Awake()
	{
		base.Awake();
		meshRenderers = GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		materialSelectionActivationClickObject = GetComponentInChildren<UXMouseClickObject>();
		UXMouseClickObject uXMouseClickObject = materialSelectionActivationClickObject;
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true));
		UXMouseClickObject uXMouseClickObject2 = materialSelectionActivationClickObject;
		uXMouseClickObject2.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(uXMouseClickObject2.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnClick != null)
			{
				OnClick();
			}
		}));
	}

	public override void OnShow()
	{
		base.OnShow();
		MeshRenderer[] array = meshRenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = true;
		}
	}

	public override void OnHide()
	{
		base.OnHide();
		MeshRenderer[] array = meshRenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = false;
		}
	}
}
