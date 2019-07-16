using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class GameTierShopRepository
{
	private Dictionary<GamePassTier, Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>>> tierShopData = new Dictionary<GamePassTier, Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>>>();

	public Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> GetTierItemData(GamePassTier tier)
	{
		if (!tierShopData.ContainsKey(tier))
		{
			return null;
		}
		return tierShopData[tier];
	}

	public void AddItemToTierShop(GamePassTier tier, MVWorldObjectDocumentationType documentationType, MVWorldObjectClient worldObject)
	{
		if (!tierShopData.ContainsKey(tier))
		{
			tierShopData.Add(tier, new Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>>());
		}
		if (!tierShopData[tier].ContainsKey(documentationType))
		{
			tierShopData[tier].Add(documentationType, new List<MVWorldObjectClient>());
		}
		tierShopData[tier][documentationType].Add(worldObject);
	}

	public void RemoveItemToTierShop(GamePassTier tier, MVWorldObjectDocumentationType documentationType, int woid)
	{
		if (!tierShopData.ContainsKey(tier))
		{
			Debug.LogError("Cant remove item " + documentationType.ToString() + " data from tier shop since there is no data for " + tier);
			return;
		}
		if (!tierShopData[tier].ContainsKey(documentationType))
		{
			Debug.LogError("Cant remove item " + documentationType.ToString() + " data from tier shop since " + tier.ToString() + " does not have its data");
			return;
		}
		for (int i = 0; i < tierShopData[tier][documentationType].Count; i++)
		{
			if (tierShopData[tier][documentationType][i].Id == woid)
			{
				tierShopData[tier][documentationType].RemoveAt(i);
				break;
			}
		}
		if (tierShopData[tier][documentationType].Count == 0)
		{
			tierShopData[tier].Remove(documentationType);
		}
		if (tierShopData[tier].Count == 0)
		{
			tierShopData.Remove(tier);
		}
	}
}
