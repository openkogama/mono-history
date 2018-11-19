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

	public void Initialize(AccessoryDataClient accessoryData)
	{
		RarityStylesDef rarityStylesDef = null;
		rarityStylesDef = ((accessoryData.level == 0 || accessoryData.priceGold != 0) ? Styles.GetAccessoryColorsFromPrice(accessoryData.priceGold) : Styles.GetAccessoryColorsFromLevel(accessoryData.level));
		rarityImage.color = rarityStylesDef.backgroundColor;
		glowImage.color = rarityStylesDef.glowColor;
		backgroundRay.gameObject.SetActive(value: false);
		if (accessoryData.owns)
		{
			return;
		}
		backgroundRay.gameObject.SetActive(accessoryData.isFeatured);
		AccessoryBundleClient accessoryBundleClient = AccessoryDataManager.GetAccessoryBundleClient();
		List<AccessoryBundleItem> accessoryBundleItems = accessoryBundleClient.accessoryBundleItems;
		for (int i = 0; i < accessoryBundleItems.Count; i++)
		{
			if (accessoryBundleItems[i].accessoryMetaDataID == accessoryData.accessoryMetaDataID)
			{
				backgroundRay.gameObject.SetActive(value: true);
				break;
			}
		}
	}
}
