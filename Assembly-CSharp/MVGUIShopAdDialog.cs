using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Localize;
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

	public ShopRepository shopRepository => MVGameController.Instance.Game.ShopRepository;

	private AEditController EditController => MVGameController.Instance.EditController;

	public override bool CanShow()
	{
		FindAllowedItemTypes();
		IDictionary<int, MVItem> shopInventory = shopRepository.ShopInventory;
		if (shopInventory.Count == 0)
		{
			Debug.Log((object)"No items to shop in shop ad. Closing Dialog");
			return false;
		}
		possibleShopItems = shopInventory.Values.Where((MVItem mvItem) => allowedItemTypes.Contains(mvItem.itemCategoryID)).ToList();
		if (possibleShopItems.Count == 0)
		{
			Debug.Log((object)"No items to shop in shop ad. Closing Dialog");
			return false;
		}
		return true;
	}

	public override void OnShowDialog()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		base.OnShowDialog();
		if (!_initialized)
		{
			adItem = possibleShopItems[Random.Range(0, possibleShopItems.Count)];
			Debug.Log((object)("Showing itemid: " + adItem.itemID));
			adViewItem = InstansiateViewItem(adItem);
			((Component)adViewItem).transform.parent = adViewItemRoot;
			((Component)adViewItem).transform.localPosition = Vector3.zero;
			((Component)adViewItem).transform.localScale = Vector3.one;
			adName.Text = adItem.name;
			adText.Text = Localization.Instance.GetText(TextSlotIndex.ShopAdText);
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
		MVGUIAggregateInventory shopInventory = MVGameController.Instance.EditController.GetShopInventory();
		foreach (MVGUIShopInventoryGroup inventoryGroup in shopInventory.inventoryGroups)
		{
			allowedItemTypes.AddRange(inventoryGroup.allowedCategoriesTypes);
		}
	}

	private void OpenShopDialog()
	{
		UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/BrightProductShopDialog", TextSlotIndex.Empty, noButtons: true, stackDialog: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData())
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
			MVGameController.Instance.Game.UnlockClientShopInventoryItem(adItem.itemID);
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
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/ItemShopPreview"));
		MVGUIItemShopPreview component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIItemShopPreview>();
		component.BuildItemShopPreview(adItem, 12f, 12f);
		dictionary.Add("ProductPreview", new ProductPreviewData
		{
			productPreview = ((Component)component).gameObject
		});
		return dictionary;
	}

	private void InsertPreviewItem()
	{
		if (EditController.PlayInEditor)
		{
			EditController.TogglePlayInEditor();
		}
		EditController.EditorWorldObjectCreation.OnAddItemFromInventory(adItem, isPreviewItem: true);
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
			MVGameController.Instance.Game.PlayerRepository.PlayerInventory.Add(adItem.itemID, adItem);
			MVGameController.Instance.Game.PlayerRepository.itemIDToInventorySlotIndex.Add(adItem.itemID, num);
			MVGameController.Instance.Game.PlayerRepository.NotifyRepositoryChange();
			MVGameController.Instance.Game.ShopRepository.RemoveItem(adItem.itemID);
			MVGameController.Instance.Game.ShopRepository.ReorganizeItemsByItemType(notifyOfChange: true);
		}
		DialogFactory.CloseDialog();
	}

	private AdViewItem InstansiateViewItem(MVItem mvItem)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)adViewItemPrefab);
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.layer = LayerMask.NameToLayer("UXElement");
		val2.transform.parent = ((Component)this).transform;
		val2.transform.localScale = Vector3.one;
		previewItemsRoot = new GameObject("Preview Root - " + ((Object)val2).name).transform;
		AdViewItem component = val2.GetComponent<AdViewItem>();
		component.BuildViewItem(mvItem, previewItemsRoot);
		return component;
	}

	public void OnDestroy()
	{
		if ((Object)(object)previewItemsRoot != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)previewItemsRoot).gameObject);
		}
	}
}
