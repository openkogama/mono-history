using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SoundInventoryController : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	private InventoryController inventoryController;

	private readonly Dictionary<int, TabState> tabs = new Dictionary<int, TabState>();

	private int selectedTab;

	private readonly Dictionary<int, List<SoundTabInfo>> soundTabInfos = new Dictionary<int, List<SoundTabInfo>>();

	[SerializeField]
	private InventoryController inventoryControllerPrefab;

	[SerializeField]
	private int numberOfSlotsPrPage;

	[SerializeField]
	private SoundViewItem soundViewItemPrefab;

	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private StreamedAudioClipList audioUrls;

	private List<StreamedAudioClipInfo> urls;

	private readonly Dictionary<int, int> categorysAmount = new Dictionary<int, int>();

	private string originalURL;

	private readonly Dictionary<string, int> categoryToNameCombinations = new Dictionary<string, int>();

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.SoundEmitter);
		selectedTab = 1;
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		if (!data.ContainsKey("url"))
		{
			data.Add("url", string.Empty);
		}
		Debug.Log("Data sound: url: " + data["url"].ToString() + " Volume: " + data["volume"].ToString() + " Pitch: " + data["pitch"].ToString());
		originalURL = (string)data["url"];
		urls = audioUrls.URLS;
		this.inventoryController = UnityEngine.Object.Instantiate(inventoryControllerPrefab);
		InventoryController inventoryController = this.inventoryController;
		inventoryController.OnPageTurned = (UnityAction<int>)Delegate.Combine(inventoryController.OnPageTurned, new UnityAction<int>(PageTurned));
		InventoryController inventoryController2 = this.inventoryController;
		inventoryController2.OnTabSelected = (UnityAction<int>)Delegate.Combine(inventoryController2.OnTabSelected, new UnityAction<int>(TabSelected));
		this.inventoryController.Initialize(numberOfSlotsPrPage);
		this.inventoryController.transform.SetParent(transform, worldPositionStays: false);
		for (int i = 0; i < urls.Count; i++)
		{
			string category = urls[i].category;
			string soundName = urls[i].name;
			if (!categoryToNameCombinations.ContainsKey(category))
			{
				categoryToNameCombinations.Add(category, categoryToNameCombinations.Count + 1);
			}
			int num = categoryToNameCombinations[category];
			if (!tabs.ContainsKey(categoryToNameCombinations[category]))
			{
				TabState tabState = new TabState(category, numberOfSlotsPrPage);
				tabs.Add(num, tabState);
				this.inventoryController.AddTab(num, tabState.name);
			}
			if (!soundTabInfos.ContainsKey(num))
			{
				soundTabInfos.Add(num, new List<SoundTabInfo>());
			}
			if (soundTabInfos[num].All((SoundTabInfo soundTabInfo) => soundTabInfo.name != soundName))
			{
				soundTabInfos[num].Add(new SoundTabInfo
				{
					name = soundName,
					categoryName = category,
					url = urls[i].url
				});
			}
			if (!categorysAmount.ContainsKey(num))
			{
				categorysAmount.Add(num, 1);
			}
			else
			{
				Dictionary<int, int> dictionary2;
				Dictionary<int, int> dictionary = (dictionary2 = categorysAmount);
				int key2;
				int key = (key2 = num);
				key2 = dictionary2[key2];
				dictionary[key] = key2 + 1;
			}
			tabs[categoryToNameCombinations[category]].highestSlotIndex++;
		}
		UpdateContent();
	}

	public void UpdateContent()
	{
		inventoryController.Clear();
		inventoryController.SelectTab(selectedTab, tabs[selectedTab].currentPage, tabs[selectedTab].MaxPages);
		OnSettingChanged("url", originalURL);
		List<SoundTabInfo> list = soundTabInfos[selectedTab];
		int num = categorysAmount[selectedTab];
		for (int i = 0; i < num; i++)
		{
			if (tabs[selectedTab].SlotIndexIsInRange(i))
			{
				SoundViewItem soundViewItem = UnityEngine.Object.Instantiate(soundViewItemPrefab);
				soundViewItem.Initialize(list[i], originalURL, SetNewOriginalUrl);
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
}
