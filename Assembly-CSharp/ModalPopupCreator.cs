using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ModalPopupCreator : MonoBehaviour, IEventSystemHandler, IModalPopupCreator
{
	[SerializeField]
	private UIPushOption popupPushOption;

	[SerializeField]
	private ConfirmationPopup confirmationPopupPrefab;

	[SerializeField]
	private NotificationPopup notificationPopupPrefab;

	[SerializeField]
	private PleaseWaitPopup waitPopupPrefab;

	public NotificationPopup Create(string text, string header = "")
	{
		NotificationPopup notificationPopup = Object.Instantiate(notificationPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(notificationPopup.gameObject, popupPushOption, null, UIGroupFlags.Popup);
		});
		notificationPopup.Initialize(text, header);
		return notificationPopup;
	}

	public ConfirmationPopup Create(string text, UnityAction<bool, ConfirmationPopup> resultCallback, string header = "")
	{
		ConfirmationPopup confirmationPopup = Object.Instantiate(confirmationPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(confirmationPopup.gameObject, popupPushOption, null, UIGroupFlags.Popup);
		});
		confirmationPopup.Initialize(text, resultCallback, header);
		return confirmationPopup;
	}

	public PleaseWaitPopup Create()
	{
		PleaseWaitPopup waitPopup = Object.Instantiate(waitPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(waitPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
		});
		return waitPopup;
	}

	public void Create(MVPurchaseReturnCode returnCode, int priceGold, int priceSilver)
	{
		if (returnCode == MVPurchaseReturnCode.InsufficientFunds)
		{
			ConfirmationPopup confirmationPopup = Object.Instantiate(confirmationPopupPrefab);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(confirmationPopup.gameObject, popupPushOption, null, UIGroupFlags.Popup);
			});
			if (priceGold > 0 && priceSilver == 0)
			{
				confirmationPopup.Initialize(TM._("Get more gold now?"), OnGoldPurchaseDialogResult, TM._("Not enough gold"));
			}
			else if (priceGold == 0 && priceSilver > 0)
			{
				confirmationPopup.Initialize(TM._("Get more silver now?"), OnSilverPurchaseDialogResult, TM._("Not enough silver"));
			}
			else
			{
				confirmationPopup.Initialize(TM._("Get more gold now?"), OnGoldPurchaseDialogResult, TM._("Not enough gold"));
			}
		}
		else
		{
			CreateErrorNotificationPopup(returnCode.ToString());
		}
	}

	public void CreateErrorNotificationPopup(string error)
	{
		Create(TM._("An error occured:\n\n") + error, TM._("Error"));
	}

	private static void OnGoldPurchaseDialogResult(bool result, ConfirmationPopup confirmationPopup)
	{
		confirmationPopup.Pop();
		if (result)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoPurchaseGold");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.purchaseGoldURL);
		}
	}

	private static void OnSilverPurchaseDialogResult(bool result, ConfirmationPopup confirmationPopup)
	{
		confirmationPopup.Pop();
		if (result)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoConvertToSilver");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.purchaseGoldURL);
		}
	}
}
