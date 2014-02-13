using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UXCollectionView : UXGUIElement, IUXContainer
{
	public delegate void OnBasicItemEventDelegate(IUXCollectionItem item);

	public delegate void OnItemDragEndDelegate();

	public delegate void OnSwapItemsDelegate(IUXCollectionItem sourceItem, IUXCollectionItem destinationItem);

	public delegate void OnMoveItemDelegate(IUXCollectionItem sourceItem, int destinationIndex);

	public OnBasicItemEventDelegate OnItemSelection;

	public OnBasicItemEventDelegate OnRemoveItem;

	public OnBasicItemEventDelegate OnItemDragStart;

	public OnItemDragEndDelegate OnItemDragEnd;

	public OnBasicItemEventDelegate OnSlotMouseOverEnter;

	public OnBasicItemEventDelegate OnSlotMouseOverExit;

	public OnSwapItemsDelegate OnSwapItems;

	public OnMoveItemDelegate OnMoveItem;

	[SerializeField]
	private UXTextButton _prevButton;

	[SerializeField]
	private UXTextButton _nextButton;

	[SerializeField]
	protected UXText _indexText;

	[SerializeField]
	private GameObject _slotPrefab;

	[SerializeField]
	private Material _mouseOverIndicatorMaterial;

	[SerializeField]
	protected float _columnSpacing = 6f;

	[SerializeField]
	protected float _rowSpacing = 6f;

	[SerializeField]
	protected int _rows;

	[SerializeField]
	protected int _columns;

	[SerializeField]
	protected bool _allowMoveItems;

	[SerializeField]
	protected bool _dontShowExtraPage;

	private IUXCollection _collection;

	private bool _isInitialized;

	protected Transform _itemRoot;

	protected Transform _slotRoot;

	protected List<UXCollectionViewSlot> _slots = new List<UXCollectionViewSlot>();

	protected float _slotHeight;

	protected float _slotWidth;

	private UXPlane _mouseOverIndicator;

	private int _dragFromGlobalSlotIndex;

	private UXCollectionViewItem _dragItem;

	private int _currentSlotMouseOver;

	public Func<IUXCollectionItem, UXCollectionViewItem> InstansiateViewItem { get; set; }

	public IUXCollection Collection
	{
		get
		{
			return _collection;
		}
		set
		{
			if (_collection != value)
			{
				if (_collection != null)
				{
					IUXCollection collection = _collection;
					collection.OnCollectionChange = (OnCollectionChangeDelegate)Delegate.Remove(collection.OnCollectionChange, new OnCollectionChangeDelegate(OnCollectionChange));
				}
				_collection = value;
				if (_collection != null)
				{
					IUXCollection collection2 = _collection;
					collection2.OnCollectionChange = (OnCollectionChangeDelegate)Delegate.Combine(collection2.OnCollectionChange, new OnCollectionChangeDelegate(OnCollectionChange));
				}
				if (_isInitialized)
				{
					OnCollectionChange();
				}
			}
		}
	}

	private int MaxIndexInItems
	{
		get
		{
			int num = 0;
			if (_collection != null)
			{
				for (int i = 0; i < _collection.Count; i++)
				{
					num = Math.Max(num, _collection.GetItem(i).Index);
				}
			}
			return num;
		}
	}

	private int SlotsPerPage => _rows * _columns;

	private int Pages => MaxIndexInItems / SlotsPerPage + ((!_allowMoveItems || _dontShowExtraPage) ? 1 : 2);

	private int CurrentPage { get; set; }

	public bool RefreshOnNextEnable { get; set; }

	public virtual void Initialize()
	{
		CreateItemRoot();
		_slots = new List<UXCollectionViewSlot>();
		CreateSlotRoot();
		if (_collection != null)
		{
			MatchSlotsToRequired();
		}
		if (_collection != null)
		{
			AddViewItemsToSlots();
		}
		if ((Object)(object)_nextButton != (Object)null)
		{
			UXTextButton nextButton = _nextButton;
			nextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(nextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				Scroll(1);
			}));
			if (_allowMoveItems && !_dontShowExtraPage)
			{
				UXDropObject component = ((Component)_nextButton).GetComponent<UXDropObject>();
				component.OnDragOverEnter = (UXDropObject.OnDragOverEnterDelegate)Delegate.Combine(component.OnDragOverEnter, (UXDropObject.OnDragOverEnterDelegate)((GameObject dragObject) =>
				{
					HandleOnDragEnterButton(1);
				}));
				UXDropObject component2 = ((Component)_nextButton).GetComponent<UXDropObject>();
				component2.OnDragOverExit = (UXDropObject.OnDragOverExitDelegate)Delegate.Combine(component2.OnDragOverExit, (UXDropObject.OnDragOverExitDelegate)((GameObject dragObject) =>
				{
					HandleOnDragExitButton();
				}));
			}
		}
		if ((Object)(object)_prevButton != (Object)null)
		{
			UXTextButton prevButton = _prevButton;
			prevButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(prevButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				Scroll(-1);
			}));
			if (_allowMoveItems && !_dontShowExtraPage)
			{
				UXDropObject component3 = ((Component)_prevButton).GetComponent<UXDropObject>();
				component3.OnDragOverEnter = (UXDropObject.OnDragOverEnterDelegate)Delegate.Combine(component3.OnDragOverEnter, (UXDropObject.OnDragOverEnterDelegate)((GameObject dragObject) =>
				{
					HandleOnDragEnterButton(-1);
				}));
				UXDropObject component4 = ((Component)_prevButton).GetComponent<UXDropObject>();
				component4.OnDragOverExit = (UXDropObject.OnDragOverExitDelegate)Delegate.Combine(component4.OnDragOverExit, (UXDropObject.OnDragOverExitDelegate)((GameObject dragObject) =>
				{
					HandleOnDragExitButton();
				}));
			}
		}
		BuildMouseOverIndicator();
		if (_columns == 1)
		{
			_columnSpacing = _slotWidth;
		}
		if (_rows == 1)
		{
			_rowSpacing = _slotHeight;
		}
		SetSize((float)_columns * _columnSpacing, (float)_rows * _rowSpacing);
		RefreshSlotVisibility();
		UpdateIndexText();
		_isInitialized = true;
	}

	private void CreateItemRoot()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("ItemRoot");
		val.layer = LayerMask.NameToLayer("UXElement");
		_itemRoot = val.transform;
		_itemRoot.parent = ((Component)this).transform;
		_itemRoot.localPosition = GetItemRootStartPosition();
		_itemRoot.localScale = Vector3.one;
	}

	protected virtual Vector3 GetItemRootStartPosition()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		return ((float)(_columns - 1) * _columnSpacing * Vector3.left + (float)(_rows - 1) * _rowSpacing * Vector3.up) / 2f;
	}

	private void CreateSlotRoot()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("SlotRoot");
		val.layer = LayerMask.NameToLayer("UXElement");
		_slotRoot = val.transform;
		_slotRoot.parent = ((Component)_itemRoot).transform;
		_slotRoot.localPosition = Vector3.zero;
		_slotRoot.localScale = Vector3.one;
	}

	private void CreateSlots(int slots)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < slots; i++)
		{
			Object val = Object.Instantiate((Object)(object)_slotPrefab);
			GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
			((Object)val2).name = $"{((Object)this).name} - Slot {_slots.Count}";
			val2.transform.parent = ((Component)_slotRoot).transform;
			val2.transform.localPosition = GetSlotPosition(_slots.Count / _rows, _slots.Count % _rows);
			val2.transform.localScale = Vector3.one;
			UXCollectionViewSlot component = val2.GetComponent<UXCollectionViewSlot>();
			component.SlotIndex = _slots.Count;
			component.OnSlotMouseClick = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Combine(component.OnSlotMouseClick, new UXCollectionViewSlot.SlotEventDelegate(HandleOnSlotMouseClick));
			component.OnSlotMouseOverEnter = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Combine(component.OnSlotMouseOverEnter, new UXCollectionViewSlot.SlotEventDelegate(HandleOnSlotMouseOverEnter));
			component.OnSlotMouseOver = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Combine(component.OnSlotMouseOver, new UXCollectionViewSlot.SlotEventDelegate(HandleOnSlotMouseOver));
			component.OnSlotMouseOverExit = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Combine(component.OnSlotMouseOverExit, new UXCollectionViewSlot.SlotEventDelegate(HandleOnSlotMouseOverExit));
			if (_allowMoveItems)
			{
				component.OnSlotMouseDragStart = (UXCollectionViewSlot.SlotDragEventDelegate)Delegate.Combine(component.OnSlotMouseDragStart, new UXCollectionViewSlot.SlotDragEventDelegate(HandleOnSlotMouseDragStart));
				component.OnSlotMouseDrag = (UXCollectionViewSlot.SlotDragEventDelegate)Delegate.Combine(component.OnSlotMouseDrag, new UXCollectionViewSlot.SlotDragEventDelegate(HandleOnSlotMouseDrag));
				component.OnSlotMouseDrop = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Combine(component.OnSlotMouseDrop, new UXCollectionViewSlot.SlotEventDelegate(HandleOnSlotMouseDrop));
			}
			if (_slotWidth == 0f || _slotHeight == 0f)
			{
				_slotWidth = component.Width;
				_slotHeight = component.Height;
				if ((Object)(object)_mouseOverIndicator != (Object)null)
				{
					_mouseOverIndicator.SetSize(_slotWidth, _slotHeight);
				}
			}
			_slots.Add(component);
		}
	}

	private void RemoveSlots(int slots)
	{
		for (int i = 0; i < slots; i++)
		{
			if (_slots.Count == 0)
			{
				break;
			}
			UXCollectionViewSlot uXCollectionViewSlot = _slots[_slots.Count - 1];
			Object.Destroy((Object)(object)((Component)uXCollectionViewSlot).gameObject);
			_slots.RemoveAt(_slots.Count - 1);
		}
	}

	private Vector3 GetSlotPosition(int column, int row)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (_columns == 1)
		{
			row += column * _rows;
			column = 0;
		}
		return (float)row * _rowSpacing * Vector3.down + (float)column * _columnSpacing * Vector3.right + -0.01f * Vector3.forward;
	}

	private void AddViewItemsToSlots()
	{
		Dictionary<object, UXCollectionViewItem> dictionary = new Dictionary<object, UXCollectionViewItem>();
		foreach (UXCollectionViewSlot slot in _slots)
		{
			foreach (UXCollectionViewItem item2 in slot.DetachAllViewItems())
			{
				dictionary.Add(item2.Item.Object, item2);
			}
		}
		for (int i = 0; i < _collection.Count; i++)
		{
			IUXCollectionItem item = _collection.GetItem(i);
			if (!dictionary.ContainsKey(item.Object))
			{
				dictionary.Add(item.Object, InstansiateViewItem(item));
			}
			UXCollectionViewItem uXCollectionViewItem = dictionary[item.Object];
			dictionary.Remove(item.Object);
			uXCollectionViewItem.Item = item;
			_slots[item.Index % SlotsPerPage].AttachViewItem(uXCollectionViewItem, item.Index / SlotsPerPage);
		}
		foreach (UXCollectionViewItem value in dictionary.Values)
		{
			Object.Destroy((Object)(object)((Component)value).gameObject);
		}
	}

	private void BuildMouseOverIndicator()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("MouseOverIndicator");
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = ((Component)_itemRoot).transform;
		val.transform.localPosition = Vector3.zero;
		val.transform.localScale = Vector3.one;
		MeshRenderer val2 = val.AddComponent<MeshRenderer>();
		((Renderer)val2).material = _mouseOverIndicatorMaterial;
		_mouseOverIndicator = val.AddComponent<UXPlane>();
		_mouseOverIndicator.uses9PatchMaterial = true;
		_mouseOverIndicator.SetSize(_slotWidth, _slotHeight);
		PlaceMouseOverIndicator(0, 0);
	}

	private void MatchSlotsToRequired()
	{
		int slotsPerPage = SlotsPerPage;
		if (slotsPerPage > _slots.Count)
		{
			CreateSlots(slotsPerPage - _slots.Count);
		}
		else if (slotsPerPage < _slots.Count)
		{
			RemoveSlots(_slots.Count - slotsPerPage);
		}
	}

	private void OnCollectionChange()
	{
		if ((Object)(object)_dragItem != (Object)null)
		{
			HandleOnSlotMouseDrop(-1, -1);
		}
		MatchSlotsToRequired();
		AddViewItemsToSlots();
		RefreshSlotVisibility();
		UpdateIndexText();
		if (CurrentPage + 1 > Pages)
		{
			Scroll(0);
		}
	}

	protected void PlaceMouseOverIndicator(int row, int column)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		_mouseOverIndicator.SetVisible(visible: true);
		Vector3 localPosition = (float)column * _columnSpacing * Vector3.right + (float)row * _rowSpacing * Vector3.down + -0.03f * Vector3.forward;
		((Component)_mouseOverIndicator).transform.localPosition = localPosition;
	}

	public override void Update()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if ((Object)(object)_mouseOverIndicator != (Object)null && _mouseOverIndicator.Visible)
		{
			((Component)_mouseOverIndicator).transform.localScale = ((Component)_slots[_currentSlotMouseOver]).transform.localScale;
		}
	}

	private void Scroll(int pages)
	{
		CurrentPage += pages;
		CurrentPage = ClampScrolling(CurrentPage);
		RefreshSlotVisibility();
		UpdateIndexText();
		_mouseOverIndicator.SetVisible(visible: false);
	}

	protected virtual int ClampScrolling(int page)
	{
		if (page < 0)
		{
			page = 0;
		}
		if (page + 1 > Pages)
		{
			page = Pages - 1;
		}
		return page;
	}

	protected virtual void UpdateIndexText()
	{
		if ((Object)(object)_indexText != (Object)null)
		{
			_indexText.Text = $"{CurrentPage + 1}/{Pages}";
		}
	}

	private void HandleOnSlotMouseOverEnter(int slotIndex, int visiblePage)
	{
		if (OnSlotMouseOverEnter != null)
		{
			UXCollectionViewItem viewItem = _slots[slotIndex].GetViewItem(CurrentPage);
			if ((Object)(object)viewItem != (Object)null && !viewItem.Deleted)
			{
				OnSlotMouseOverEnter(viewItem.Item);
			}
			else
			{
				OnSlotMouseOverEnter(null);
			}
		}
	}

	protected virtual void HandleOnSlotMouseOver(int slotIndex, int visiblePage)
	{
		_currentSlotMouseOver = slotIndex;
		int row = slotIndex % _rows;
		int column = slotIndex / _rows;
		PlaceMouseOverIndicator(row, column);
	}

	private void HandleOnSlotMouseOverExit(int slotIndex, int visiblePage)
	{
		if (OnSlotMouseOverExit != null)
		{
			UXCollectionViewItem viewItem = _slots[slotIndex].GetViewItem(CurrentPage);
			if ((Object)(object)viewItem != (Object)null && !viewItem.Deleted)
			{
				OnSlotMouseOverExit(viewItem.Item);
			}
			else
			{
				OnSlotMouseOverExit(null);
			}
		}
	}

	private void HandleOnSlotMouseClick(int slotIndex, int visiblePage)
	{
		UXCollectionViewSlot uXCollectionViewSlot = _slots[slotIndex];
		UXCollectionViewItem viewItem = uXCollectionViewSlot.GetViewItem(CurrentPage);
		if ((Object)(object)viewItem != (Object)null && !viewItem.Deleted)
		{
			uXCollectionViewSlot.Reset();
			if (OnItemSelection != null)
			{
				OnItemSelection(viewItem.Item);
			}
		}
	}

	private void HandleOnSlotMouseDragStart(int slotIndex, Vector3 dragPos)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)_slots[slotIndex].GetViewItem(CurrentPage) == (Object)null))
		{
			_dragFromGlobalSlotIndex = slotIndex + CurrentPage * SlotsPerPage;
			_dragItem = _slots[slotIndex].DetachViewItem(CurrentPage);
			HandleOnSlotMouseDrag(slotIndex, dragPos);
			if (OnItemDragStart != null)
			{
				OnItemDragStart(_dragItem.Item);
			}
		}
	}

	private void HandleOnSlotMouseDrag(int slotIndex, Vector3 dragPos)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_dragItem != (Object)null)
		{
			Vector3 localPosition = ((Component)_itemRoot).transform.InverseTransformPoint(dragPos);
			localPosition.z = -1f;
			((Component)_dragItem).transform.localPosition = localPosition;
		}
	}

	private void HandleOnSlotMouseDrop(int slotIndex, int visiblePage)
	{
		if ((Object)(object)_dragItem == (Object)null)
		{
			return;
		}
		_slots[_dragFromGlobalSlotIndex % SlotsPerPage].AttachViewItem(_dragItem, _dragFromGlobalSlotIndex / SlotsPerPage);
		if (slotIndex != -1)
		{
			if ((Object)(object)_slots[slotIndex].GetViewItem(CurrentPage) != (Object)null)
			{
				if (OnSwapItems != null)
				{
					OnSwapItems(_dragItem.Item, _slots[slotIndex].GetViewItem(CurrentPage).Item);
				}
			}
			else if (OnMoveItem != null)
			{
				OnMoveItem(_dragItem.Item, slotIndex + CurrentPage * SlotsPerPage);
			}
		}
		_dragFromGlobalSlotIndex = -1;
		_dragItem = null;
		if (OnItemDragEnd != null)
		{
			OnItemDragEnd();
		}
	}

	private void HandleOnDragEnterButton(int direction)
	{
		((MonoBehaviour)this).StartCoroutine("DragScrollRoutine", (object)direction);
	}

	private void HandleOnDragExitButton()
	{
		((MonoBehaviour)this).StopCoroutine("DragScrollRoutine");
	}

	private IEnumerator DragScrollRoutine(int direction)
	{
		yield return (object)new WaitForSeconds(1f);
		while (true)
		{
			Scroll(direction);
			yield return (object)new WaitForSeconds(0.8f);
		}
	}

	public override void SetVisible(bool visible)
	{
		Visible = visible;
		foreach (UXCollectionViewSlot slot in _slots)
		{
			slot.SetVisible(visible);
		}
		if (!visible && (Object)(object)_mouseOverIndicator != (Object)null)
		{
			_mouseOverIndicator.SetVisible(visible: false);
		}
	}

	private void RefreshSlotVisibility()
	{
		if (!((Component)this).gameObject.active)
		{
			return;
		}
		foreach (UXCollectionViewSlot slot in _slots)
		{
			slot.SetVisiblePage(CurrentPage);
		}
	}

	public UXCollectionViewItem GetCollectionViewItemFromIndex(int globalSlotIndex)
	{
		int index = globalSlotIndex % SlotsPerPage;
		return _slots[index].GetViewItem(globalSlotIndex / SlotsPerPage);
	}

	public UXCollectionViewItem GetDraggedViewItem()
	{
		return _dragItem;
	}

	public void ResetViewItems()
	{
		if (!_isInitialized)
		{
			return;
		}
		foreach (UXCollectionViewSlot slot in _slots)
		{
			slot.DetachAllViewItems(detachAndDestroy: true);
		}
		RefreshOnNextEnable = true;
	}

	private void OnEnable()
	{
		if (_isInitialized)
		{
			if (RefreshOnNextEnable)
			{
				OnCollectionChange();
			}
			else
			{
				RefreshSlotVisibility();
			}
		}
	}
}
