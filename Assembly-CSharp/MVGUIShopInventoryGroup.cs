using System;
using System.Collections;
using System.Collections.Generic;
using Localize;
using MV.WorldObject;
using UnityEngine;

public class MVGUIShopInventoryGroup : MVGUIInventoryGroup
{
	private MVItem _purchaseItem;

	protected override void InitializeCollectionView()
	{
		repositoryCollection = new ShopRepositoryCollection(MVGameController.Instance.Game.ShopRepository, allowedCategoriesTypes);
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
		repositoryCollection = new ShopRepositoryCollection(MVGameController.Instance.Game.ShopRepository, allowedCategoriesTypes);
		collectionView.Collection = repositoryCollection;
	}

	private void OnItemSelection(IUXCollectionItem collectionItem)
	{
		_purchaseItem = (MVItem)collectionItem.Object;
		UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/BrightProductShopDialog", TextSlotIndex.Empty, noButtons: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData(_purchaseItem, collectionItem.Index))
			.Show();
		((Component)collectionView).gameObject.SetActiveRecursively(false);
		MVGUIProductShopDialog mVGUIProductShopDialog = (MVGUIProductShopDialog)uXDialogFactory.CurrentDialogBox;
		mVGUIProductShopDialog.SetAllowInsertProductPreview(allowInsert: true);
		mVGUIProductShopDialog.OnInsertProductPreview = () =>
		{
			InsertPreviewItem(_purchaseItem);
		};
		mVGUIProductShopDialog.SetPrice(_purchaseItem.priceGold, _purchaseItem.priceSilver);
		mVGUIProductShopDialog.OnTryPurchaseProduct = () =>
		{
			MVGameController.Instance.Game.UnlockClientShopInventoryItem(_purchaseItem.itemID);
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
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/ItemShopPreview"));
		MVGUIItemShopPreview component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIItemShopPreview>();
		component.BuildItemShopPreview(item, 12f, 12f);
		dictionary.Add("ProductPreview", new ProductPreviewData
		{
			productPreview = ((Component)component).gameObject
		});
		return dictionary;
	}

	private void InsertPreviewItem(MVItem item)
	{
		MVGameController.Instance.EditController.EditorWorldObjectCreation.OnAddItemFromInventory(item, isPreviewItem: true);
		if (NotifyItemSelection != null)
		{
			NotifyItemSelection();
		}
	}

	private void OnPurchaseDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Hashtable hashtable = (Hashtable)dialogBox.GetResult();
			if (!hashtable.ContainsKey((byte)22))
			{
				Debug.LogError((object)"Purchased product, but received no slot index to put it into");
				return;
			}
			int num = (int)hashtable[(byte)22];
			MVGameController.Instance.Game.PlayerRepository.PlayerInventory.Add(_purchaseItem.itemID, _purchaseItem);
			MVGameController.Instance.Game.PlayerRepository.itemIDToInventorySlotIndex.Add(_purchaseItem.itemID, num);
			MVGameController.Instance.Game.ItemBusinessLogic.AddItem(_purchaseItem);
			MVGameController.Instance.Game.PlayerRepository.NotifyRepositoryChange();
			MVGameController.Instance.Game.ShopRepository.RemoveItem(_purchaseItem.itemID);
			MVGameController.Instance.Game.ShopRepository.ReorganizeItemsByItemType(notifyOfChange: true);
		}
		((Component)collectionView).gameObject.SetActiveRecursively(true);
	}
}
