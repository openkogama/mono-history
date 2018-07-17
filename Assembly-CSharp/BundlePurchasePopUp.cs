using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class BundlePurchasePopUp : MonoBehaviour
{
	private AccessoryBundleClient bundleDataClient;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private Text originalPriceText;

	[SerializeField]
	private GameObject discountTag;

	[SerializeField]
	private Text discountTagText;

	[SerializeField]
	private AccessoryTimeLimitDisplayer timeLimitDisplayer;

	private int originalPrice;

	private UnityAction<bool> resultCallback;

	public void Initialize(AccessoryBundleClient bundleDataClient, int price, int originalPrice, UnityAction<bool> resultCallback)
	{
		this.bundleDataClient = bundleDataClient;
		this.originalPrice = originalPrice;
		this.resultCallback = resultCallback;
		priceText.text = price.ToString();
		HandleNotOwnedUI();
	}

	public void Purchase()
	{
		resultCallback(arg0: true);
	}

	public void Exit()
	{
		resultCallback(arg0: false);
	}

	private void OnGoldPurchaseDialogResult(bool result)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (result)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoPurchaseGold");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.purchaseGoldURL);
		}
	}

	public void Pop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void HandlePrices(AccessoryBundleClient bundleDataClient)
	{
		int discount = bundleDataClient.discount;
		int num = originalPrice;
		originalPriceText.gameObject.SetActive(discount > 0);
		discountTag.SetActive(discount > 0);
		if (discount > 0)
		{
			discountTagText.text = ((discount < 100) ? ("-" + discount + "%") : "FREE");
			int num2 = Mathf.FloorToInt((float)originalPrice * ((float)discount / 100f));
			num = originalPrice - num2;
			originalPriceText.text = originalPrice.ToString("N0");
		}
		priceText.text = num.ToString("N0");
	}

	private void HandleNotOwnedUI()
	{
		HandlePrices(bundleDataClient);
		timeLimitDisplayer.Initialize(bundleDataClient.timelimit);
		timeLimitDisplayer.gameObject.SetActive(bundleDataClient.timelimit.IsTimeLimited);
	}
}
