using System;
using UnityEngine;

public abstract class UXComboBoxItem : UXLine
{
	public delegate void OnItemClickDelegate(UXComboBoxItem comboBoxItem);

	public OnItemClickDelegate OnItemClick;

	public Material BGMaterial;

	protected UXPlane Background;

	private bool mouseOver;

	private bool mouseDown;

	public virtual void BuildItem()
	{
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.size = new Vector3(Width, Height, 1f);
		UXMouseClickObject uXMouseClickObject = gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePos) =>
		{
			if (GetClippedBounds().Contains(mousePos))
			{
				mouseDown = true;
			}
			return false;
		}));
		uXMouseClickObject.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(uXMouseClickObject.OnMouseUp, (UXMouseClickObject.OnMouseUpDelegate)((UXMouseClickObject clickObject, Vector3 mousePos) =>
		{
			if (Visible && mouseDown && OnItemClick != null)
			{
				OnItemClick(this);
			}
			mouseDown = false;
		}));
		CreateBackground();
	}

	public override void SetAlpha(float alpha, string materialProperty)
	{
		Background.SetAlpha(alpha, materialProperty);
	}

	public void OnMouseOver()
	{
		mouseOver = true;
	}

	public override void Update()
	{
		base.Update();
		if (mouseOver)
		{
			UpdateMouseOver(mouseOver);
			mouseOver = false;
		}
		else
		{
			UpdateMouseOver(mouseOver);
		}
	}

	protected virtual void CreateBackground()
	{
		GameObject gameObject = new GameObject("BG");
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = transform;
		gameObject.AddComponent<MeshRenderer>().material = new Material(BGMaterial);
		Background = gameObject.AddComponent<UXPlane>();
		Background.materialProperty = "_MainColor";
		Background.SetSize(Width, Height + 0.1f);
	}

	protected virtual void UpdateMouseOver(bool mouseOver)
	{
		Background.SetColor((!mouseOver) ? Color.white : Color.gray, string.Empty);
	}

	public abstract GameObject CreateClone();

	public abstract object GetValue();
}
