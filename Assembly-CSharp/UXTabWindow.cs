using System;
using System.Collections.Generic;
using UnityEngine;

public class UXTabWindow : UXWindow
{
	public delegate void OnTabSelectedDelegate(int tabId);

	public OnTabSelectedDelegate OnTabSelect;

	[SerializeField]
	private List<UXTabPane> _tabs;

	[SerializeField]
	private float _tabPaneHeight = 2f;

	[SerializeField]
	private bool _adjustWidthToTabs = true;

	[SerializeField]
	private float _tabPaneSpacing = 0.3f;

	public List<UXTabPane> TabPanes => _tabs;

	public override void Awake()
	{
		base.Awake();
		float num = 0f;
		for (int i = 0; i < _tabs.Count; i++)
		{
			UXTabPane uXTabPane = _tabs[i];
			uXTabPane.BuildTab(_tabPaneHeight, i);
			uXTabPane.OnTabClick = (UXTabPane.OnTabClickDelegate)Delegate.Combine(uXTabPane.OnTabClick, new UXTabPane.OnTabClickDelegate(OnTabClick));
			num += uXTabPane.Width;
			uXTabPane.Hide();
		}
		num += _tabPaneSpacing * (float)(_tabs.Count - 1);
		if (_adjustWidthToTabs)
		{
			Width = num;
		}
		SetSize(Width, Height);
		AlignTabs();
		transform.localPosition += new Vector3(0f, (0f - _tabPaneHeight) / 2f, 0f);
		SelectTab(0);
	}

	private void AlignTabs()
	{
		float num = 0f;
		for (int i = 0; i < _tabs.Count; i++)
		{
			UXTabPane uXTabPane = _tabs[i];
			uXTabPane.transform.localPosition = new Vector3(num + uXTabPane.Width / 2f, Height - 0.5f, 0.1f) - Alignment;
			num += uXTabPane.Width + _tabPaneSpacing;
		}
	}

	public UXTabPane GetTab(int tabId)
	{
		if (tabId > _tabs.Count - 1)
		{
			return null;
		}
		return _tabs[tabId];
	}

	public void SelectTab(int tabId)
	{
		UXTabPane tab = GetTab(tabId);
		if (tab != null)
		{
			tab.FireOnClick();
		}
	}

	public void OnTabClick(int tabId)
	{
		foreach (UXTabPane tab2 in _tabs)
		{
			tab2.Hide();
		}
		UXTabPane tab = GetTab(tabId);
		if (tab != null)
		{
			tab.Show();
		}
		if (OnTabSelect != null)
		{
			OnTabSelect(tabId);
		}
	}
}
