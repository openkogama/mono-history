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
		InitializeMouseOverObject();
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.size = new Vector3(width, GetHeight(), 0.1f);
		UXMouseClickObject uXMouseClickObject = gameObject.AddComponent<UXMouseClickObject>();
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
		mouseOverObject = new GameObject("MouseOver");
		mouseOverObject.transform.parent = transform;
		mouseOverObject.transform.localPosition = Vector3.zero;
		mouseOverObject.transform.localScale = Vector3.one;
		mouseOverObject.transform.localRotation = Quaternion.identity;
		MeshRenderer meshRenderer = mouseOverObject.AddComponent<MeshRenderer>();
		meshRenderer.material = (Material)Resources.Load("Materials/UX/ListMenu/MouseOver");
		MeshFilter meshFilter = mouseOverObject.AddComponent<MeshFilter>();
		UXUtils.BuildPlaneMesh(meshFilter.mesh, width, GetHeight(), "MenuItemMouseOverMesh");
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
		mouseOverObject.SetActive(mouseOver);
	}
}
