using System.Collections.Generic;
using MV.WorldObject.Accessories;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BundleView : MonoBehaviour
{
	[SerializeField]
	private Text originalPriceText;

	[SerializeField]
	private Text discountedPriceText;

	[SerializeField]
	private GameObject discountTag;

	[SerializeField]
	private Text discountTagText;

	[SerializeField]
	private Text goldSavedText;

	public void Initialize()
	{
		AccessoryBundleClient accessoryBundleClient = AccessoryDataManager.GetAccessoryBundleClient();
		HandlePrices(accessoryBundleClient);
	}

	public void OnBundlePurchaseClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(TM._("Are you sure you wish to purchase this bundle?"), OnPurchaseBundleConfirmation, TM._("Confirm"));
		});
	}

	private void OnPurchaseBundleConfirmation(bool confirmed, ConfirmationPopup popup)
	{
		if (confirmed)
		{
			MVGameControllerBase.OperationRequests.PurchaseAvatarAccessoryBundle(AccessoryDataManager.GetAccessoryBundleId());
			return;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void HandlePrices(AccessoryBundleClient accessoryData)
	{
		int num = 0;
		int num2 = 0;
		List<AccessoryBundleItem> accessoryBundleItems = accessoryData.accessoryBundleItems;
		for (int i = 0; i < accessoryBundleItems.Count; i++)
		{
			AccessoryDataClient accessoryDataByMetaDataId = AccessoryDataManager.GetAccessoryDataByMetaDataId(accessoryBundleItems[i].accessoryMetaDataID);
			if (!accessoryDataByMetaDataId.owns)
			{
				num += accessoryDataByMetaDataId.priceGold;
				num2++;
			}
		}
		if (num2 == 0)
		{
			Debug.LogError("Bundle shown, but all items are owned");
			return;
		}
		int discount = accessoryData.discount;
		int num3 = num;
		originalPriceText.gameObject.SetActive(discount > 0);
		discountTag.SetActive(discount > 0);
		if (discount > 0)
		{
			discountTagText.text = "-" + discount + "%";
			int num4 = Mathf.FloorToInt((float)num * ((float)discount / 100f));
			num3 = num - num4;
			originalPriceText.text = num.ToString("N0");
			goldSavedText.gameObject.SetActive(value: true);
			goldSavedText.text = num4.ToString("N0");
		}
		discountedPriceText.text = num3.ToString("N0");
	}
}
