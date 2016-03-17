using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SoundInventoryController : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	private InventoryController inventoryController;

	private readonly Dictionary<int, List<SoundBite>> categoryToAssets = new Dictionary<int, List<SoundBite>>();

	private readonly Dictionary<int, TabState> tabs = new Dictionary<int, TabState>();

	private int selectedTab;

	[SerializeField]
	private InventoryController inventoryControllerPrefab;

	[SerializeField]
	private int numberOfSlotsPrPage;

	[SerializeField]
	private SoundViewItem soundViewItemPrefab;

	[SerializeField]
	private SettingsBase settingsBase;

	private string originalURL;

	private readonly Dictionary<string, int> categoryToNameCombinations = new Dictionary<string, int>
	{
		{ "Nature", 1 },
		{ "Machinery", 2 },
		{ "Urban", 3 },
		{ "MusicLoop", 4 },
		{ "Alarms", 5 },
		{ "Horror", 6 },
		{ "Delight", 7 },
		{ "Suspense", 6 },
		{ "Robotic", 2 }
	};

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root);
		selectedTab = 1;
		foreach (ProductInventoryInfo item in MVGameControllerBase.Game.StreamingAssetInventory.Get(StreamingAssetType.AmbientAudio))
		{
			int key = categoryToNameCombinations[item.ProductInfo.CategoryName];
			if (!tabs.ContainsKey(key))
			{
				TabState value = new TabState(TM._(item.ProductInfo.CategoryName), numberOfSlotsPrPage);
				tabs.Add(item.ProductInfo.CategoryID, value);
			}
			if (!categoryToAssets.ContainsKey(key))
			{
				categoryToAssets.Add(key, new List<SoundBite>());
			}
			categoryToAssets[key].Add(new SoundBite
			{
				assetInfo = item.ProductInfo,
				unlocked = true
			});
			tabs[key].highestSlotIndex++;
		}
		StreamingAssetInfo assetInfo;
		foreach (StreamingAssetInfo item2 in MVGameControllerBase.Game.StreamingAssetShopInventory.Get(StreamingAssetType.AmbientAudio))
		{
			assetInfo = item2;
			int key2 = categoryToNameCombinations[assetInfo.CategoryName];
			if (!tabs.ContainsKey(categoryToNameCombinations[assetInfo.CategoryName]))
			{
				TabState value2 = new TabState(TM._(assetInfo.CategoryName), numberOfSlotsPrPage);
				tabs.Add(assetInfo.CategoryID, value2);
			}
			if (!categoryToAssets.ContainsKey(key2))
			{
				categoryToAssets.Add(key2, new List<SoundBite>());
			}
			if (categoryToAssets[key2].All((SoundBite sb) => sb.assetInfo.ProductID != assetInfo.ProductID))
			{
				categoryToAssets[key2].Add(new SoundBite
				{
					assetInfo = assetInfo,
					unlocked = false
				});
			}
			tabs[categoryToNameCombinations[assetInfo.CategoryName]].highestSlotIndex++;
		}
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("url", "AmbientAudio/Nature/kgm_amb_forest.unity3d");
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		if (!dictionary2.ContainsKey("url"))
		{
			originalURL = "AmbientAudio / Nature / kgm_amb_forest.unity3d";
		}
		originalURL = (string)dictionary2["url"];
		this.inventoryController = UnityEngine.Object.Instantiate(inventoryControllerPrefab);
		InventoryController inventoryController = this.inventoryController;
		inventoryController.OnPageTurned = (UnityAction<int>)Delegate.Combine(inventoryController.OnPageTurned, new UnityAction<int>(PageTurned));
		InventoryController inventoryController2 = this.inventoryController;
		inventoryController2.OnTabSelected = (UnityAction<int>)Delegate.Combine(inventoryController2.OnTabSelected, new UnityAction<int>(TabSelected));
		this.inventoryController.Initialize(numberOfSlotsPrPage);
		foreach (KeyValuePair<int, TabState> tab in tabs)
		{
			this.inventoryController.AddTab(tab.Key, tab.Value.name);
		}
		this.inventoryController.transform.SetParent(transform, worldPositionStays: false);
		UpdateContent();
	}

	public void UpdateContent()
	{
		inventoryController.Clear();
		inventoryController.SelectTab(selectedTab, tabs[selectedTab].currentPage, tabs[selectedTab].MaxPages);
		OnSettingChanged("url", originalURL);
		List<SoundBite> list = categoryToAssets[selectedTab];
		for (int i = 0; i < list.Count; i++)
		{
			if (tabs[selectedTab].SlotIndexIsInRange(i))
			{
				SoundViewItem soundViewItem = UnityEngine.Object.Instantiate(soundViewItemPrefab);
				soundViewItem.Initialize(list[i].assetInfo, originalURL, SetNewOriginalUrl);
				inventoryController.AddObject(soundViewItem.gameObject, i % numberOfSlotsPrPage);
			}
		}
	}

	private void SetNewOriginalUrl(string url)
	{
		originalURL = url;
		UpdateContent();
	}

	private void PageTurned(int dir)
	{
		if (tabs[selectedTab].UpdatePage(dir))
		{
			UpdateContent();
		}
	}

	private void TabSelected(int tab)
	{
		if (selectedTab != tab)
		{
			selectedTab = tab;
			UpdateContent();
		}
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToString(value));
	}

	private void OnDestroy()
	{
		OnSettingChanged("url", originalURL);
	}
}
