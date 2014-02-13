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
		((Component)this).gameObject.AddComponent<MeshFilter>().mesh = BuildMesh();
		if ((Object)(object)((Component)this).GetComponent<BoxCollider>() == (Object)null)
		{
			((Component)this).gameObject.AddComponent<BoxCollider>();
		}
		UXMouseClickObject component = ((Component)this).GetComponent<UXMouseClickObject>();
		component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => !clickThrough));
		component.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			click = true;
		}));
		component.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(component.OnMouseUp, new UXMouseClickObject.OnMouseUpDelegate(HandleOnUp));
		UXMouseOverHighlight component2 = ((Component)this).GetComponent<UXMouseOverHighlight>();
		if ((Object)(object)component2 != (Object)null)
		{
			component2.AddMaterial(((Component)this).renderer.material);
		}
	}

	public override void SetSize(float width, float height)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		base.SetSize(width, height);
		BoxCollider val = UXUtils.AddComponentIfNotExists<BoxCollider>(((Component)this).gameObject);
		val.size = new Vector3(Width, Height, 0.5f);
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
