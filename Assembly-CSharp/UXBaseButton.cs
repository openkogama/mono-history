using System;
using UnityEngine;

[RequireComponent(typeof(UXMouseClickObject))]
public abstract class UXBaseButton : UXGUIElement
{
	public delegate void OnClickDelegate();

	public OnClickDelegate OnClick;

	public bool buttonEnabled = true;

	public bool clickThrough;

	private bool click;

	public override void Awake()
	{
		base.Awake();
		Initialize();
	}

	protected virtual void Initialize()
	{
		if (GetComponent<MeshFilter>() == null)
		{
			gameObject.AddComponent<MeshFilter>();
		}
		BuildMesh(gameObject.GetComponent<MeshFilter>().mesh);
		if (GetComponent<BoxCollider>() == null)
		{
			gameObject.AddComponent<BoxCollider>();
		}
		UXMouseClickObject component = GetComponent<UXMouseClickObject>();
		component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => !clickThrough));
		component.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			click = true;
		}));
		component.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(component.OnMouseUp, new UXMouseClickObject.OnMouseUpDelegate(HandleOnUp));
		UXMouseOverHighlight component2 = GetComponent<UXMouseOverHighlight>();
		if (component2 != null)
		{
			component2.AddMaterial(GetComponent<Renderer>().material);
		}
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);
		BoxCollider boxCollider = UXUtils.AddComponentIfNotExists<BoxCollider>(gameObject);
		boxCollider.size = new Vector3(Width, Height, 0.5f);
	}

	private void HandleOnUp(UXMouseClickObject mouseClickObject, Vector3 mousePositionWorld)
	{
		if (click)
		{
			NotifyOnClick();
			click = false;
		}
	}

	public void FireOnClick()
	{
		NotifyOnClick();
	}

	private void NotifyOnClick()
	{
		if (OnClick != null && buttonEnabled)
		{
			OnClick();
		}
	}
}
