using System;
using System.Collections.Generic;
using MV.WorldObject.Accessories;
using UnityEngine;
using UnityEngine.UI;

public class AccessoryItemBackground : MonoBehaviour
{
	[SerializeField]
	private Image rarityImage;

	[SerializeField]
	private Image glowImage;

	[SerializeField]
	private Image backgroundRay;

	[SerializeField]
	private List<RarityColor> rarities;

	private static Dictionary<PriceRange, RarityColor> priceColors;

	public void Initialize(AccessoryDataClient accessoryData)
	{
		if (priceColors == null)
		{
			priceColors = new Dictionary<PriceRange, RarityColor>();
			for (int i = 0; i < rarities.Count; i++)
			{
				priceColors.Add(rarities[i].priceClass, rarities[i]);
			}
		}
		RarityColor colorFromPrice = GetColorFromPrice(accessoryData.priceGold);
		rarityImage.color = colorFromPrice.correspondingColor;
		glowImage.color = colorFromPrice.glowColor;
		backgroundRay.gameObject.SetActive(accessoryData.isFeatured);
		AccessoryBundleClient accessoryBundleClient = AccessoryDataManager.GetAccessoryBundleClient();
		List<AccessoryBundleItem> accessoryBundleItems = accessoryBundleClient.accessoryBundleItems;
		for (int j = 0; j < accessoryBundleItems.Count; j++)
		{
			if (accessoryBundleItems[j].accessoryMetaDataID == accessoryData.accessoryMetaDataID)
			{
				backgroundRay.gameObject.SetActive(value: true);
				break;
			}
		}
	}

	public static RarityColor GetColorFromPrice(int price)
	{
		Array values = Enum.GetValues(typeof(PriceRange));
		for (int i = 0; i < values.Length; i++)
		{
			if (price <= (int)values.GetValue(i))
			{
				return priceColors[(PriceRange)(int)values.GetValue(i)];
			}
		}
		return priceColors[PriceRange.PriceLegendary];
	}
}
