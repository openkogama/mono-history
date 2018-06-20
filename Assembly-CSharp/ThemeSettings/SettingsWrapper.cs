using System.Collections.Generic;
using ThemeAttributes;
using UnityEngine;

namespace ThemeSettings;

public abstract class SettingsWrapper
{
	protected Dictionary<object, object> settingsData;

	private List<ThemeAttribute> attributes = new List<ThemeAttribute>(8);

	private int activeAttributeGroup;

	private IMenu menu;

	public Dictionary<object, object> Data => settingsData;

	public int ActiveAttributeGroup => activeAttributeGroup;

	public RectTransform[] SettingsUI
	{
		get
		{
			List<ThemeAttribute> list = attributes.FindAll((ThemeAttribute attrib) => (attrib.Groups & activeAttributeGroup) == attrib.Groups);
			RectTransform[] array = new RectTransform[list.Count];
			for (int num = 0; num < list.Count; num++)
			{
				ThemeAttribute themeAttribute = list[num];
				array[num] = themeAttribute.GetSettingsUIObject();
			}
			return array;
		}
	}

	public abstract void CommitChanges();

	public void Initialize()
	{
		foreach (ThemeAttribute attribute in attributes)
		{
			if (attribute.Groups == 0)
			{
				attribute.ApplyValue();
			}
		}
	}

	public void Add(ThemeAttribute attrib)
	{
		attributes.Add(attrib);
	}

	public T GetValueForAttribute<T>(ThemeAttribute attrib)
	{
		if (settingsData.ContainsKey(attrib.Key))
		{
			return (T)settingsData[attrib.Key];
		}
		return (T)attrib.Data;
	}

	public void UpdateData(string key, object val)
	{
		settingsData[key] = val;
	}

	public void DisableAttributeGroups(int groupsFlag)
	{
		SetAttributeGroups(activeAttributeGroup & ~groupsFlag);
	}

	public void EnableAttributeGroups(int groupsFlag)
	{
		SetAttributeGroups(activeAttributeGroup | groupsFlag);
	}

	public void SetAttributeGroups(int groupsFlag)
	{
		List<ThemeAttribute> list = new List<ThemeAttribute>(2);
		foreach (ThemeAttribute attribute in attributes)
		{
			bool flag = (attribute.Groups & activeAttributeGroup) == attribute.Groups;
			bool flag2 = (attribute.Groups & groupsFlag) == attribute.Groups;
			if (!flag && flag2)
			{
				list.Add(attribute);
			}
		}
		activeAttributeGroup = groupsFlag;
		foreach (ThemeAttribute item in list)
		{
			item.ApplyValue();
		}
		if (menu != null)
		{
			menu.Refresh();
		}
	}

	public void SubscribeToSettingsUI(IMenu menu)
	{
		this.menu = menu;
		menu.Refresh();
	}

	public void UnsubscribeToSettingsUI()
	{
		menu = null;
	}
}
