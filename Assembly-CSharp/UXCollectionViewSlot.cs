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
		UXMouseOverObject uXMouseOverObject = UXUtils.AddComponentIfNotExists<UXMouseOverObject>(gameObject);
		uXMouseOverObject.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(HandleMouseOverEnter));
		uXMouseOverObject.OnMouseOver = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOver, new UXMouseOverObject.OnMouseOverDelegate(HandleMouseOver));
		uXMouseOverObject.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOverExit, new UXMouseOverObject.OnMouseOverDelegate(HandleMouseOverExit));
		UXMouseClickObject uXMouseClickObject = UXUtils.AddComponentIfNotExists<UXMouseClickObject>(gameObject);
		uXMouseClickObject.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(uXMouseClickObject.OnMouseUp, new UXMouseClickObject.OnMouseUpDelegate(HandleMouseClick));
		UXDragObject uXDragObject = UXUtils.AddComponentIfNotExists<UXDragObject>(gameObject);
		uXDragObject.OnDragStart = (UXDragObject.OnDragStartDelegate)Delegate.Combine(uXDragObject.OnDragStart, new UXDragObject.OnDragStartDelegate(HandleMouseDragStart));
		uXDragObject.OnDrag = (UXDragObject.OnDragDelegate)Delegate.Combine(uXDragObject.OnDrag, new UXDragObject.OnDragDelegate(HandleMouseDrag));
		uXDragObject.OnDragStop = (UXDragObject.OnDragStopDelegate)Delegate.Combine(uXDragObject.OnDragStop, new UXDragObject.OnDragStopDelegate(HandleMouseDragStop));
		UXDropObject uXDropObject = UXUtils.AddComponentIfNotExists<UXDropObject>(gameObject);
		uXDropObject.AcceptDrop = (UXDropObject.AcceptDropDelegate)Delegate.Combine(uXDropObject.AcceptDrop, new UXDropObject.AcceptDropDelegate(HandleMouseDrop));
		mouseOverScale = GetComponent<UXSimpleMouseOverScale>();
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
		gameObject.GetComponent<UXPlane>().SetVisible(visible);
		foreach (KeyValuePair<int, UXCollectionViewItem> pageIndexToViewItem in _pageIndexToViewItems)
		{
			pageIndexToViewItem.Value.SetVisible(Visible && pageIndexToViewItem.Key == _visiblePage);
		}
		Reset();
	}

	public void Reset()
	{
		if (mouseOverScale != null)
		{
			mouseOverScale.Reset();
		}
	}

	public void AttachViewItem(UXCollectionViewItem viewItem, int pageIndex)
	{
		if (_pageIndexToViewItems.ContainsKey(pageIndex))
		{
			Debug.LogWarning("Trying to add view item to occupied slot");
			return;
		}
		_pageIndexToViewItems.Add(pageIndex, viewItem);
		viewItem.transform.parent = transform;
		viewItem.transform.localPosition = Vector3.zero;
		viewItem.transform.localScale = Vector3.one;
		if (_visiblePage == pageIndex && !viewItem.IsInitialized)
		{
			viewItem.Initialize();
		}
		viewItem.PageIndex = pageIndex;
		viewItem.OnAttachToSlot(this);
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
		UXCollectionViewItem uXCollectionViewItem = _pageIndexToViewItems[pageIndex];
		_pageIndexToViewItems.Remove(pageIndex);
		Reset();
		if (detachAndDestroy)
		{
			if (uXCollectionViewItem != null)
			{
				uXCollectionViewItem.OnDetachFromSlot(this);
				UnityEngine.Object.Destroy(uXCollectionViewItem.gameObject);
			}
			return null;
		}
		uXCollectionViewItem.transform.parent = transform.parent.parent;
		uXCollectionViewItem.transform.localPosition = Vector3.zero;
		uXCollectionViewItem.transform.localScale = Vector3.one;
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
		if (OnSlotMouseDragStart != null)
		{
			OnSlotMouseDragStart(SlotIndex, mousePositionWorld);
		}
		return OnSlotMouseDragStart != null;
	}

	public void HandleMouseDrag(Vector3 mousePositionWorld)
	{
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
