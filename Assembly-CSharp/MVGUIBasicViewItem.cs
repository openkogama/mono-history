using System;
using UnityEngine;

public abstract class MVGUIBasicViewItem : UXCollectionViewItem
{
	public delegate void OnViewItemBuiltDelegate(MVGUIBasicViewItem viewItem);

	private const float LOADING_CIRCLE_SPEED = 20f;

	public OnViewItemBuiltDelegate OnViewItemBuilt;

	public Material ItemPreviewMaterial;

	public UXPlane LoadingCircle;

	protected UXToolTip tooltip;

	protected bool _loading;

	public override IUXCollectionItem Item { get; set; }

	public UXPlane ItemImagePlane { get; private set; }

	protected void BuildImagePlane()
	{
		if (!(ItemImagePlane != null))
		{
			GameObject gameObject = new GameObject("Image Plane");
			gameObject.layer = LayerMask.NameToLayer("UXElement");
			gameObject.transform.parent = transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = new Vector3(0f, 0f, -0.01f);
			ItemImagePlane = gameObject.AddComponent<UXPlane>();
			ItemImagePlane.SetSize(Width, Height);
			ItemImagePlane.SetVisible(Visible);
		}
	}

	protected void AddTooltip(string tooltipText)
	{
		if (!(tooltipText == string.Empty))
		{
			tooltip = ItemImagePlane.gameObject.GetComponent<UXToolTip>();
			if (tooltip == null)
			{
				tooltip = ItemImagePlane.gameObject.AddComponent<UXToolTip>();
			}
			tooltip.toolTipText = tooltipText;
		}
	}

	public override void OnAttachToSlot(UXCollectionViewSlot slot)
	{
		slot.OnSlotMouseOverEnter = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Combine(slot.OnSlotMouseOverEnter, new UXCollectionViewSlot.SlotEventDelegate(OnMouseSlotOverEnter));
		slot.OnSlotMouseOver = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Combine(slot.OnSlotMouseOver, new UXCollectionViewSlot.SlotEventDelegate(OnMouseSlotOver));
		slot.OnSlotMouseOverExit = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Combine(slot.OnSlotMouseOverExit, new UXCollectionViewSlot.SlotEventDelegate(OnMouseSlotOverExit));
		if (IsInitialized)
		{
			ItemImagePlane.ignoreClipping = false;
		}
	}

	public override void OnDetachFromSlot(UXCollectionViewSlot slot)
	{
		slot.OnSlotMouseOverEnter = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Remove(slot.OnSlotMouseOverEnter, new UXCollectionViewSlot.SlotEventDelegate(OnMouseSlotOverEnter));
		slot.OnSlotMouseOver = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Remove(slot.OnSlotMouseOver, new UXCollectionViewSlot.SlotEventDelegate(OnMouseSlotOver));
		slot.OnSlotMouseOverExit = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Remove(slot.OnSlotMouseOverExit, new UXCollectionViewSlot.SlotEventDelegate(OnMouseSlotOverExit));
		if (IsInitialized)
		{
			ItemImagePlane.ignoreClipping = true;
		}
	}

	public override void OnMouseSlotOverEnter(int slotIndex, int visiblePage)
	{
		if (PageIndex == visiblePage && tooltip != null && gameObject.activeInHierarchy)
		{
			tooltip.OnMouseOverEnter(null);
		}
	}

	public override void OnMouseSlotOver(int slotIndex, int visiblePage)
	{
		if (PageIndex == visiblePage && tooltip != null && gameObject.activeInHierarchy)
		{
			tooltip.HandleOnMouseOver(null);
		}
	}

	public override void OnMouseSlotOverExit(int slotIndex, int visiblePage)
	{
	}

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		if (ItemImagePlane != null)
		{
			ItemImagePlane.SetVisible(visible);
		}
		if (LoadingCircle != null)
		{
			LoadingCircle.SetVisible(visible && _loading);
		}
	}

	public virtual void Update()
	{
		if (_loading)
		{
			LoadingCircle.transform.Rotate(Vector3.forward, 20f * Time.deltaTime * 57.29578f);
		}
	}
}
