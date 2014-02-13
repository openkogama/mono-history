using System;
using UnityEngine;

public class MVGUICurrentSelectedMaterialCube : MonoBehaviour
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
			foreach (MeshRenderer val in array)
			{
				((Renderer)val).material = value;
			}
		}
	}

	public void Awake()
	{
		meshRenderers = ((Component)this).GetComponentsInChildren<MeshRenderer>(true);
		materialSelectionActivationClickObject = ((Component)this).GetComponentInChildren<UXMouseClickObject>();
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
}
