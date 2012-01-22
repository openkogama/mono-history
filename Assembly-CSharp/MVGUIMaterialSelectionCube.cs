using System;
using UnityEngine;

public class MVGUIMaterialSelectionCube : MonoBehaviour
{
	public OnMaterialDelegate OnSelection;

	public OnMaterialDelegate OnMouseOver;

	public MeshRenderer cubeRenderer;

	public byte materialId;

	public Material Material
	{
		set
		{
			((Renderer)cubeRenderer).material = value;
		}
	}

	public void Awake()
	{
		UXMouseClickObject component = ((Component)this).GetComponent<UXMouseClickObject>();
		component.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnSelection != null)
			{
				OnSelection(materialId);
			}
		}));
		UXMouseOverObject component2 = ((Component)this).GetComponent<UXMouseOverObject>();
		component2.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverEnterDelegate)Delegate.Combine(component2.OnMouseOverEnter, (UXMouseOverObject.OnMouseOverEnterDelegate)((UXMouseOverObject o) =>
		{
			if (OnMouseOver != null)
			{
				OnMouseOver(materialId);
			}
		}));
	}
}
