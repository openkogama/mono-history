using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIProductShopDialog : UXCustomDialogBox
{
	public delegate void OnTryPurchaseProductDelegate();

	public delegate void OnInsertProductPreviewDelegate();

	public OnTryPurchaseProductDelegate OnTryPurchaseProduct;

	public OnInsertProductPreviewDelegate OnInsertProductPreview;

	public UXTextButton previewButton;

	public UXText goldPriceText;

	public UXPlane goldCubePlane;

	public UXText silverPriceText;

	public UXPlane silverCubePlane;

	public UXText waitText;

	public UXTextButton purchaseButton;

	public UXText productDescription;

	public GameObject productPreview;

	private Dictionary<object, object> purchaseResponseData;

	private int priceGold;

	private int priceSilver;

	private bool allowInsert;

	private bool isInitialized;

	public void SetAllowInsertProductPreview(bool allowInsert)
	{
		this.allowInsert = allowInsert;
		previewButton.gameObject.SetActive(this.allowInsert);
	}

	public void SetPrice(int gold, int silver)
	{
		priceGold = gold;
		priceSilver = silver;
		goldPriceText.Text = priceGold + string.Empty;
		silverPriceText.Text = priceSilver + string.Empty;
		if (priceGold == 0)
		{
			goldPriceText.SetAlpha(0.3f, string.Empty);
			goldCubePlane.SetAlpha(0.5f, string.Empty);
		}
		if (priceSilver == 0)
		{
			silverPriceText.SetAlpha(0.3f, string.Empty);
			silverCubePlane.SetAlpha(0.5f, string.Empty);
		}
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!isInitialized)
		{
			productDescription.WordWrap = true;
			UXTextButton uXTextButton = purchaseButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, new UXBaseButton.OnClickDelegate(OnPurchaseClick));
			UXTextButton uXTextButton2 = previewButton;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, new UXBaseButton.OnClickDelegate(OnClickInsertProductPreview));
			isInitialized = true;
		}
		previewButton.gameObject.SetActive(allowInsert);
		if (!allowInsert)
		{
			productDescription.transform.localPosition += 4f * Vector3.left;
		}
		waitText.SetVisible(visible: false);
		DialogWindow.MoveHeader(new Vector3(-6f, DialogWindow.Height / 2f - 1f, -0.1f));
		DialogResult = UXDialogResult.Negative;
	}

	public override object GetResult()
	{
		return purchaseResponseData;
	}

	private void OnClickInsertProductPreview()
	{
		if (OnInsertProductPreview != null)
		{
			DialogFactory.CreateDialog(TM._("Since you are previewing this item,\nit will be deleted when you exit."), TM._("Insert Preview"), UXDialogType.Simple, noButtons: false, stackDialog: true).AddPositiveButton(TM._("Ok")).SetOnResultCallback(OnInsertAlertDialogResult)
				.Show();
		}
	}

	private void OnInsertAlertDialogResult(UXDialogBox dialogBox)
	{
		OnNegativeClose();
		DialogFactory.CloseDialog();
		OnInsertProductPreview();
	}

	private void OnPurchaseClick()
	{
		if (OnTryPurchaseProduct != null)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
			OnTryPurchaseProduct();
			purchaseButton.gameObject.SetActive(value: false);
			waitText.SetVisible(visible: true);
			DialogWindow.GetExitButton().SetVisible(visible: false);
		}
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
			ShowErrorDialog(returnCode);
		}
	}

	private void ShowErrorDialog(int returnCode)
	{
		if (returnCode == 1)
		{
			if (priceGold > 0 && priceSilver == 0)
			{
				DialogFactory.CreateDialog(TM._("Get more gold?"), TM._("Insufficient Funds"), UXDialogType.Simple, noButtons: false, stackDialog: true).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
					.SetOnResultCallback(OnGoldPurchaseDialogResult)
					.Show();
			}
			else if (priceGold == 0 && priceSilver > 0)
			{
				DialogFactory.CreateDialog(TM._("Get more silver"), TM._("Insufficient Funds"), UXDialogType.Simple, noButtons: false, stackDialog: true).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
					.SetOnResultCallback(OnSilverConvertDialogResult)
					.Show();
			}
			else
			{
				DialogFactory.CreateDialog(TM._("Get more gold?"), TM._("Insufficient Funds"), UXDialogType.Simple, noButtons: false, stackDialog: true).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
					.SetOnResultCallback(OnGoldPurchaseDialogResult)
					.Show();
			}
		}
		else
		{
			DialogFactory.CreateDialog(TM._("An error occured:\n"), TM._("Error"), UXDialogType.Simple, noButtons: false, stackDialog: true, canClose: true, new ValueInsert().AddInt(returnCode)).Show();
		}
	}

	public void OnSilverConvertDialogResult(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoConvertToSilver");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.convertToSilverURL);
		}
	}

	public void OnGoldPurchaseDialogResult(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoPurchaseGold");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.purchaseGoldURL);
		}
	}
}
