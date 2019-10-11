using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoldPurchasedTracker : MonoBehaviour
{
	private int goldDeltaExpected;

	private int currentGold;

	private int goldGainedTotal;

	private const string goldPendingUpdate = "GoldPendingUpdate";

	public void Initialize()
	{
		BrowserComm browserComm = MVGameControllerBase.BrowserComm;
		browserComm.OnGoldPurchasedFromWeb = (Action<int, int>)Delegate.Combine(browserComm.OnGoldPurchasedFromWeb, new Action<int, int>(StartGoldPurchasePendingUpdate));
	}

	private void OnDestroy()
	{
		if (IsInvoking("GoldPendingUpdate"))
		{
			CancelInvoke("GoldPendingUpdate");
		}
		if (MVGameControllerBase.IsAlive)
		{
			BrowserComm browserComm = MVGameControllerBase.BrowserComm;
			browserComm.OnGoldPurchasedFromWeb = (Action<int, int>)Delegate.Remove(browserComm.OnGoldPurchasedFromWeb, new Action<int, int>(StartGoldPurchasePendingUpdate));
			MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
			localPlayer.OnGoldAmountChange = (Action)Delegate.Remove(localPlayer.OnGoldAmountChange, new Action(GoldUpdatedCallback));
		}
	}

	public void StartGoldPurchasePendingUpdate(int currentGold, int goldDelta)
	{
		this.currentGold = currentGold;
		goldDeltaExpected += goldDelta;
		goldGainedTotal = 0;
		Debug.Log("Requesting gold purchase update");
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(TM._("Thank you for your purchase! It may take up to 10 minutes before your gold is available"), TM._("Purchase pending"));
		});
		if (!IsInvoking("GoldPendingUpdate"))
		{
			MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
			localPlayer.OnGoldAmountChange = (Action)Delegate.Combine(localPlayer.OnGoldAmountChange, new Action(GoldUpdatedCallback));
			InvokeRepeating("GoldPendingUpdate", 5f, 30f);
		}
	}

	public void GoldPendingUpdate()
	{
		Debug.Log("Invoke: Requesting gold update response from server");
		MVGameControllerBase.Game.OperationRequestSender.RequestUpdateGoldResponse();
	}

	private void GoldUpdatedCallback()
	{
		goldGainedTotal += Mathf.Max(0, MVGameControllerBase.Game.LocalPlayer.UserProfileData.Gold - currentGold);
		currentGold = MVGameControllerBase.Game.LocalPlayer.UserProfileData.Gold;
		Debug.Log("currentGold: " + currentGold + " and delta is " + goldGainedTotal + ". Expecting a total of: " + goldDeltaExpected);
		if (goldGainedTotal >= goldDeltaExpected)
		{
			goldGainedTotal = 0;
			goldDeltaExpected = 0;
			BrowserComm.ToJavaScript.ExternalCall("refreshCredentials");
			NotificationController.PushNotification(TM._("Thank you for waiting! Your purchased gold should now be available."));
			MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
			localPlayer.OnGoldAmountChange = (Action)Delegate.Remove(localPlayer.OnGoldAmountChange, new Action(GoldUpdatedCallback));
			CancelInvoke("GoldPendingUpdate");
			Debug.Log("Finished calling Gold Update. Disabling invoke, total gold: " + MVGameControllerBase.Game.LocalPlayer.UserProfileData.Gold);
		}
	}
}
