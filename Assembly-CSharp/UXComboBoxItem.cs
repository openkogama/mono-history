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
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		BoxCollider val = ((Component)this).gameObject.AddComponent<BoxCollider>();
		val.size = new Vector3(Width, Height, 1f);
		UXMouseClickObject uXMouseClickObject = ((Component)this).gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePos) =>
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			Rect clippedBounds = GetClippedBounds();
			if (clippedBounds.Contains(mousePos))
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
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected Obj, but got Unknown
		GameObject val = new GameObject("BG");
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = ((Component)this).transform;
		((Renderer)val.AddComponent<MeshRenderer>()).material = new Material(BGMaterial);
		Background = val.AddComponent<UXPlane>();
		Background.materialProperty = "_MainColor";
		Background.SetSize(Width, Height + 0.1f);
	}

	protected virtual void UpdateMouseOver(bool mouseOver)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Background.SetColor((!mouseOver) ? Color.white : Color.gray, string.Empty);
	}

	public abstract GameObject CreateClone();

	public abstract object GetValue();
}
