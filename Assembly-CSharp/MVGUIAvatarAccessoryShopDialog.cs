using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessoryShopDialog : UXCustomDialogBox
{
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

	private Dictionary<object, object> purchaseResponseData;

	private bool isRenting;

	private bool renewingItem;

	private int expiredInventoryID;

	private bool isInitialized;

	private int purchasedInventoryID = -1;

	private AvatarAccessory accessoryToBeEquipped;

	private StreamingAssetInfo streamingAssetInfo;

	private MVNetworkGame Game => MVGameControllerBase.Game;

	private CharacterEditorController CEController => MVGameControllerLegacyUI.CharacterEditorController;

	private MVBody AvatarBody
	{
		get
		{
			if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
			{
				return CEController.CurrentBody;
			}
			return MVGameControllerBase.WOCM.AvatarLocal.Body;
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
			if (ShopInfo.IsBuyable)
			{
				if (ShopInfo.PriceGold == 0)
				{
					purchaseGoldGroup.SetAlpha(0.4f, string.Empty);
				}
				if (ShopInfo.PriceSilver == 0)
				{
					purchaseSilverGroup.SetAlpha(0.4f, string.Empty);
				}
			}
			else
			{
				purchaseGroup.SetAlpha(0.4f, string.Empty);
			}
			purchaseButton.GetComponent<Collider>().enabled = ShopInfo.IsBuyable;
			removeButton.GetComponent<Collider>().enabled = renewingItem;
			removeButton.SetVisible(renewingItem);
		}
		else
		{
			Debug.LogError("Trying to build shop with no shop info");
		}
	}

	private void SetRentText(int rentExpireSeconds)
	{
		float num = 0f;
		string empty = string.Empty;
		if (rentExpireSeconds < 3600)
		{
			num = (float)rentExpireSeconds / 60f;
			empty = TM._("{0} minutes");
		}
		else if (rentExpireSeconds < 86400)
		{
			num = (float)rentExpireSeconds / 60f / 60f;
			empty = TM._("{0} hours");
		}
		else
		{
			num = (float)rentExpireSeconds / 60f / 60f / 24f;
			empty = TM._("{0} days");
		}
		ValueInsert valueInsert = new ValueInsert();
		valueInsert.AddString($"{num:0.#}");
		string textWithValues = TM.GetTextWithValues(empty, valueInsert);
		if (num == 1f)
		{
			textWithValues = textWithValues.Substring(0, textWithValues.Length - 1);
		}
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!isInitialized)
		{
			UXTextButton uXTextButton = purchaseButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, new UXBaseButton.OnClickDelegate(OnPurchaseClick));
			UXTextButton uXTextButton2 = removeButton;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, new UXBaseButton.OnClickDelegate(OnRemoveClick));
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
		if (!streamingAssetInfo.IsEditorPreview)
		{
			TryPurchaseAvatarAccessory(ShopInfo.PriceGold, ShopInfo.PriceSilver);
		}
	}

	private void TryPurchaseAvatarAccessory(int priceGold, int priceSilver)
	{
		this.priceGold = priceGold;
		this.priceSilver = priceSilver;
		MVNetworkGame game = Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		Game.PurchaseAvatarAccessory(streamingAssetInfo.ProductID);
		purchaseButton.SetVisible(visible: false);
		removeButton.SetVisible(visible: false);
		waitText.SetVisible(visible: true);
		if (DialogWindow.HasExitButton)
		{
			DialogWindow.GetExitButton().SetVisible(visible: false);
		}
	}

	private MVBody GetBodyOfEquippedItem(int inventoryID)
	{
		int num = 0;
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			CharacterEditorController characterEditorController = MVGameControllerLegacyUI.IngameController as CharacterEditorController;
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
			num = MVGameControllerBase.WOCM.AvatarLocal.Body.Id;
		}
		if (num != 0)
		{
			return MVGameControllerBase.WOCM.GetWorldObjectClient(num) as MVBody;
		}
		return null;
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		waitText.SetVisible(visible: false);
		if (returnCode == 0)
		{
			this.purchaseResponseData = purchaseResponseData;
			HandleSuccessfulPurchase(purchaseResponseData);
			return;
		}
		purchaseButton.SetVisible(visible: true);
		removeButton.SetVisible(visible: true);
		if (DialogWindow.HasExitButton)
		{
			DialogWindow.GetExitButton().SetVisible(visible: true);
		}
		PurchaseFailedDialog.ShowErrorDialog(returnCode, priceGold, priceSilver);
	}

	private void HandleSuccessfulPurchase(Dictionary<object, object> purchaseResponseData)
	{
		purchasedInventoryID = (int)purchaseResponseData[(byte)73];
		long ticks = (long)purchaseResponseData[(byte)83];
		DateTime purchaseTime = new DateTime(ticks);
		if (!Game.StreamingAssetInventory.Contains(purchasedInventoryID))
		{
			AddToInventory(purchasedInventoryID, purchaseTime, purchaseResponseData);
		}
		if (!renewingItem || !AvatarBody.HasAccessoryWithID(expiredInventoryID))
		{
			UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
			uXDialogFactory.CreateDialog(TM._("Attach to avatar now?"), string.Empty, UXDialogType.Simple, noButtons: false, stackDialog: true).SetOnResultCallback(OnAttachOfferDialogResult).AddPositiveButton(TM._("Yes"))
				.AddNegativeButton(TM._("No"))
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

	private void AddToInventory(int invID, DateTime purchaseTime, Dictionary<object, object> purchaseResponse)
	{
		int productID = streamingAssetInfo.ProductID;
		StreamingAssetInfo value = null;
		Game.StreamingAssetInfoMap.TryGetValue(productID, out value);
		if (value != null)
		{
			ProductInventoryInfo invInfo = new ProductInventoryInfo(invID, value, purchaseTime);
			Game.StreamingAssetInventory.Add(invInfo);
			Game.StreamingAssetInventory.NotifyProductInventoryChange();
		}
		else
		{
			Debug.LogError("Trying to add non-existing avatar accessory to inventory");
		}
	}

	private void UpdateExpirationInfo(int invID, DateTime purchaseTime, Dictionary<object, object> purchaseResponse)
	{
		int rentExpireSeconds = (int)purchaseResponse[(byte)80];
		ProductInventoryInfo productInventoryInfo = Game.StreamingAssetInventory.Get(invID);
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
		UXUtils.FindGUIObjectOfType<AvatarAccessoryController>().AvatarAccessoryInventory.ShowInventoryUpdated();
	}

	private void AttachPurchasedAccessory()
	{
		ProductInventoryInfo invInfo = Game.StreamingAssetInventory.Get(purchasedInventoryID);
		AvatarAccessory.Create(invInfo, AvatarAccessoryCreateHandler);
	}

	private void AvatarAccessoryCreateHandler(AvatarAccessory avatarAccessory)
	{
		accessoryToBeEquipped = avatarAccessory;
		AvatarAccessory avatarAccessory2 = AvatarBody.GetAccessories(avatarAccessory.AccessorySettings.DefaultSlot).FirstOrDefault();
		if (avatarAccessory2 != null)
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
		UnityEngine.Object.Destroy(accessoryToBeEquipped.gameObject);
		accessoryToBeEquipped = null;
		OnPositiveClose();
		DialogFactory.CloseDialog();
	}

	private void Equip()
	{
		AvatarAccessory avatarAccessory = accessoryToBeEquipped;
		MVNetworkGame game = Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseEquipHandler));
		Game.SetAvatarAccessorySlot(AvatarBody.Id, avatarAccessory.InventoryID, avatarAccessory.AccessorySettings.DefaultSlot, avatarAccessory.AccessorySettings.DefaultOffset);
		AvatarBody.AttachAccessory(avatarAccessory, accessoryToBeEquipped.AccessorySettings.DefaultSlot, accessoryToBeEquipped.AccessorySettings.DefaultOffset);
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
		purchaseResponseData = purchaseResponseData ?? new Dictionary<object, object>();
		purchaseResponseData["oldInventoryID"] = expiredInventoryID;
		OnNegativeClose();
		DialogFactory.CloseDialog();
	}
}
