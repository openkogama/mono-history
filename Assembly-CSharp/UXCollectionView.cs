using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UX/Collections/Collection View")]
public class UXCollectionView : UXViewScript
{
	public delegate void OnMoveItemDelegate(int sourceSlotIndex, int destinationSlotIndex);

	public delegate void OnItemSelectionDelegate(IUXCollectionItem item);

	public delegate void OnRemoveItemDelegate(IUXCollectionItem item);

	public delegate void OnNextPageDelegate();

	public delegate void OnPreviousPageDelegate();

	private Vector3 ROW_AXIS = Vector3.down;

	private Vector3 COLUMN_AXIS = Vector3.right;

	private static float SPACING = 8f;

	private static float X_PADDING = SPACING;

	public UXButton previousButton;

	public UXButton nextButton;

	public UXText pageNumberText;

	public GameObject slotPrefab;

	private int currentPageIndex;

	private int pageIndexTarget;

	private float offset;

	private float offsetTarget;

	private float offsetVelocity;

	private int rowCount = 2;

	private int columnCount = 6;

	private float itemSpaceWidth;

	private float itemSpaceHeight;

	private Dictionary<object, IUXCollectionItem> object2items;

	private UXCollectionViewItem[] viewItems = new UXCollectionViewItem[0];

	private UXCollectionViewSlot[] slots = new UXCollectionViewSlot[0];

	private IUXCollection collection;

	private Dictionary<IUXCollectionItem, UXCollectionViewItem> collectionViewItemCache = new Dictionary<IUXCollectionItem, UXCollectionViewItem>();

	private Func<IUXCollectionItem, UXCollectionViewItem> instansiateViewItem;

	private HashSet<UXCollectionViewItem> activeViewItems = new HashSet<UXCollectionViewItem>();

	public Transform root;

	private GameObject itemsOrigo;

	private GameObject itemsOffset;

	private GameObject mouseOverCursor;

	public OnMoveItemDelegate OnMoveItem;

	public OnItemSelectionDelegate OnItemSelection;

	public OnRemoveItemDelegate OnRemoveItem;

	public OnNextPageDelegate OnNextPage;

	public OnPreviousPageDelegate OnPreviousPage;

	private float SLIDE_SMOOTH_TIME = 0.1f;

	private float SLIDE_MAX_SPEED = 60f;

	public LinearCurve visibilityCurve;

	public int RowCount
	{
		get
		{
			return rowCount;
		}
		set
		{
			rowCount = value;
		}
	}

	public int ColumnCount
	{
		get
		{
			return columnCount;
		}
		set
		{
			columnCount = value;
		}
	}

	private int NumberOfPages => (FindMaxIndex() + 1) / PageSize + 1;

	private int PageSize => rowCount * columnCount;

	public Func<IUXCollectionItem, UXCollectionViewItem> InstansiateViewItem
	{
		get
		{
			return instansiateViewItem;
		}
		set
		{
			instansiateViewItem = value;
		}
	}

	public IUXCollection Collection
	{
		get
		{
			return collection;
		}
		set
		{
			IUXCollection iUXCollection = collection;
			if (iUXCollection != value)
			{
				if (iUXCollection != null)
				{
					iUXCollection.OnCollectionChange = (OnCollectionChangeDelegate)Delegate.Remove(iUXCollection.OnCollectionChange, new OnCollectionChangeDelegate(OnCollectionChange));
				}
				collection = value;
				if (collection != null)
				{
					IUXCollection iUXCollection2 = collection;
					iUXCollection2.OnCollectionChange = (OnCollectionChangeDelegate)Delegate.Combine(iUXCollection2.OnCollectionChange, new OnCollectionChangeDelegate(OnCollectionChange));
				}
			}
			OnCollectionChange();
		}
	}

	public UXCollectionView()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void OnInitialize()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected Obj, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected Obj, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		itemSpaceWidth = (float)columnCount * SPACING;
		itemSpaceHeight = (float)rowCount * SPACING;
		itemsOffset = new GameObject("Offset");
		itemsOffset.transform.parent = root;
		itemsOffset.transform.localScale = Vector3.one;
		itemsOffset.transform.localPosition = new Vector3(SPACING * 0.5f, (0f - SPACING) * 0.5f, -1f);
		itemsOrigo = new GameObject("Origo");
		itemsOrigo.transform.parent = itemsOffset.transform;
		itemsOrigo.transform.localPosition = Vector3.zero;
		itemsOrigo.transform.localScale = Vector3.one;
		mouseOverCursor = NewMouseOverCursor();
		UXButton uXButton = nextButton;
		uXButton.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton.OnClick, new UXButton.OnClickDelegate(HandleOnClickNext));
		UXButton uXButton2 = previousButton;
		uXButton2.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton2.OnClick, new UXButton.OnClickDelegate(HandleOnClickPrevious));
		visibilityCurve = new LinearCurve(new LinearCurveKey[4]
		{
			new LinearCurveKey(SPACING * 0.25f, 0f),
			new LinearCurveKey(SPACING * 0.5f, 1f),
			new LinearCurveKey(SPACING * 0.5f + (float)(columnCount - 1) * SPACING, 1f),
			new LinearCurveKey(SPACING * 0.5f + (float)(columnCount - 1) * SPACING + 0.25f * SPACING, 0f)
		});
		RefreshUI();
		MoveCursorToSlot(-1);
	}

	public override void OnShow()
	{
		base.OnShow();
		RefreshUI();
	}

	private void RefreshVisibility()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		UXVisibility[] componentsInChildren = ((Component)this).GetComponentsInChildren<UXVisibility>();
		foreach (UXVisibility uXVisibility in componentsInChildren)
		{
			if ((Object)(object)uXVisibility != (Object)null)
			{
				Matrix4x4 val = root.worldToLocalMatrix * ((Component)uXVisibility).transform.localToWorldMatrix;
				Vector3 val2 = val.MultiplyPoint(Vector3.zero);
				uXVisibility.Visibility = ((!View.isVisible) ? 0f : visibilityCurve.Evaluate(val2.x));
			}
		}
	}

	private void GoToPage(int pageIndex)
	{
		((MonoBehaviour)this).StopAllCoroutines();
		((MonoBehaviour)this).StartCoroutine(Slide(pageIndex));
	}

	private IEnumerator Slide(int pageIndex)
	{
		pageIndexTarget = pageIndex;
		offsetTarget = (float)(-pageIndex) * itemSpaceWidth;
		while (Mathf.Abs(offsetTarget - offset) > 0.01f)
		{
			offset = Mathf.SmoothDamp(offset, offsetTarget, ref offsetVelocity, SLIDE_SMOOTH_TIME, SLIDE_MAX_SPEED);
			itemsOrigo.transform.localPosition = new Vector3(offset, 0f);
			RefreshVisibility();
			yield return (object)new WaitForSeconds(0f);
		}
		currentPageIndex = pageIndexTarget;
		RefreshPageNumberText();
	}

	private void RefreshPageNumberText()
	{
		pageNumberText.Text = $"{currentPageIndex + 1}/{NumberOfPages}";
	}

	private GameObject NewMouseOverCursor()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected Obj, but got Unknown
		GameObject val = new GameObject("MouseOverCursor");
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = itemsOrigo.transform;
		((Component)val.transform).transform.localScale = Vector3.one * 6f;
		MeshFilter val2 = val.AddComponent<MeshFilter>();
		val2.mesh = UXUtils.BuildPlaneMesh();
		MeshRenderer val3 = val.AddComponent<MeshRenderer>();
		Object val4 = Resources.Load("Materials/CollectionViewMouseOver");
		((Renderer)val3).material = new Material((Material)(object)((val4 is Material) ? val4 : null));
		val.AddComponent<UXVisibilityMeshRenderers>();
		return val;
	}

	private void OnCollectionChange()
	{
		UpdateViewItems();
	}

	private void RefreshSlots()
	{
		DestroySlotObjects();
		CreateSlotObjects();
	}

	private void DestroySlotObjects()
	{
		UXCollectionViewSlot[] array = slots;
		foreach (UXCollectionViewSlot uXCollectionViewSlot in array)
		{
			Object.Destroy((Object)(object)((Component)uXCollectionViewSlot).gameObject);
		}
	}

	private void CreateSlotObjects()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		slots = new UXCollectionViewSlot[PageSize * NumberOfPages];
		for (int i = 0; i < slots.Length; i++)
		{
			Object val = Object.Instantiate((Object)(object)slotPrefab);
			GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
			((Object)val2).name = $"{((Object)this).name} - Slot {i}";
			val2.transform.parent = itemsOrigo.transform;
			val2.transform.localPosition = ViewItemPosition(i);
			val2.transform.localScale = Vector3.one * 5f;
			UXCollectionViewSlot component = val2.GetComponent<UXCollectionViewSlot>();
			component.SlotIndex = i;
			component.OnSlotMouseDown = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Combine(component.OnSlotMouseDown, new UXCollectionViewSlot.SlotEventDelegate(HandleOnSlotMouseClick));
			component.OnSlotMouseOver = (UXCollectionViewSlot.SlotEventDelegate)Delegate.Combine(component.OnSlotMouseOver, new UXCollectionViewSlot.SlotEventDelegate(HandleOnSlotMouseOver));
			slots[i] = component;
		}
	}

	private void RefreshUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Initialize();
		RefreshSlots();
		for (int i = 0; i < viewItems.Length; i++)
		{
			UXCollectionViewItem uXCollectionViewItem = viewItems[i];
			if ((Object)(object)uXCollectionViewItem != (Object)null)
			{
				((Component)uXCollectionViewItem).transform.localPosition = ViewItemPosition(i) - 2f * Vector3.forward;
			}
		}
		RefreshVisibility();
		pageNumberText.Initialize();
		RefreshPageNumberText();
	}

	private Vector3 ViewItemPosition(int index)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return ViewItemPosition(index / rowCount, index % rowCount);
	}

	private Vector3 ViewItemPosition(int c, int r)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return ((float)c * COLUMN_AXIS + (float)r * ROW_AXIS) * SPACING;
	}

	private void UpdateViewItems()
	{
		HashSet<UXCollectionViewItem> hashSet = new HashSet<UXCollectionViewItem>(activeViewItems);
		viewItems = new UXCollectionViewItem[NumberOfPages * PageSize];
		if (collection != null)
		{
			for (int i = 0; i < collection.Count; i++)
			{
				IUXCollectionItem item = collection.GetItem(i);
				UXCollectionViewItem viewItem = GetViewItem(item);
				if (!activeViewItems.Contains(viewItem))
				{
					InitializeViewItem(viewItem);
					activeViewItems.Add(viewItem);
				}
				hashSet.Remove(viewItem);
				viewItems[item.Index] = viewItem;
			}
		}
		foreach (UXCollectionViewItem item2 in hashSet)
		{
			DestroyViewItem(item2);
			activeViewItems.Remove(item2);
		}
		RefreshUI();
	}

	private void InitializeViewItem(UXCollectionViewItem viewItem)
	{
	}

	private void DestroyViewItem(UXCollectionViewItem viewItem)
	{
		collectionViewItemCache.Remove(viewItem.Item);
		Object.Destroy((Object)(object)((Component)viewItem).gameObject);
	}

	private void HandleOnSlotMouseOver(int slotIndex)
	{
		MoveCursorToSlot(slotIndex);
		RefreshVisibility();
	}

	private void HandleOnSlotMouseClick(int slotIndex)
	{
		UXCollectionViewItem uXCollectionViewItem = viewItems[slotIndex];
		if ((Object)(object)uXCollectionViewItem != (Object)null)
		{
			NotifyItemSelection(uXCollectionViewItem.Item);
		}
	}

	private void HandleOnRemove(UXCollectionViewItem viewItem)
	{
		NotifyRemoveItem(viewItem.Item);
	}

	private void HandleOnClickNext()
	{
		if (pageIndexTarget < NumberOfPages - 1)
		{
			pageIndexTarget++;
		}
		GoToPage(pageIndexTarget);
		NotifyNextPage();
	}

	private void HandleOnClickPrevious()
	{
		if (pageIndexTarget > 0)
		{
			pageIndexTarget--;
		}
		GoToPage(pageIndexTarget);
		NotifyPreviousPage();
	}

	private void NotifyRemoveItem(IUXCollectionItem item)
	{
		if (OnRemoveItem != null)
		{
			OnRemoveItem(item);
		}
	}

	private void NotifyItemSelection(IUXCollectionItem item)
	{
		if (OnItemSelection != null)
		{
			OnItemSelection(item);
		}
	}

	private void NotifyNextPage()
	{
		if (OnNextPage != null)
		{
			OnNextPage();
		}
	}

	private void NotifyPreviousPage()
	{
		if (OnPreviousPage != null)
		{
			OnPreviousPage();
		}
	}

	private void MoveCursorToSlot(int index)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		mouseOverCursor.renderer.enabled = index > -1;
		mouseOverCursor.transform.localPosition = ViewItemPosition(index);
	}

	private UXCollectionViewItem GetViewItem(IUXCollectionItem collectionItem)
	{
		UXCollectionViewItem value = null;
		if (!collectionViewItemCache.TryGetValue(collectionItem, out value))
		{
			value = InstansiateViewItem(collectionItem);
			((Component)value).transform.parent = itemsOrigo.transform;
			UXCollectionViewItem uXCollectionViewItem = value;
			uXCollectionViewItem.OnRemove = (OnCollectionViewItemDelegate)Delegate.Combine(uXCollectionViewItem.OnRemove, new OnCollectionViewItemDelegate(HandleOnRemove));
		}
		return value;
	}

	private int FindMaxIndex()
	{
		int num = -1;
		if (collection != null)
		{
			for (int i = 0; i < collection.Count; i++)
			{
				IUXCollectionItem item = collection.GetItem(i);
				if (item.Index > num)
				{
					num = item.Index;
				}
			}
		}
		return num;
	}
}
