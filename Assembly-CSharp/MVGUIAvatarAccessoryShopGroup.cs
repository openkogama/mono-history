using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessoryShopGroup : MVGUIAvatarAccessoryBaseGroup
{
	public delegate void OnItemPreviewDelegate(AvatarAccessoryShopViewItem viewItem);

	public OnItemPreviewDelegate OnItemPreview;

	private StreamingAssetInfo streamingAssetInfo;

	private int purchasedInventoryID = -1;

	private AvatarAccessory accessoryToBeEquipped;

	private AvatarAccessoryController avatarAccessoryController;

	private CharacterEditorController CEController => MVGameController.CharacterEditorController;

	private MVBody AvatarBody
	{
		get
		{
			if (MVGameController.GameMode == MVGameMode.CharacterEditor)
			{
				return CEController.CurrentBody;
			}
			return MVGameController.WOCM.AvatarLocal.Body;
		}
	}

	public AvatarAccessoryShopCollection AvatarAccessoryShopCollection { get; protected set; }

	public override void Initialize(AvatarAccessoryController avatarAccessoryController)
	{
		base.Initialize(avatarAccessoryController);
		this.avatarAccessoryController = avatarAccessoryController;
	}

	protected override void InitializeCollectionView()
	{
		Debug.Log("Streaming assets in shop: " + Game.StreamingAssetShopInventory.Count);
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
		avatarAccessoryController.AvatarAccessoryShop.gameObject.SetActive(value: false);
	}

	private void OpenShopDialog(IUXCollectionItem item)
	{
		streamingAssetInfo = (StreamingAssetInfo)item.Object;
		UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/AvatarAccessoryShopDialog", string.Empty, noButtons: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData())
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
		MVGUIAvatarAccessoryShopPreview component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/AvatarAccessoryShopPreview")) as GameObject).GetComponent<MVGUIAvatarAccessoryShopPreview>();
		component.CreateNewViewItem(streamingAssetInfo);
		dictionary.Add("AccessoryPreview", new ProductPreviewData
		{
			productPreview = component.gameObject
		});
		return dictionary;
	}

	private void OnPurchaseDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult != UXDialogResult.Positive)
		{
			avatarAccessoryController.AvatarAccessoryShop.gameObject.SetActive(value: true);
		}
		avatarAccessoryController.AvatarAccessoryShop.gameObject.SetActive(value: true);
	}

	private void HandleRent(int invID, DateTime purchaseTime, Dictionary<object, object> purchaseResponse)
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

	private void AddToInventory(int invID, DateTime purchaseTime, Dictionary<object, object> purchaseResponse, bool isRenting)
	{
		int productID = streamingAssetInfo.ProductID;
		StreamingAssetInfo value = null;
		Game.StreamingAssetInfoMap.TryGetValue(productID, out value);
		if (value != null)
		{
			ProductInventoryInfo invInfo = new ProductInventoryInfo(invID, value, purchaseTime, isRenting);
			Game.StreamingAssetInventory.Add(invInfo);
			Game.StreamingAssetInventory.NotifyProductInventoryChange();
		}
		else
		{
			Debug.LogError("Trying to add non-existing avatar accessory to inventory");
		}
	}

	private void OnAttachOfferDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			ProductInventoryInfo invInfo = Game.StreamingAssetInventory.Get(purchasedInventoryID);
			AvatarAccessory.Create(invInfo, AvatarAccessoryCreateHandler);
		}
		else
		{
			avatarAccessoryController.AvatarAccessoryInventory.ShowInventoryUpdated();
			avatarAccessoryController.AvatarAccessoryShop.gameObject.SetActive(value: true);
		}
	}

	private void AvatarAccessoryCreateHandler(AvatarAccessory avatarAccessory)
	{
		accessoryToBeEquipped = avatarAccessory;
		AvatarAccessory avatarAccessory2 = AvatarBody.GetAccessories(avatarAccessory.DefaultSlot).FirstOrDefault();
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
		avatarAccessoryController.AvatarAccessoryShop.gameObject.SetActive(value: true);
	}

	private void Equip()
	{
		AvatarAccessory avatarAccessory = accessoryToBeEquipped;
		MVNetworkGame game = Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseEquipHandler));
		Game.SetAvatarAccessorySlot(AvatarBody.Id, avatarAccessory.InventoryID, avatarAccessory.DefaultSlot, avatarAccessory.DefaultOffset);
		AvatarBody.AttachAccessory(avatarAccessory, accessoryToBeEquipped.DefaultSlot, accessoryToBeEquipped.DefaultOffset);
		avatarAccessoryController.AvatarAccessoryShop.gameObject.SetActive(value: true);
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
		GameObject gameObject = UnityEngine.Object.Instantiate(inventoryViewItemPrefab);
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = transform;
		gameObject.transform.localScale = Vector3.one;
		AvatarAccessoryShopViewItem component = gameObject.GetComponent<AvatarAccessoryShopViewItem>();
		component.Item = item;
		component.PreviewItemsRoot = previewItemsRoot;
		component.OnPreviewAvatarAccessory = (AvatarAccessoryShopViewItem.OnPreviewAvatarAccessoryDelegate)Delegate.Combine(component.OnPreviewAvatarAccessory, new AvatarAccessoryShopViewItem.OnPreviewAvatarAccessoryDelegate(PreviewAvatarItem));
		return component;
	}
}
