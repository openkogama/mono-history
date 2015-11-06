using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVGUIGameCoinBoostShopDialog : UXCustomDialogBox
{
	public delegate void OnTryPurchaseProductDelegate();

	private bool isInitialized;

	[SerializeField]
	private UXText waitText;

	[SerializeField]
	private UXText price;

	[SerializeField]
	private UXText productDescription;

	[SerializeField]
	private UXBaseButton purchase;

	private Dictionary<object, object> purchaseResponseData;

	public OnTryPurchaseProductDelegate OnTryPurchaseProduct;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!isInitialized)
		{
			UXBaseButton uXBaseButton = purchase;
			uXBaseButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXBaseButton.OnClick, new UXBaseButton.OnClickDelegate(OnPurchaseClick));
			price.Text = PricesManager.GetPrice("GameCoinBoost").gold.ToString();
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, CommonValues.GameCoinBoostTime);
			string arg = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
			productDescription.Text = string.Format(TM._("Boost time\n{0}"), arg);
			waitText.SetVisible(visible: false);
			isInitialized = true;
		}
		DialogResult = UXDialogResult.Negative;
	}

	private void OnPurchaseClick()
	{
		if (OnTryPurchaseProduct != null)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
			OnTryPurchaseProduct();
			purchase.gameObject.SetActive(value: false);
			waitText.SetVisible(visible: true);
			DialogWindow.GetExitButton().SetVisible(visible: false);
		}
	}

	public override object GetResult()
	{
		return purchaseResponseData;
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		if (returnCode == 0)
		{
			this.purchaseResponseData = purchaseResponseData;
			OnPositiveClose();
			DialogFactory.CloseDialog();
		}
		else
		{
			DialogWindow.GetExitButton().SetVisible(visible: true);
			PurchaseFailedDialog.ShowErrorDialog(returnCode, PricesManager.GetPrice("GameCoinBoost").gold, PricesManager.GetPrice("GameCoinBoost").silver);
			purchase.gameObject.SetActive(value: true);
			waitText.SetVisible(visible: false);
		}
	}
}
