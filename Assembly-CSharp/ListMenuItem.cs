using System;
using UnityEngine;

public abstract class ListMenuItem : MonoBehaviour
{
	protected bool mouseOver;

	private bool mouseDown;

	protected ListMenu.OnMenuItemClickDelegate OnMenuItemClick;

	private GameObject mouseOverObject;

	protected float width;

	public void BuildMenuItem(float width, ListMenu.OnMenuItemClickDelegate OnMenuItemClick)
	{
		this.width = width;
		this.OnMenuItemClick = OnMenuItemClick;
		Initialize();
	}

	protected virtual void Initialize()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		InitializeMouseOverObject();
		BoxCollider val = ((Component)this).gameObject.AddComponent<BoxCollider>();
		val.size = new Vector3(width, GetHeight(), 0.1f);
		UXMouseClickObject uXMouseClickObject = ((Component)this).gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject uxClickObject, Vector3 mousePos) =>
		{
			mouseDown = true;
			return false;
		}));
		uXMouseClickObject.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(uXMouseClickObject.OnMouseUp, (UXMouseClickObject.OnMouseUpDelegate)((UXMouseClickObject uxClickObject, Vector3 mousePos) =>
		{
			if (mouseDown)
			{
				mouseDown = false;
				OnClick();
			}
		}));
	}

	private void InitializeMouseOverObject()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected Obj, but got Unknown
		mouseOverObject = new GameObject("MouseOver");
		mouseOverObject.transform.parent = ((Component)this).transform;
		mouseOverObject.transform.localPosition = Vector3.zero;
		mouseOverObject.transform.localScale = Vector3.one;
		mouseOverObject.transform.localRotation = Quaternion.identity;
		MeshRenderer val = mouseOverObject.AddComponent<MeshRenderer>();
		((Renderer)val).material = (Material)Resources.Load("Materials/UX/ListMenu/MouseOver");
		MeshFilter val2 = mouseOverObject.AddComponent<MeshFilter>();
		val2.mesh = UXUtils.BuildPlaneMesh(width, GetHeight(), "MenuItemMouseOverMesh");
	}

	public abstract float GetHeight();

	public void OnMouseOver()
	{
		mouseOver = true;
	}

	private void OnClick()
	{
		if (OnMenuItemClick != null)
		{
			OnMenuItemClick();
		}
	}

	private void Update()
	{
		if (mouseOver)
		{
			UpdateMouseOver();
			mouseOver = false;
		}
		else
		{
			UpdateMouseOver();
		}
	}

	protected virtual void UpdateMouseOver()
	{
		mouseOverObject.active = mouseOver;
	}
}
