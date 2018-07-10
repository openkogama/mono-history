using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.HighlightSystem;
using MV.WorldObject.HighlightSystem.HighlightPayloads;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabMenuButtonAccessory : TabMenuButtonBase, IHighlightedElement
{
	[Serializable]
	private struct AccessoryTabDef
	{
		public AccessoryCategoryClient tabID;

		public Image streamedImagePrefab;
	}

	[SerializeField]
	private Button button;

	[SerializeField]
	private LayoutElement layoutElement;

	[SerializeField]
	private List<AccessoryTabDef> tabDefs;

	[SerializeField]
	private float selectedTabHeight = 140f;

	[SerializeField]
	private float lerpTime = 0.5f;

	[SerializeField]
	private GameObject redDot;

	[SerializeField]
	private Text redDotCount;

	private float startTime;

	private Image icon;

	private float defaultHeight = 120f;

	private AccessoryCategoryClient category;

	public override void Initialize(int tabId, string categoryName)
	{
		category = (AccessoryCategoryClient)tabId;
		defaultHeight = layoutElement.minHeight;
		UpdateHighlightState();
		bool flag = false;
		for (int i = 0; i < tabDefs.Count; i++)
		{
			if (tabId == (int)tabDefs[i].tabID)
			{
				icon = UnityEngine.Object.Instantiate(tabDefs[i].streamedImagePrefab);
				icon.transform.SetParent(transform, worldPositionStays: false);
				icon.transform.SetAsFirstSibling();
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Debug.LogError("No TabDef exists for accessory category: " + tabId);
			return;
		}
		button.onClick.AddListener(() =>
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (ITabSelected x, BaseEventData y) =>
			{
				x.TabSelected(tabId);
			});
		});
	}

	public void UpdateHighlightState()
	{
		List<Highlight<HighlightAccessory>> highLights = HighlightManager.GetHighLights<HighlightAccessory>(HighlightType.Accessory);
		List<AccessoryDataClient> accessoriesByCategoryId = AccessoryDataManager.GetAccessoriesByCategoryId((AccessoryCategory)category);
		int num = 0;
		for (int i = 0; i < highLights.Count; i++)
		{
			for (int j = 0; j < accessoriesByCategoryId.Count; j++)
			{
				if (accessoriesByCategoryId[j].accessoryMetaDataID == highLights[i].highlightData.accessoryMetaDataId && accessoriesByCategoryId[j].GetShowInShop())
				{
					num++;
				}
			}
		}
		redDot.SetActive(num > 0);
		redDotCount.text = num.ToString();
	}

	public override void SetAsSelected()
	{
		icon.color = Styles.GetColor(ColorStyle.SelectedTab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
		{
			x.OpenCategoryScreen(category != AccessoryCategoryClient.Featured);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
		{
			x.DisplayFlare(category == AccessoryCategoryClient.Featured);
		});
		StopAllCoroutines();
		StartCoroutine(LerpToSize(selectedTabHeight));
	}

	public override void SetAsDeselected()
	{
		icon.color = Styles.GetColor(ColorStyle.OffWhite);
		StopAllCoroutines();
		StartCoroutine(LerpToSize(defaultHeight));
	}

	private IEnumerator LerpToSize(float size)
	{
		startTime = Time.time;
		float height = layoutElement.minHeight;
		while (Time.time - startTime <= lerpTime)
		{
			layoutElement.minHeight = Mathf.Lerp(height, size, (Time.time - startTime) / lerpTime);
			yield return 0;
		}
		layoutElement.minHeight = Mathf.Lerp(height, size, 1f);
	}
}
