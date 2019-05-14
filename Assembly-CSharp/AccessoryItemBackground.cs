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
		rarityStylesDef = ((accessoryData.lvl == 0 || accessoryData.cost != 0) ? Styles.GetAccessoryColorsFromPrice(accessoryData.cost) : Styles.GetAccessoryColorsFromLevel(accessoryData.lvl));
		rarityImage.color = rarityStylesDef.backgroundColor;
		glowImage.color = rarityStylesDef.glowColor;
		backgroundRay.gameObject.SetActive(value: false);
		if (accessoryData.owns)
		{
			return;
		}
		backgroundRay.gameObject.SetActive(accessoryData.iFtr);
		AccessoryBundleClient accessoryBundleClient = AccessoryDataManager.AccessoryBundleClient;
		List<AccessoryBundleItem> accessoryBundleItems = accessoryBundleClient.accessoryBundleItems;
		for (int i = 0; i < accessoryBundleItems.Count; i++)
		{
			if (accessoryBundleItems[i].accessoryMetaDataID == accessoryData.aMDID)
			{
				backgroundRay.gameObject.SetActive(value: true);
				break;
			}
		}
	}
}
