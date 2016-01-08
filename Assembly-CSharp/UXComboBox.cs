using System;
using System.Collections.Generic;
using UnityEngine;

public class UXComboBox : UXGUIElement, IUXContainer
{
	public delegate void OnComboBoxItemSelectDelegate(int itemIndex);

	public delegate void OnComboBoxActionDelegate();

	public OnComboBoxItemSelectDelegate OnComboBoxItemSelect;

	public OnComboBoxActionDelegate OnComboBoxOpen;

	public OnComboBoxActionDelegate OnComboBoxClose;

	public UXScrollableBox ScrollableBoxPrefab;

	private UXScrollableBox _scrollableBox;

	private UXToggleIconButton _dropdownArrow;

	private Vector3 dropDownArrowOffset = new Vector3(-0.25f, 0.25f, 0f);

	public Material bgMaterial;

	public Vector3 SliderOffset;

	private float scrollableBoxOffset = 2f;

	private float scrollableBoxSliderWidth = 1f;

	private bool _open;

	private GameObject _bg;

	private UXComboBoxItem _currentItem;

	public List<UXComboBoxItem> Items { get; private set; }

	public int CurrentlySelectedItemIndex { get; private set; }

	public UXComboBoxItem CurrentlySelectedItem
	{
		get
		{
			if (CurrentlySelectedItemIndex == -1)
			{
				return null;
			}
			return Items[CurrentlySelectedItemIndex];
		}
	}

	public override void Awake()
	{
		base.Awake();
		Initialize();
	}

	private void Initialize()
	{
		InitializeDropDownArrow();
		InitializeScrollableBox();
		InitializeBackground();
		Items = new List<UXComboBoxItem>();
		CurrentlySelectedItemIndex = -1;
		Close();
	}

	private void InitializeDropDownArrow()
	{
		_dropdownArrow = transform.FindChild("DropDownArrow").GetComponent<UXToggleIconButton>();
		_dropdownArrow.transform.localPosition = new Vector3(Width - scrollableBoxSliderWidth, Height - scrollableBoxOffset, -1f) - Alignment + dropDownArrowOffset;
		UXToggleIconButton dropdownArrow = _dropdownArrow;
		dropdownArrow.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(dropdownArrow.OnToggle, new UXToggleIconButton.OnToggleDelegate(Toggle));
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.size = new Vector3(Width - scrollableBoxSliderWidth, scrollableBoxOffset, 1f);
		boxCollider.center = new Vector3(Width / 2f - scrollableBoxSliderWidth / 2f, Height - scrollableBoxOffset / 2f, 0f) - Alignment;
		UXMouseClickObject uXMouseClickObject = gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(uXMouseClickObject.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject click, Vector3 pos) =>
		{
			Toggle(!_open);
		}));
	}

	private void InitializeScrollableBox()
	{
		_scrollableBox = UnityEngine.Object.Instantiate(ScrollableBoxPrefab);
		_scrollableBox.transform.parent = transform;
		_scrollableBox.transform.localScale = Vector3.one;
		_scrollableBox.transform.localPosition = new Vector3(Width / 2f, Height / 2f - scrollableBoxOffset / 2f, -0.1f) - Alignment;
		_scrollableBox.ScrollBehaviour = ScrollBoxBehaviour.NoScroll;
		_scrollableBox.hideSliderWhenFull = true;
		_scrollableBox.SliderOffset = SliderOffset;
		_scrollableBox.SetSize(Width, Height - scrollableBoxOffset);
	}

	private void InitializeBackground()
	{
		_bg = new GameObject("BG");
		_bg.layer = LayerMask.NameToLayer("UXElement");
		_bg.transform.parent = transform;
		_bg.transform.localScale = Vector3.one;
		_bg.transform.localPosition = new Vector3(Width / 2f, Height - scrollableBoxOffset / 2f) - Alignment;
		_bg.AddComponent<UXPlane>().SetSize(new Vector2(Width, scrollableBoxOffset));
		_bg.AddComponent<MeshRenderer>().material = new Material(bgMaterial);
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		_bg.GetComponent<UXPlane>().SetAlpha(alpha, materialProperty);
		_dropdownArrow.SetAlpha(alpha, materialProperty);
		_scrollableBox.SetAlpha(alpha, materialProperty);
		if (_currentItem != null)
		{
			_currentItem.SetAlpha(alpha, materialProperty);
		}
	}

	public override void SetVisible(bool visible)
	{
		Visible = visible;
		_bg.GetComponent<UXPlane>().SetVisible(visible);
		_dropdownArrow.SetVisible(visible);
		_scrollableBox.SetVisible(visible);
		if (_currentItem != null)
		{
			_currentItem.SetVisible(visible);
		}
	}

	public void Add(string[] items)
	{
		foreach (string text in items)
		{
			Add(text);
		}
	}

	public void Add(string text)
	{
		SimpleTextComboBoxItem component = UnityEngine.Object.Instantiate(PrefabPool.Instance.SimpleTextComboBoxItemObject).GetComponent<SimpleTextComboBoxItem>();
		component.SetSize(new Vector2(Width + SliderOffset.x, 2f));
		component.name = text;
		component.CreateItem(text);
		Add(component);
	}

	public void Add(UXComboBoxItem comboBoxItem)
	{
		Items.Add(comboBoxItem);
		_scrollableBox.AddLine(comboBoxItem);
		comboBoxItem.OnItemClick = (UXComboBoxItem.OnItemClickDelegate)Delegate.Combine(comboBoxItem.OnItemClick, new UXComboBoxItem.OnItemClickDelegate(OnItemClick));
		Close();
	}

	public void RemoveItem(int index)
	{
		UXComboBoxItem comboBoxItem = Items[index];
		RemoveItem(comboBoxItem);
	}

	public void RemoveItem(UXComboBoxItem comboBoxItem)
	{
		comboBoxItem.OnItemClick = (UXComboBoxItem.OnItemClickDelegate)Delegate.Remove(comboBoxItem.OnItemClick, new UXComboBoxItem.OnItemClickDelegate(OnItemClick));
		_scrollableBox.RemoveLine(comboBoxItem);
		Items.Remove(comboBoxItem);
		Close();
	}

	public void RemoveAllItems()
	{
		foreach (UXComboBoxItem item in Items)
		{
			item.OnItemClick = (UXComboBoxItem.OnItemClickDelegate)Delegate.Remove(item.OnItemClick, new UXComboBoxItem.OnItemClickDelegate(OnItemClick));
		}
		_scrollableBox.RemoveAllLines();
		Items.Clear();
		Close();
	}

	private void Toggle(bool toggle)
	{
		if (toggle)
		{
			Open();
		}
		else
		{
			Close();
		}
	}

	public void Open()
	{
		_open = true;
		_dropdownArrow.SetToggleState(_open);
		_scrollableBox.SetVisible(visible: true);
		if (OnComboBoxOpen != null)
		{
			OnComboBoxOpen();
		}
	}

	public void Close()
	{
		_open = false;
		_dropdownArrow.SetToggleState(_open);
		_scrollableBox.SetVisible(visible: false);
		if (OnComboBoxClose != null)
		{
			OnComboBoxClose();
		}
	}

	private void OnItemClick(UXComboBoxItem comboBoxItem)
	{
		SetCurrentItem(Items.IndexOf(comboBoxItem));
		Close();
	}

	public void SetCurrentItem(int itemIndex)
	{
		CurrentlySelectedItemIndex = itemIndex;
		if (_currentItem != null)
		{
			UnityEngine.Object.Destroy(_currentItem.gameObject);
		}
		_currentItem = Items[itemIndex].CreateClone().GetComponent<UXComboBoxItem>();
		_currentItem.name = "SelectedItem";
		_currentItem.transform.parent = transform;
		_currentItem.transform.localPosition = new Vector3(_currentItem.Width / 2f, Height - _currentItem.Height / 2f, -0.1f) - Alignment;
		_currentItem.transform.localScale = Vector3.one;
		_currentItem.transform.localRotation = Quaternion.identity;
		if (OnComboBoxItemSelect != null)
		{
			OnComboBoxItemSelect(CurrentlySelectedItemIndex);
		}
	}
}
