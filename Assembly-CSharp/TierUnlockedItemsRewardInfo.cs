using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TierUnlockedItemsRewardInfo : MonoBehaviour, IGamePassShopContent
{
	[SerializeField]
	private GameObject backgroundTier1;

	[SerializeField]
	private GameObject backgroundTier2;

	[SerializeField]
	private GameObject backgroundTier3;

	[SerializeField]
	private Text itemAmountText;

	[SerializeField]
	private TierUnlockedItemsPopup tierUnlockedItemsPopupPrefab;

	private GamePassTier tier;

	private Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopData;

	public void Initialize(GamePassTier tier, Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopData)
	{
		this.tier = tier;
		this.tierShopData = tierShopData;
		ChangeBackground(tier);
		UpdateItemAmountText();
	}

	public void OnSeeItemsButtonPressed()
	{
		TierUnlockedItemsPopup tierUnlockedItemsPopup = Object.Instantiate(tierUnlockedItemsPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierUnlockedItemsPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierUnlockedItemsPopup.Initialize(tier, tierShopData);
	}

	public void Activate()
	{
	}

	public void Deactivate()
	{
	}

	private void ChangeBackground(GamePassTier tier)
	{
		bool flag = tier == GamePassTier.Tier1;
		bool flag2 = tier == GamePassTier.Tier2;
		bool flag3 = tier == GamePassTier.Tier3;
		if (backgroundTier1.activeSelf != flag)
		{
			backgroundTier1.SetActive(flag);
		}
		if (backgroundTier2.activeSelf != flag2)
		{
			backgroundTier2.SetActive(flag2);
		}
		if (backgroundTier3.activeSelf != flag3)
		{
			backgroundTier3.SetActive(flag3);
		}
	}

	private void UpdateItemAmountText()
	{
		int num = 0;
		foreach (List<MVWorldObjectClient> value in tierShopData.Values)
		{
			num += value.Count;
		}
		itemAmountText.text = "x" + num;
	}
}
