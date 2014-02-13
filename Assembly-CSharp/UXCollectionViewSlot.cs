using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UXCollectionViewSlot : MonoBehaviour
{
	public delegate void SlotEventDelegate(int slotIndex, int visiblePage);

	public delegate void SlotDragEventDelegate(int slotIndex, Vector3 dragPos);

	public SlotEventDelegate OnSlotMouseClick;

	public SlotEventDelegate OnSlotMouseDrop;

	public SlotEventDelegate OnSlotMouseOverEnter;

	public SlotEventDelegate OnSlotMouseOver;

	public SlotEventDelegate OnSlotMouseOverExit;

	public SlotDragEventDelegate OnSlotMouseDragStart;

	public SlotDragEventDelegate OnSlotMouseDrag;

	public float Width;

	public float Height;

	public bool Visible;

	private Dictionary<int, UXCollectionViewItem> _pageIndexToViewItems = new Dictionary<int, UXCollectionViewItem>();

	private int _visiblePage;

	private UXSimpleMouseOverScale mouseOverScale;

	public int SlotIndex { get; set; }

	public void Start()
	{
		UXMouseOverObject uXMouseOverObject = UXUtils.AddComponentIfNotExists<UXMouseOverObject>(((Component)this).gameObject);
		uXMouseOverObject.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(HandleMouseOverEnter));
		uXMouseOverObject.OnMouseOver = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOver, new UXMouseOverObject.OnMouseOverDelegate(HandleMouseOver));
		uXMouseOverObject.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOverExit, new UXMouseOverObject.OnMouseOverDelegate(HandleMouseOverExit));
		UXMouseClickObject uXMouseClickObject = UXUtils.AddComponentIfNotExists<UXMouseClickObject>(((Component)this).gameObject);
		uXMouseClickObject.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(uXMouseClickObject.OnMouseUp, new UXMouseClickObject.OnMouseUpDelegate(HandleMouseClick));
		UXDragObject uXDragObject = UXUtils.AddComponentIfNotExists<UXDragObject>(((Component)this).gameObject);
		uXDragObject.OnDragStart = (UXDragObject.OnDragStartDelegate)Delegate.Combine(uXDragObject.OnDragStart, new UXDragObject.OnDragStartDelegate(HandleMouseDragStart));
		uXDragObject.OnDrag = (UXDragObject.OnDragDelegate)Delegate.Combine(uXDragObject.OnDrag, new UXDragObject.OnDragDelegate(HandleMouseDrag));
		uXDragObject.OnDragStop = (UXDragObject.OnDragStopDelegate)Delegate.Combine(uXDragObject.OnDragStop, new UXDragObject.OnDragStopDelegate(HandleMouseDragStop));
		UXDropObject uXDropObject = UXUtils.AddComponentIfNotExists<UXDropObject>(((Component)this).gameObject);
		uXDropObject.AcceptDrop = (UXDropObject.AcceptDropDelegate)Delegate.Combine(uXDropObject.AcceptDrop, new UXDropObject.AcceptDropDelegate(HandleMouseDrop));
		mouseOverScale = ((Component)this).GetComponent<UXSimpleMouseOverScale>();
	}

	public void SetVisiblePage(int pageIndex)
	{
		_visiblePage = pageIndex;
		foreach (KeyValuePair<int, UXCollectionViewItem> pageIndexToViewItem in _pageIndexToViewItems)
		{
			pageIndexToViewItem.Value.SetVisible(Visible && pageIndexToViewItem.Key == _visiblePage);
		}
		if (_pageIndexToViewItems.ContainsKey(pageIndex) && !_pageIndexToViewItems[pageIndex].IsInitialized)
		{
			_pageIndexToViewItems[pageIndex].Initialize();
		}
	}

	public void SetVisible(bool visible)
	{
		Visible = visible;
		((Component)this).gameObject.GetComponent<UXPlane>().SetVisible(visible);
		foreach (KeyValuePair<int, UXCollectionViewItem> pageIndexToViewItem in _pageIndexToViewItems)
		{
			pageIndexToViewItem.Value.SetVisible(Visible && pageIndexToViewItem.Key == _visiblePage);
		}
		Reset();
	}

	public void Reset()
	{
		if ((Object)(object)mouseOverScale != (Object)null)
		{
			mouseOverScale.Reset();
		}
	}

	public void AttachViewItem(UXCollectionViewItem viewItem, int pageIndex)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (_pageIndexToViewItems.ContainsKey(pageIndex))
		{
			Debug.LogWarning((object)"Trying to add view item to occupied slot");
			return;
		}
		_pageIndexToViewItems.Add(pageIndex, viewItem);
		((Component)viewItem).transform.parent = ((Component)this).transform;
		((Component)viewItem).transform.localPosition = Vector3.zero;
		((Component)viewItem).transform.localScale = Vector3.one;
		if (_visiblePage == pageIndex && !viewItem.IsInitialized)
		{
			viewItem.Initialize();
		}
		viewItem.PageIndex = pageIndex;
		viewItem.OnAttachToSlot(this);
		if (!((Component)this).gameObject.active)
		{
			((Component)viewItem).gameObject.SetActiveRecursively(false);
		}
		Reset();
	}

	public UXCollectionViewItem GetViewItem(int pageIndex)
	{
		if (_pageIndexToViewItems.ContainsKey(pageIndex))
		{
			return _pageIndexToViewItems[pageIndex];
		}
		return null;
	}

	public UXCollectionViewItem DetachViewItem(int pageIndex, bool detachAndDestroy = false)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		UXCollectionViewItem uXCollectionViewItem = _pageIndexToViewItems[pageIndex];
		_pageIndexToViewItems.Remove(pageIndex);
		Reset();
		if (detachAndDestroy)
		{
			if ((Object)(object)uXCollectionViewItem != (Object)null)
			{
				uXCollectionViewItem.OnDetachFromSlot(this);
				Object.Destroy((Object)(object)((Component)uXCollectionViewItem).gameObject);
			}
			return null;
		}
		((Component)uXCollectionViewItem).transform.parent = ((Component)this).transform.parent.parent;
		((Component)uXCollectionViewItem).transform.localPosition = Vector3.zero;
		((Component)uXCollectionViewItem).transform.localScale = Vector3.one;
		uXCollectionViewItem.OnDetachFromSlot(this);
		return uXCollectionViewItem;
	}

	public List<UXCollectionViewItem> DetachAllViewItems(bool detachAndDestroy = false)
	{
		List<UXCollectionViewItem> list = new List<UXCollectionViewItem>();
		List<int> list2 = _pageIndexToViewItems.Keys.ToList();
		foreach (int item in list2)
		{
			list.Add(DetachViewItem(item, detachAndDestroy));
		}
		Reset();
		return list;
	}

	public void HandleMouseOverEnter(UXMouseOverObject mouseOverObject)
	{
		if (OnSlotMouseOverEnter != null)
		{
			OnSlotMouseOverEnter(SlotIndex, _visiblePage);
		}
	}

	public void HandleMouseOver(UXMouseOverObject mouseOverObject)
	{
		if (OnSlotMouseOver != null)
		{
			OnSlotMouseOver(SlotIndex, _visiblePage);
		}
	}

	public void HandleMouseOverExit(UXMouseOverObject mouseOverObject)
	{
		if (OnSlotMouseOverExit != null)
		{
			OnSlotMouseOverExit(SlotIndex, _visiblePage);
		}
	}

	public void HandleMouseClick(UXMouseClickObject mouseClickObject, Vector3 mousePositionWorld)
	{
		if (OnSlotMouseClick != null)
		{
			OnSlotMouseClick(SlotIndex, _visiblePage);
		}
	}

	public bool HandleMouseDragStart(Vector3 mousePositionWorld)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (OnSlotMouseDragStart != null)
		{
			OnSlotMouseDragStart(SlotIndex, mousePositionWorld);
		}
		return OnSlotMouseDragStart != null;
	}

	public void HandleMouseDrag(Vector3 mousePositionWorld)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (OnSlotMouseDrag != null)
		{
			OnSlotMouseDrag(-1, mousePositionWorld);
		}
	}

	public void HandleMouseDragStop(Vector3 dropPos, bool accept)
	{
		if (OnSlotMouseDrag != null)
		{
			OnSlotMouseDrop(-1, -1);
		}
	}

	public bool HandleMouseDrop(GameObject drop)
	{
		if (OnSlotMouseDrop != null)
		{
			OnSlotMouseDrop(SlotIndex, _visiblePage);
		}
		return true;
	}
}
