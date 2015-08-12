using System;
using UnityEngine;

public class UXSliderBar : UXPlane
{
	public delegate void OnSliderBarClickDelegate(Vector3 worldClickPos);

	public OnSliderBarClickDelegate OnSliderBarClick;

	private void Start()
	{
		UXMouseClickObject uXMouseClickObject = gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, new UXMouseClickObject.OnMouseDownDelegate(OnBarClick));
		gameObject.AddComponent<BoxCollider>();
		ignoreClipping = true;
		SetVisible(Visible);
	}

	private bool OnBarClick(UXMouseClickObject mouseClickObject, Vector3 worldMousePos)
	{
		if (OnSliderBarClick != null)
		{
			OnSliderBarClick(worldMousePos);
		}
		return true;
	}
}
