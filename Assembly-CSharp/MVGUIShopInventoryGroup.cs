using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVGUIShopInventoryGroup : MVGUIInventoryGroup
{
	private MVItem _purchaseItem;

	protected override void InitializeCollectionView()
	{
		repositoryCollection = new ShopRepositoryCollection(MVGameController.Game.ShopRepository, allowedCategoriesTypes);
		collectionView.Initialize();
		collectionView.InstansiateViewItem = InstansiateViewItem;
		UXCollectionView uXCollectionView = collectionView;
		uXCollectionView.OnItemSelection = (UXCollectionView.OnBasicItemEventDelegate)Delegate.Combine(uXCollectionView.OnItemSelection, new UXCollectionView.OnBasicItemEventDelegate(OnItemSelection));
		collectionView.Collection = repositoryCollection;
		collectionView.SetVisible(Group.Visible);
	}

	public override void InitializeAfterReset()
	{
		CreatePreviewItemRoot();
		repositoryCollection = new ShopRepositoryCollection(MVGameController.Game.ShopRepository, allowedCategoriesTypes);
		collectionView.Collection = repositoryCollection;
	}

	private void OnItemSelection(IUXCollectionItem collectionItem)
	{
		_purchaseItem = (MVItem)collectionItem.Object;
		UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/BrightProductShopDialog", string.Empty, noButtons: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData(_purchaseItem, collectionItem.Index))
			.Show();
		collectionView.gameObject.SetActive(value: false);
		MVGUIProductShopDialog mVGUIProductShopDialog = (MVGUIProductShopDialog)uXDialogFactory.CurrentDialogBox;
		mVGUIProductShopDialog.SetAllowInsertProductPreview(allowInsert: true);
		mVGUIProductShopDialog.OnInsertProductPreview = () =>
		{
			InsertPreviewItem(_purchaseItem);
		};
		mVGUIProductShopDialog.SetPrice(_purchaseItem.priceGold, _purchaseItem.priceSilver);
		mVGUIProductShopDialog.OnTryPurchaseProduct = () =>
		{
			MVGameController.Game.UnlockClientShopInventoryItem(_purchaseItem.itemID);
		};
	}

	private Dictionary<string, DialogData> BuildDialogData(MVItem item, int slotIndex)
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("ProductName", new TextData
		{
			text = item.name,
			useWordWrap = true
		});
		dictionary.Add("ProductDescription", new TextData
		{
			text = item.description.Replace("\\n", "\n"),
			useWordWrap = true
		});
		MVGUIItemShopPreview component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/ItemShopPreview")) as GameObject).GetComponent<MVGUIItemShopPreview>();
		component.BuildItemShopPreview(item, 12f, 12f);
		dictionary.Add("ProductPreview", new ProductPreviewData
		{
			productPreview = component.gameObject
		});
		return dictionary;
	}

	private void InsertPreviewItem(MVItem item)
	{
		MVGameController.EditorController.EditorWorldObjectCreation.OnAddItemFromInventory(item, isPreviewItem: true);
		if (NotifyItemSelection != null)
		{
			NotifyItemSelection();
		}
	}

	private void OnPurchaseDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)dialogBox.GetResult();
			if (!dictionary.ContainsKey((byte)22))
			{
				Debug.LogError("Purchased product, but received no slot index to put it into");
				return;
			}
			int num = (int)dictionary[(byte)22];
			MVGameController.Game.PlayerRepository.PlayerInventory.Add(_purchaseItem.itemID, _purchaseItem);
			MVGameController.Game.PlayerRepository.itemIDToInventorySlotIndex.Add(_purchaseItem.itemID, num);
			MVGameController.Game.ItemBusinessLogic.AddItem(_purchaseItem);
			MVGameController.Game.PlayerRepository.NotifyRepositoryChange();
			MVGameController.Game.ShopRepository.RemoveItem(_purchaseItem.itemID);
			MVGameController.Game.ShopRepository.ReorganizeItemsByItemType(notifyOfChange: true);
		}
		collectionView.gameObject.SetActive(value: true);
	}
}
