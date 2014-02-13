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

	public UXComboBox()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		_dropdownArrow = ((Component)((Component)this).transform.FindChild("DropDownArrow")).GetComponent<UXToggleIconButton>();
		((Component)_dropdownArrow).transform.localPosition = new Vector3(Width - scrollableBoxSliderWidth, Height - scrollableBoxOffset, -1f) - Alignment + dropDownArrowOffset;
		UXToggleIconButton dropdownArrow = _dropdownArrow;
		dropdownArrow.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(dropdownArrow.OnToggle, new UXToggleIconButton.OnToggleDelegate(Toggle));
		BoxCollider val = ((Component)this).gameObject.AddComponent<BoxCollider>();
		val.size = new Vector3(Width - scrollableBoxSliderWidth, scrollableBoxOffset, 1f);
		val.center = new Vector3(Width / 2f - scrollableBoxSliderWidth / 2f, Height - scrollableBoxOffset / 2f, 0f) - Alignment;
		UXMouseClickObject uXMouseClickObject = ((Component)this).gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(uXMouseClickObject.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject click, Vector3 pos) =>
		{
			Toggle(!_open);
		}));
	}

	private void InitializeScrollableBox()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		_scrollableBox = Object.Instantiate((Object)(object)ScrollableBoxPrefab) as UXScrollableBox;
		((Component)_scrollableBox).transform.parent = ((Component)this).transform;
		((Component)_scrollableBox).transform.localScale = Vector3.one;
		((Component)_scrollableBox).transform.localPosition = new Vector3(Width / 2f, Height / 2f - scrollableBoxOffset / 2f, -0.1f) - Alignment;
		_scrollableBox.ScrollBehaviour = ScrollBoxBehaviour.NoScroll;
		_scrollableBox.hideSliderWhenFull = true;
		_scrollableBox.SliderOffset = SliderOffset;
		_scrollableBox.SetSize(Width, Height - scrollableBoxOffset);
	}

	private void InitializeBackground()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected Obj, but got Unknown
		_bg = new GameObject("BG");
		_bg.layer = LayerMask.NameToLayer("UXElement");
		_bg.transform.parent = ((Component)this).transform;
		_bg.transform.localScale = Vector3.one;
		_bg.transform.localPosition = new Vector3(Width / 2f, Height - scrollableBoxOffset / 2f) - Alignment;
		_bg.AddComponent<UXPlane>().SetSize(new Vector2(Width, scrollableBoxOffset));
		((Renderer)_bg.AddComponent<MeshRenderer>()).material = new Material(bgMaterial);
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		_bg.GetComponent<UXPlane>().SetAlpha(alpha, materialProperty);
		_dropdownArrow.SetAlpha(alpha, materialProperty);
		_scrollableBox.SetAlpha(alpha, materialProperty);
		if ((Object)(object)_currentItem != (Object)null)
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
		if ((Object)(object)_currentItem != (Object)null)
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ComboBox/SimpleTextComboBoxItem"));
		SimpleTextComboBoxItem component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<SimpleTextComboBoxItem>();
		component.SetSize(new Vector2(Width + SliderOffset.x, 2f));
		((Object)component).name = text;
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
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		CurrentlySelectedItemIndex = itemIndex;
		if ((Object)(object)_currentItem != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)_currentItem).gameObject);
		}
		_currentItem = Items[itemIndex].CreateClone().GetComponent<UXComboBoxItem>();
		((Object)_currentItem).name = "SelectedItem";
		((Component)_currentItem).transform.parent = ((Component)this).transform;
		((Component)_currentItem).transform.localPosition = new Vector3(_currentItem.Width / 2f, Height - _currentItem.Height / 2f, -0.1f) - Alignment;
		((Component)_currentItem).transform.localScale = Vector3.one;
		((Component)_currentItem).transform.localRotation = Quaternion.identity;
		if (OnComboBoxItemSelect != null)
		{
			OnComboBoxItemSelect(CurrentlySelectedItemIndex);
		}
	}
}
