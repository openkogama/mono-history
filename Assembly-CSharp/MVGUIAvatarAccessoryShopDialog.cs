using System;
using System.Collections;
using System.Linq;
using Localize;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessoryShopDialog : UXCustomDialogBox
{
	public UXGroup rentGroup;

	public UXText rentGoldPriceText;

	public UXGroup rentGoldGroup;

	public UXText rentSilverPriceText;

	public UXGroup rentSilverGroup;

	public UXTextButton rentButton;

	public UXText rentTimeText;

	public UXGroup purchaseGroup;

	public UXText purchaseGoldPriceText;

	public UXGroup purchaseGoldGroup;

	public UXText purchaseSilverPriceText;

	public UXGroup purchaseSilverGroup;

	public UXTextButton purchaseButton;

	public UXTextButton removeButton;

	public UXText waitText;

	public UXText accessoryNameHeader;

	public UXText accessoryNameHeaderExpired;

	private int priceGold;

	private int priceSilver;

	private Hashtable purchaseResponseData;

	private bool isRenting;

	private bool renewingItem;

	private int expiredInventoryID;

	private bool isInitialized;

	private int purchasedInventoryID = -1;

	private AvatarAccessory accessoryToBeEquipped;

	private StreamingAssetInfo streamingAssetInfo;

	private MVNetworkGame Game => MVGameController.Instance.Game;

	private CharacterEditorController CEController => MVGameController.Instance.CharacterEditorController;

	private MVBody AvatarBody
	{
		get
		{
			if (Game.GameMode == MVGameMode.CharacterEditor)
			{
				return CEController.CurrentBody;
			}
			return MVGameController.Instance.WOCM.AvatarLocal.Body;
		}
	}

	private ProductShopInfo ShopInfo => streamingAssetInfo.ShopInfo;

	public void BuildShopDialogForRentRenewal(StreamingAssetInfo streamingAssetInfo, int expiredInventoryID)
	{
		this.expiredInventoryID = expiredInventoryID;
		renewingItem = true;
		BuildShopDialog(streamingAssetInfo);
	}

	public void BuildShopDialog(StreamingAssetInfo streamingAssetInfo)
	{
		this.streamingAssetInfo = streamingAssetInfo;
		if (ShopInfo != null)
		{
			accessoryNameHeader.SetVisible(!renewingItem);
			accessoryNameHeaderExpired.SetVisible(renewingItem);
			purchaseGoldPriceText.Text = ShopInfo.PriceGold.ToString();
			purchaseSilverPriceText.Text = ShopInfo.PriceSilver.ToString();
			rentGoldPriceText.Text = ShopInfo.RentPriceGold.ToString();
			rentSilverPriceText.Text = ShopInfo.RentPriceSilver.ToString();
			if (ShopInfo.IsBuyable)
			{
				if (ShopInfo.PriceGold == 0)
				{
					purchaseGoldGroup.SetAlpha(0.4f);
				}
				if (ShopInfo.RentPriceSilver == 0)
				{
					purchaseSilverGroup.SetAlpha(0.4f);
				}
			}
			else
			{
				purchaseGroup.SetAlpha(0.4f);
			}
			if (ShopInfo.IsRentable)
			{
				SetRentText(ShopInfo.RentExpireSeconds);
				if (ShopInfo.RentPriceGold == 0)
				{
					rentGoldGroup.SetAlpha(0.4f);
				}
				if (ShopInfo.RentPriceSilver == 0)
				{
					rentSilverGroup.SetAlpha(0.4f);
				}
			}
			else
			{
				rentGroup.SetAlpha(0.4f);
			}
			((Component)purchaseButton).collider.enabled = ShopInfo.IsBuyable;
			((Component)rentButton).collider.enabled = ShopInfo.IsRentable;
			((Component)removeButton).collider.enabled = renewingItem;
			removeButton.SetVisible(renewingItem);
		}
		else
		{
			Debug.LogError((object)"Trying to build shop with no shop info");
		}
	}

	private void SetRentText(int rentExpireSeconds)
	{
		float num = 0f;
		TextSlotIndex textSlotIndex = TextSlotIndex.Empty;
		if (rentExpireSeconds < 3600)
		{
			num = (float)rentExpireSeconds / 60f;
			textSlotIndex = TextSlotIndex.ForMinutes;
		}
		else if (rentExpireSeconds < 86400)
		{
			num = (float)rentExpireSeconds / 60f / 60f;
			textSlotIndex = TextSlotIndex.ForHours;
		}
		else
		{
			num = (float)rentExpireSeconds / 60f / 60f / 24f;
			textSlotIndex = TextSlotIndex.ForDays;
		}
		ValueInsert valueInsert = new ValueInsert();
		valueInsert.AddString($"{num:0.#}");
		string text = Localization.Instance.GetTextWithValues(textSlotIndex, valueInsert);
		if (num == 1f)
		{
			text = text.Substring(0, text.Length - 1);
		}
		rentTimeText.Text = text;
		rentButton.Text = Localization.Instance.GetText((!renewingItem) ? TextSlotIndex.Rent : TextSlotIndex.Extend);
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!isInitialized)
		{
			UXTextButton uXTextButton = purchaseButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, new UXBaseButton.OnClickDelegate(OnPurchaseClick));
			UXTextButton uXTextButton2 = rentButton;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, new UXBaseButton.OnClickDelegate(OnRentClick));
			UXTextButton uXTextButton3 = removeButton;
			uXTextButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton3.OnClick, new UXBaseButton.OnClickDelegate(OnRemoveClick));
			isInitialized = true;
		}
		waitText.SetVisible(visible: false);
	}

	public override object GetResult()
	{
		return purchaseResponseData;
	}

	private void OnPurchaseClick()
	{
		TryPurchaseAvatarAccessory(ShopInfo.PriceGold, ShopInfo.PriceSilver, isRenting: false);
	}

	private void OnRentClick()
	{
		TryPurchaseAvatarAccessory(ShopInfo.RentPriceGold, ShopInfo.RentPriceSilver, isRenting: true);
	}

	private void TryPurchaseAvatarAccessory(int priceGold, int priceSilver, bool isRenting)
	{
		this.priceGold = priceGold;
		this.priceSilver = priceSilver;
		this.isRenting = isRenting;
		MVNetworkGame game = Game;
		game.PurchaseProductResponseHandler = (Action<int, Hashtable>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Hashtable>(ProductPurchaseResponseHandler));
		if (isRenting)
		{
			if (renewingItem)
			{
				MVBody bodyOfEquippedItem = GetBodyOfEquippedItem(expiredInventoryID);
				if (bodyOfEquippedItem != null)
				{
					Game.ExtendRentAvatarAccessory(streamingAssetInfo.ProductID, expiredInventoryID, bodyOfEquippedItem.Id, bodyOfEquippedItem.GetAvatarAccessorySlot(expiredInventoryID), 0f);
				}
				else
				{
					Game.ExtendRentAvatarAccessory(streamingAssetInfo.ProductID, expiredInventoryID);
				}
			}
			else
			{
				Game.RentAvatarAccessory(streamingAssetInfo.ProductID);
			}
		}
		else
		{
			Game.PurchaseAvatarAccessory(streamingAssetInfo.ProductID);
		}
		purchaseButton.SetVisible(visible: false);
		rentButton.SetVisible(visible: false);
		removeButton.SetVisible(visible: false);
		rentTimeText.SetVisible(visible: false);
		waitText.SetVisible(visible: true);
		if (DialogWindow.HasExitButton)
		{
			DialogWindow.GetExitButton().SetVisible(visible: false);
		}
	}

	private MVBody GetBodyOfEquippedItem(int inventoryID)
	{
		int num = 0;
		if (Game.GameMode == MVGameMode.CharacterEditor)
		{
			CharacterEditorController characterEditorController = MVGameController.Instance.IngameController as CharacterEditorController;
			foreach (MVBody body in characterEditorController.Bodies)
			{
				MVBody mVBody = body;
				if (mVBody.HasAccessoryWithID(inventoryID))
				{
					num = mVBody.Id;
					break;
				}
			}
		}
		else
		{
			num = MVGameController.Instance.WOCM.AvatarLocal.Body.Id;
		}
		if (num != 0)
		{
			return MVGameController.Instance.WOCM.GetWorldObjectClient(num) as MVBody;
		}
		return null;
	}

	private void ProductPurchaseResponseHandler(int returnCode, Hashtable purchaseResponseData)
	{
		MVNetworkGame game = Game;
		game.PurchaseProductResponseHandler = (Action<int, Hashtable>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Hashtable>(ProductPurchaseResponseHandler));
		waitText.SetVisible(visible: false);
		if (returnCode == 0)
		{
			this.purchaseResponseData = purchaseResponseData;
			HandleSuccessfulPurchase(purchaseResponseData);
			return;
		}
		purchaseButton.SetVisible(visible: true);
		rentButton.SetVisible(visible: true);
		removeButton.SetVisible(visible: true);
		rentTimeText.SetVisible(visible: true);
		if (DialogWindow.HasExitButton)
		{
			DialogWindow.GetExitButton().SetVisible(visible: true);
		}
		ShowErrorDialog(returnCode);
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

	private void HandleSuccessfulPurchase(Hashtable purchaseResponseData)
	{
		purchasedInventoryID = (int)purchaseResponseData[(byte)74];
		long ticks = (long)purchaseResponseData[(byte)84];
		DateTime purchaseTime = new DateTime(ticks);
		if (!Game.StreamingAssetInventory.Contains(purchasedInventoryID))
		{
			AddToInventory(purchasedInventoryID, purchaseTime, purchaseResponseData, isRenting);
		}
		if (isRenting)
		{
			UpdateExpirationInfo(purchasedInventoryID, purchaseTime, purchaseResponseData);
		}
		if (!renewingItem || !AvatarBody.HasAccessoryWithID(expiredInventoryID))
		{
			UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
			uXDialogFactory.CreateDialog(TextSlotIndex.OfferAttachAccessory, TextSlotIndex.Empty, UXDialogType.Simple, noButtons: false, stackDialog: true).SetOnResultCallback(OnAttachOfferDialogResult).AddPositiveButton(TextSlotIndex.Confirm)
				.AddNegativeButton(TextSlotIndex.Reject)
				.Show();
		}
		else
		{
			AttachPurchasedAccessory();
		}
		if (renewingItem)
		{
			purchaseResponseData["oldInventoryID"] = expiredInventoryID;
			purchaseResponseData["newInventoryID"] = purchasedInventoryID;
		}
	}

	private void AddToInventory(int invID, DateTime purchaseTime, Hashtable purchaseResponse, bool isRenting)
	{
		int productID = streamingAssetInfo.ProductID;
		StreamingAssetInfo value = null;
		Game.StreamingAssetInfoMap.TryGetValue(productID, out value);
		if (value != null)
		{
			ProductInventoryInfo<StreamingAssetInfo> invInfo = new ProductInventoryInfo<StreamingAssetInfo>(invID, value, purchaseTime, isRenting);
			Game.StreamingAssetInventory.Add(invInfo);
			Game.StreamingAssetInventory.NotifyProductInventoryChange();
		}
		else
		{
			Debug.LogError((object)"Trying to add non-existing avatar accessory to inventory");
		}
	}

	private void UpdateExpirationInfo(int invID, DateTime purchaseTime, Hashtable purchaseResponse)
	{
		int rentExpireSeconds = (int)purchaseResponse[(byte)81];
		ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo = Game.StreamingAssetInventory.Get(invID);
		productInventoryInfo.Renew(purchaseTime, rentExpireSeconds);
		InventoryExpirationInfo expirationInfo = Game.StreamingAssetExpirationChecker.GetExpirationInfo(invID);
		if (expirationInfo != null)
		{
			expirationInfo.Renew(purchaseTime, rentExpireSeconds);
			return;
		}
		expirationInfo = new InventoryExpirationInfo(MVProductType.StreamingAsset, invID, ProductExpirationState.Expiring, purchaseTime, rentExpireSeconds);
		Game.StreamingAssetExpirationChecker.AddExpirationInfo(expirationInfo);
	}

	private void OnAttachOfferDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			AttachPurchasedAccessory();
			return;
		}
		OnPositiveClose();
		DialogFactory.CloseDialog();
		MVGameController.Instance.CharacterEditorController.AvatarAccessoryInventory.ShowInventoryUpdated();
	}

	private void AttachPurchasedAccessory()
	{
		ProductInventoryInfo<StreamingAssetInfo> invInfo = Game.StreamingAssetInventory.Get(purchasedInventoryID);
		AvatarAccessory.Create(invInfo, AvatarAccessoryCreateHandler);
	}

	private void AvatarAccessoryCreateHandler(AvatarAccessory avatarAccessory)
	{
		accessoryToBeEquipped = avatarAccessory;
		AvatarAccessory avatarAccessory2 = AvatarBody.GetAccessories(avatarAccessory.DefaultSlot).FirstOrDefault();
		if ((Object)(object)avatarAccessory2 != (Object)null)
		{
			Unequip(avatarAccessory2);
		}
		else
		{
			Equip();
		}
	}

	private void Unequip(AvatarAccessory avatarAccessory)
	{
		MVNetworkGame game = Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseUnequipHandler));
		Game.SetAvatarAccessorySlot(AvatarBody.Id, avatarAccessory.InventoryID, AvatarAccessorySlot.Undefined, 0f);
	}

	private void Game_OnSetAvatarAccessorySlotResponseUnequipHandler(bool setSlotSuccess)
	{
		MVNetworkGame game = Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Remove(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseUnequipHandler));
		if (setSlotSuccess)
		{
			Equip();
			return;
		}
		Object.Destroy((Object)(object)((Component)accessoryToBeEquipped).gameObject);
		accessoryToBeEquipped = null;
		OnPositiveClose();
		DialogFactory.CloseDialog();
	}

	private void Equip()
	{
		AvatarAccessory avatarAccessory = accessoryToBeEquipped;
		MVNetworkGame game = Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseEquipHandler));
		Game.SetAvatarAccessorySlot(AvatarBody.Id, avatarAccessory.InventoryID, avatarAccessory.DefaultSlot, avatarAccessory.DefaultOffset);
		AvatarBody.AttachAccessory(avatarAccessory, accessoryToBeEquipped.DefaultSlot, accessoryToBeEquipped.DefaultOffset);
	}

	private void Game_OnSetAvatarAccessorySlotResponseEquipHandler(bool setSlotSuccess)
	{
		MVNetworkGame game = Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Remove(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseEquipHandler));
		if (!setSlotSuccess)
		{
			AvatarBody.DestroyAccessory(purchasedInventoryID);
		}
		accessoryToBeEquipped = null;
		purchasedInventoryID = -1;
		OnPositiveClose();
		DialogFactory.CloseDialog();
	}

	private void OnRemoveClick()
	{
		purchaseResponseData = purchaseResponseData ?? new Hashtable();
		purchaseResponseData["oldInventoryID"] = expiredInventoryID;
		OnNegativeClose();
		DialogFactory.CloseDialog();
	}
}
