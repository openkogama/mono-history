using System.Collections.Generic;
using UnityEngine;

public class ItemCategories
{
	private Dictionary<string, int> itemCategoriesNameID;

	private Dictionary<int, string> itemCategoriesIDName = new Dictionary<int, string>();

	public ItemCategories(Dictionary<string, int> itemCategories)
	{
		itemCategoriesNameID = itemCategories;
		foreach (KeyValuePair<string, int> item in itemCategoriesNameID)
		{
			itemCategoriesIDName.Add(item.Value, item.Key);
		}
	}

	public int NameToID(string name)
	{
		if (!itemCategoriesNameID.ContainsKey(name))
		{
			Debug.LogError("Could not find category id");
			return -1;
		}
		return itemCategoriesNameID[name];
	}

	public string IDToName(int id)
	{
		if (!itemCategoriesIDName.ContainsKey(id))
		{
			Debug.LogError("Could not find category name");
			return string.Empty;
		}
		return itemCategoriesIDName[id];
	}

	public string[] GetNames()
	{
		List<string> list = new List<string>();
		foreach (string key in itemCategoriesNameID.Keys)
		{
			list.Add(key);
		}
		return list.ToArray();
	}
}
