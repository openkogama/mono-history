using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Localize;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessoryShopGroup : MVGUIAvatarAccessoryBaseGroup
{
	public delegate void OnItemPreviewDelegate(AvatarAccessoryShopViewItem viewItem);

	public OnItemPreviewDelegate OnItemPreview;

	private StreamingAssetInfo streamingAssetInfo;

	private int purchasedInventoryID = -1;

	private AvatarAccessory accessoryToBeEquipped;

	private CharacterEditorController CEController => MVGameController.Instance.CharacterEditorController;

	private MVBody AvatarBody => CEController.CurrentBody;

	public AvatarAccessoryShopCollection AvatarAccessoryShopCollection { get; protected set; }

	protected override void InitializeCollectionView()
	{
		Debug.Log((object)("Streaming assets in shop: " + Game.StreamingAssetShopInventory.Count));
		AvatarAccessoryShopCollection = new AvatarAccessoryShopCollection(Game.StreamingAssetShopInventory);
		collectionView.Initialize();
		collectionView.InstansiateViewItem = InstansiateViewItem;
		UXCollectionView uXCollectionView = collectionView;
		uXCollectionView.OnItemSelection = (UXCollectionView.OnBasicItemEventDelegate)Delegate.Combine(uXCollectionView.OnItemSelection, new UXCollectionView.OnBasicItemEventDelegate(OnItemSelection));
		collectionView.Collection = AvatarAccessoryShopCollection;
		collectionView.SetVisible(Group.Visible);
	}

	public override void ResetInventoryGroup()
	{
		AvatarAccessoryShopCollection = null;
		base.ResetInventoryGroup();
	}

	public override void InitializeAfterReset()
	{
		base.InitializeAfterReset();
		AvatarAccessoryShopCollection = new AvatarAccessoryShopCollection(Game.StreamingAssetShopInventory);
		collectionView.Collection = AvatarAccessoryShopCollection;
	}

	private void OnItemSelection(IUXCollectionItem item)
	{
		if (OnItemPreview != null)
		{
			OnItemPreview(null);
		}
		OpenShopDialog(item);
		((Component)MVGameController.Instance.CharacterEditorController.AvatarAccessoryShop).gameObject.SetActiveRecursively(false);
	}

	private void OpenShopDialog(IUXCollectionItem item)
	{
		streamingAssetInfo = (StreamingAssetInfo)item.Object;
		UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/AvatarAccessoryShopDialog", TextSlotIndex.Empty, noButtons: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData())
			.Show();
		MVGUIAvatarAccessoryShopDialog mVGUIAvatarAccessoryShopDialog = (MVGUIAvatarAccessoryShopDialog)uXDialogFactory.CurrentDialogBox;
		mVGUIAvatarAccessoryShopDialog.BuildShopDialog(item.Object as StreamingAssetInfo);
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("AccessoryName", new TextData
		{
			text = streamingAssetInfo.Name,
			useWordWrap = true
		});
		dictionary.Add("AccessoryDescription", new TextData
		{
			text = streamingAssetInfo.Desc,
			useWordWrap = true
		});
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/AvatarAccessoryShopPreview"));
		MVGUIAvatarAccessoryShopPreview component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIAvatarAccessoryShopPreview>();
		component.CreateNewViewItem(streamingAssetInfo);
		dictionary.Add("AccessoryPreview", new ProductPreviewData
		{
			productPreview = ((Component)component).gameObject
		});
		return dictionary;
	}

	private void OnPurchaseDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult != UXDialogResult.Positive)
		{
			((Component)MVGameController.Instance.CharacterEditorController.AvatarAccessoryShop).gameObject.SetActiveRecursively(true);
		}
		((Component)MVGameController.Instance.CharacterEditorController.AvatarAccessoryShop).gameObject.SetActiveRecursively(true);
	}

	private void HandleRent(int invID, DateTime purchaseTime, Hashtable purchaseResponse)
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

	private void OnAttachOfferDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			ProductInventoryInfo<StreamingAssetInfo> invInfo = Game.StreamingAssetInventory.Get(purchasedInventoryID);
			AvatarAccessory.Create(invInfo, AvatarAccessoryCreateHandler);
		}
		else
		{
			MVGameController.Instance.CharacterEditorController.AvatarAccessoryInventory.ShowInventoryUpdated();
			((Component)MVGameController.Instance.CharacterEditorController.AvatarAccessoryShop).gameObject.SetActiveRecursively(true);
		}
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
		((Component)CEController.AvatarAccessoryShop).gameObject.SetActiveRecursively(true);
	}

	private void Equip()
	{
		AvatarAccessory avatarAccessory = accessoryToBeEquipped;
		MVNetworkGame game = Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseEquipHandler));
		Game.SetAvatarAccessorySlot(AvatarBody.Id, avatarAccessory.InventoryID, avatarAccessory.DefaultSlot, avatarAccessory.DefaultOffset);
		AvatarBody.AttachAccessory(avatarAccessory, accessoryToBeEquipped.DefaultSlot, accessoryToBeEquipped.DefaultOffset);
		((Component)CEController.AvatarAccessoryShop).gameObject.SetActiveRecursively(true);
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
	}

	private void PreviewAvatarItem(AvatarAccessoryShopViewItem viewItem)
	{
		if (OnItemPreview != null)
		{
			OnItemPreview(viewItem);
		}
	}

	protected override UXCollectionViewItem InstansiateViewItem(IUXCollectionItem item)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)inventoryViewItemPrefab);
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.layer = LayerMask.NameToLayer("UXElement");
		val2.transform.parent = ((Component)this).transform;
		val2.transform.localScale = Vector3.one;
		AvatarAccessoryShopViewItem component = val2.GetComponent<AvatarAccessoryShopViewItem>();
		component.Item = item;
		component.PreviewItemsRoot = previewItemsRoot;
		component.OnPreviewAvatarAccessory = (AvatarAccessoryShopViewItem.OnPreviewAvatarAccessoryDelegate)Delegate.Combine(component.OnPreviewAvatarAccessory, new AvatarAccessoryShopViewItem.OnPreviewAvatarAccessoryDelegate(PreviewAvatarItem));
		return component;
	}
}
