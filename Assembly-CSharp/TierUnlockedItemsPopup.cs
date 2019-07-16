using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class TierUnlockedItemsPopup : MonoBehaviour
{
	[SerializeField]
	private Transform itemElementContainer;

	[SerializeField]
	private TierUnlockedItemElement tierUnlockedItemElementPrefab;

	public void Initialize(GamePassTier tier, Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopData)
	{
		int num = 0;
		foreach (List<MVWorldObjectClient> value in tierShopData.Values)
		{
			TierUnlockedItemElement tierUnlockedItemElement = Object.Instantiate(tierUnlockedItemElementPrefab);
			tierUnlockedItemElement.transform.SetParent(itemElementContainer, worldPositionStays: false);
			tierUnlockedItemElement.Initialize(value, num);
			num++;
		}
	}
}
