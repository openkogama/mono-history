using System;
using Localize;
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected Obj, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)ItemImagePlane != (Object)null))
		{
			GameObject val = new GameObject("Image Plane");
			val.layer = LayerMask.NameToLayer("UXElement");
			val.transform.parent = ((Component)this).transform;
			val.transform.localScale = Vector3.one;
			val.transform.localPosition = new Vector3(0f, 0f, -0.01f);
			ItemImagePlane = val.AddComponent<UXPlane>();
			ItemImagePlane.SetSize(Width, Height);
			ItemImagePlane.SetVisible(Visible);
		}
	}

	protected void AddTooltip(string tooltipText)
	{
		if (!(tooltipText == string.Empty))
		{
			tooltip = ((Component)ItemImagePlane).gameObject.GetComponent<UXToolTip>();
			if ((Object)(object)tooltip == (Object)null)
			{
				tooltip = ((Component)ItemImagePlane).gameObject.AddComponent<UXToolTip>();
			}
			tooltip.toolTipTextID = TextSlotIndex.Empty;
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
		if (PageIndex == visiblePage && (Object)(object)tooltip != (Object)null && ((Component)this).gameObject.active)
		{
			tooltip.OnMouseOverEnter(null);
		}
	}

	public override void OnMouseSlotOver(int slotIndex, int visiblePage)
	{
		if (PageIndex == visiblePage && (Object)(object)tooltip != (Object)null && ((Component)this).gameObject.active)
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
		if ((Object)(object)ItemImagePlane != (Object)null)
		{
			ItemImagePlane.SetVisible(visible);
		}
		if ((Object)(object)LoadingCircle != (Object)null)
		{
			LoadingCircle.SetVisible(visible && _loading);
		}
	}

	public virtual void Update()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (_loading)
		{
			((Component)LoadingCircle).transform.RotateAroundLocal(Vector3.forward, 20f * Time.deltaTime);
		}
	}
}
