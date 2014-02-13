using System;
using System.Collections;
using Localize;
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

	private Hashtable purchaseResponseData;

	private int priceGold;

	private int priceSilver;

	private bool allowInsert;

	private bool isInitialized;

	public void SetAllowInsertProductPreview(bool allowInsert)
	{
		this.allowInsert = allowInsert;
		((Component)previewButton).gameObject.SetActiveRecursively(this.allowInsert);
	}

	public void SetPrice(int gold, int silver)
	{
		priceGold = gold;
		priceSilver = silver;
		goldPriceText.Text = priceGold + string.Empty;
		silverPriceText.Text = priceSilver + string.Empty;
		if (priceGold == 0)
		{
			goldPriceText.SetAlpha(0.3f);
			goldCubePlane.SetAlpha(0.5f, "_MainColor");
		}
		if (priceSilver == 0)
		{
			silverPriceText.SetAlpha(0.3f);
			silverCubePlane.SetAlpha(0.5f, "_MainColor");
		}
	}

	public override void OnShowDialog()
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
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
		((Component)previewButton).gameObject.SetActiveRecursively(allowInsert);
		if (!allowInsert)
		{
			Transform transform = ((Component)productDescription).transform;
			transform.localPosition += 4f * Vector3.left;
		}
		waitText.SetVisible(visible: false);
		DialogWindow.MoveHeader(new Vector3(-6f, DialogWindow.Height / 2f - 1f, -0.1f));
	}

	public override object GetResult()
	{
		return purchaseResponseData;
	}

	private void OnClickInsertProductPreview()
	{
		if (OnInsertProductPreview != null)
		{
			DialogFactory.CreateDialog(TextSlotIndex.PreviewMessage, TextSlotIndex.InsertPreview, UXDialogType.Simple, noButtons: false, stackDialog: true).AddPositiveButton(TextSlotIndex.Ok).SetOnResultCallback(OnInsertAlertDialogResult)
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
			MVNetworkGame game = MVGameController.Instance.Game;
			game.PurchaseProductResponseHandler = (Action<int, Hashtable>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Hashtable>(ProductPurchaseResponseHandler));
			OnTryPurchaseProduct();
			((Component)purchaseButton).gameObject.SetActiveRecursively(false);
			waitText.SetVisible(visible: true);
			DialogWindow.GetExitButton().SetVisible(visible: false);
		}
	}

	private void ProductPurchaseResponseHandler(int returnCode, Hashtable purchaseResponseData)
	{
		MVNetworkGame game = MVGameController.Instance.Game;
		game.PurchaseProductResponseHandler = (Action<int, Hashtable>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Hashtable>(ProductPurchaseResponseHandler));
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
		if (returnCode == 3)
		{
			if (priceGold > 0 && priceSilver == 0)
			{
				Application.ExternalCall("attentionGetGold", new object[1] { 1 });
				DialogFactory.CreateDialog(TextSlotIndex.GetGoldMessage, TextSlotIndex.InsufficientFunds, UXDialogType.Simple, noButtons: false, stackDialog: true).SetOnResultCallback(OnGoldPurchaseDialogResult).Show();
			}
			else if (priceGold == 0 && priceSilver > 0)
			{
				DialogFactory.CreateDialog(TextSlotIndex.NotEnoughSilver, TextSlotIndex.InsufficientFunds, UXDialogType.Simple, noButtons: false, stackDialog: true).Show();
			}
			else
			{
				DialogFactory.CreateDialog(TextSlotIndex.NotEnoughFunds, TextSlotIndex.InsufficientFunds, UXDialogType.Simple, noButtons: false, stackDialog: true).Show();
			}
		}
		else
		{
			DialogFactory.CreateDialog(TextSlotIndex.ErrorOccured, TextSlotIndex.ErrorHeadline, UXDialogType.Simple, noButtons: false, stackDialog: true, canClose: true, new ValueInsert().AddInt(returnCode)).Show();
		}
	}

	public void OnGoldPurchaseDialogResult(UXDialogBox dialog)
	{
		Application.ExternalCall("attentionGetGold", new object[1] { 0 });
	}
}
