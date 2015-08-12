using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVGUIAvatarShopInventory : MVGUIInventoryGroup
{
	private MVItem _purchaseItem;

	protected override void InitializeCollectionView()
	{
		repositoryCollection = new ShopRepositoryCollection(MVGameController.Game.AvatarShopRepository, new int[0]);
		collectionView.InstansiateViewItem = InstansiateViewItem;
		UXCollectionView uXCollectionView = collectionView;
		uXCollectionView.OnItemSelection = (UXCollectionView.OnBasicItemEventDelegate)Delegate.Combine(uXCollectionView.OnItemSelection, new UXCollectionView.OnBasicItemEventDelegate(OnItemSelection));
		collectionView.Collection = repositoryCollection;
		collectionView.Initialize();
		collectionView.SetVisible(gameObject.GetComponent<UXViewScript>().View.isVisible);
	}

	public override void InitializeAfterReset()
	{
		CreatePreviewItemRoot();
		repositoryCollection = new ShopRepositoryCollection(MVGameController.Game.AvatarShopRepository, new int[0]);
		collectionView.Collection = repositoryCollection;
	}

	private void OnItemSelection(IUXCollectionItem item)
	{
		ShowAvatarPurchaseDialog(item);
	}

	private void ShowAvatarPurchaseDialog(IUXCollectionItem collectionItem)
	{
		_purchaseItem = (MVItem)collectionItem.Object;
		UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/BrightProductShopDialog", string.Empty, noButtons: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData(_purchaseItem, collectionItem.Index))
			.Show();
		MVGUIProductShopDialog mVGUIProductShopDialog = (MVGUIProductShopDialog)uXDialogFactory.CurrentDialogBox;
		mVGUIProductShopDialog.SetPrice(_purchaseItem.priceGold, _purchaseItem.priceSilver);
		mVGUIProductShopDialog.OnTryPurchaseProduct = () =>
		{
			MVGameController.Game.PurchaseAvatar(_purchaseItem.itemID);
		};
	}

	private Dictionary<string, DialogData> BuildDialogData(MVItem item, int slotIndex)
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("ProductName", new TextData
		{
			text = "New Avatar"
		});
		AvatarViewItem avatarViewItem = (AvatarViewItem)collectionView.GetCollectionViewItemFromIndex(slotIndex);
		MVGUIAvatarShopPreview component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/AvatarShopPreview")) as GameObject).GetComponent<MVGUIAvatarShopPreview>();
		component.SetAvatarViewItem(avatarViewItem);
		dictionary.Add("ProductPreview", new ProductPreviewData
		{
			productPreview = component.gameObject
		});
		return dictionary;
	}

	private void OnPurchaseDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			MVGameController.CharacterEditorController.AvatarShop.View.Hide();
		}
		_purchaseItem = null;
	}
}
