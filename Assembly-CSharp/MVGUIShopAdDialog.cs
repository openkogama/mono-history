using System;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class MVGUIShopAdDialog : MVGUIAdDialog
{
	public UXText adName;

	public UXText adText;

	public GameObject adViewItemPrefab;

	public Transform adViewItemRoot;

	public UXTextButton goToShopButton;

	private List<int> allowedItemTypes = new List<int>();

	private Transform previewItemsRoot;

	private List<MVItem> possibleShopItems;

	private MVItem adItem;

	private AdViewItem adViewItem;

	private bool _initialized;

	public ShopRepository shopRepository => MVGameControllerBase.Game.ShopRepository;

	public override bool CanShow()
	{
		FindAllowedItemTypes();
		IDictionary<int, MVItem> shopInventory = shopRepository.ShopInventory;
		if (shopInventory.Count == 0)
		{
			Debug.Log("No items to shop in shop ad. Closing Dialog");
			return false;
		}
		possibleShopItems = shopInventory.Values.Where((MVItem mvItem) => allowedItemTypes.Contains(mvItem.itemCategoryID)).ToList();
		if (possibleShopItems.Count == 0)
		{
			Debug.Log("No items to shop in shop ad. Closing Dialog");
			return false;
		}
		return true;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_initialized)
		{
			adItem = possibleShopItems[UnityEngine.Random.Range(0, possibleShopItems.Count)];
			Debug.Log("Showing itemid: " + adItem.itemID);
			adViewItem = InstansiateViewItem(adItem);
			adViewItem.transform.parent = adViewItemRoot;
			adViewItem.transform.localPosition = Vector3.zero;
			adViewItem.transform.localScale = Vector3.one;
			adName.Text = adItem.name;
			adText.Text = TM._("Why not get this great item from the shop?");
			UXTextButton uXTextButton = goToShopButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OpenShopDialog();
			}));
			_initialized = true;
		}
	}

	private void FindAllowedItemTypes()
	{
		MVGUIAggregateInventory shopInventory = MVGameControllerLegacyUI.EditorController.GetShopInventory();
		foreach (MVGUIShopInventoryGroup inventoryGroup in shopInventory.inventoryGroups)
		{
			allowedItemTypes.AddRange(inventoryGroup.allowedCategoriesTypes);
		}
	}

	private void OpenShopDialog()
	{
		UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/BrightProductShopDialog", string.Empty, noButtons: true, stackDialog: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData())
			.Show();
		MVGUIProductShopDialog mVGUIProductShopDialog = (MVGUIProductShopDialog)uXDialogFactory.CurrentDialogBox;
		mVGUIProductShopDialog.SetAllowInsertProductPreview(allowInsert: true);
		mVGUIProductShopDialog.OnInsertProductPreview = () =>
		{
			InsertPreviewItem();
		};
		mVGUIProductShopDialog.SetPrice(adItem.priceGold, adItem.priceSilver);
		mVGUIProductShopDialog.OnTryPurchaseProduct = () =>
		{
			MVGameControllerBase.Game.UnlockClientShopInventoryItem(adItem.itemID);
		};
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("ProductName", new TextData
		{
			text = adItem.name,
			useWordWrap = true
		});
		MVGUIItemShopPreview component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/ItemShopPreview")) as GameObject).GetComponent<MVGUIItemShopPreview>();
		component.BuildItemShopPreview(adItem, 12f, 12f);
		dictionary.Add("ProductPreview", new ProductPreviewData
		{
			productPreview = component.gameObject
		});
		return dictionary;
	}

	private void InsertPreviewItem()
	{
		MVGameControllerLegacyUI.EditorController.EditorWorldObjectCreation.OnAddItemFromInventory(adItem, isPreviewItem: true);
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
			MVGameControllerBase.Game.PlayerRepository.PlayerInventory.Add(adItem.itemID, adItem);
			MVGameControllerBase.Game.PlayerRepository.itemIDToInventorySlotIndex.Add(adItem.itemID, num);
			MVGameControllerBase.Game.PlayerRepository.NotifyRepositoryChange();
			MVGameControllerBase.Game.ShopRepository.RemoveItem(adItem.itemID);
			MVGameControllerBase.Game.ShopRepository.ReorganizeItemsByItemType(notifyOfChange: true);
		}
		DialogFactory.CloseDialog();
	}

	private AdViewItem InstansiateViewItem(MVItem mvItem)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(adViewItemPrefab);
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = transform;
		gameObject.transform.localScale = Vector3.one;
		previewItemsRoot = new GameObject("Preview Root - " + gameObject.name).transform;
		AdViewItem component = gameObject.GetComponent<AdViewItem>();
		component.BuildViewItem(mvItem, previewItemsRoot);
		return component;
	}

	public void OnDestroy()
	{
		if (previewItemsRoot != null)
		{
			UnityEngine.Object.Destroy(previewItemsRoot.gameObject);
		}
	}
}
