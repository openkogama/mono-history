using System;
using System.Collections.Generic;
using Localize;
using MV.WorldObject;
using UnityEngine;

public class MVGUIAvatarShopInventory : MVGUIInventoryGroup
{
	private MVItem _purchaseItem;

	protected override void InitializeCollectionView()
	{
		repositoryCollection = new ShopRepositoryCollection(MVGameController.Instance.Game.AvatarShopRepository, new int[0]);
		collectionView.InstansiateViewItem = InstansiateViewItem;
		UXCollectionView uXCollectionView = collectionView;
		uXCollectionView.OnItemSelection = (UXCollectionView.OnBasicItemEventDelegate)Delegate.Combine(uXCollectionView.OnItemSelection, new UXCollectionView.OnBasicItemEventDelegate(OnItemSelection));
		collectionView.Collection = repositoryCollection;
		collectionView.Initialize();
		collectionView.SetVisible(((Component)this).gameObject.GetComponent<UXViewScript>().View.isVisible);
	}

	public override void InitializeAfterReset()
	{
		CreatePreviewItemRoot();
		repositoryCollection = new ShopRepositoryCollection(MVGameController.Instance.Game.AvatarShopRepository, new int[0]);
		collectionView.Collection = repositoryCollection;
	}

	private void OnItemSelection(IUXCollectionItem item)
	{
		ShowAvatarPurchaseDialog(item);
	}

	private void ShowAvatarPurchaseDialog(IUXCollectionItem collectionItem)
	{
		_purchaseItem = (MVItem)collectionItem.Object;
		UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/BrightProductShopDialog", TextSlotIndex.Empty, noButtons: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData(_purchaseItem, collectionItem.Index))
			.Show();
		MVGUIProductShopDialog mVGUIProductShopDialog = (MVGUIProductShopDialog)uXDialogFactory.CurrentDialogBox;
		mVGUIProductShopDialog.SetPrice(_purchaseItem.priceGold, _purchaseItem.priceSilver);
		mVGUIProductShopDialog.OnTryPurchaseProduct = () =>
		{
			MVGameController.Instance.Game.PurchaseAvatar(_purchaseItem.itemID);
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
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/AvatarShopPreview"));
		MVGUIAvatarShopPreview component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIAvatarShopPreview>();
		component.SetAvatarViewItem(avatarViewItem);
		dictionary.Add("ProductPreview", new ProductPreviewData
		{
			productPreview = ((Component)component).gameObject
		});
		return dictionary;
	}

	private void OnPurchaseDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			MVGameController.Instance.CharacterEditorController.AvatarShop.View.Hide();
		}
		_purchaseItem = null;
	}
}
