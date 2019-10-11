using System;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemPreviewSell : ManageItemPage
{
	[SerializeField]
	private InputField itemName;

	[SerializeField]
	private InputField description;

	[SerializeField]
	private RawImage previewImage;

	[SerializeField]
	private Button removeFromMarketButton;

	[SerializeField]
	private Text sellButtonText;

	[SerializeField]
	private Button sellButton;

	[SerializeField]
	private ProgressBarAndroid compareSlider;

	[SerializeField]
	private RectTransform thresholdCaret;

	[SerializeField]
	private RectTransform sliderTransform;

	[SerializeField]
	private Text compareText;

	private InventoryItem previewItem;

	private bool addingToMarket;

	public override void Initialize(RawImage preview, InventoryItem item)
	{
		previewImage.texture = preview.mainTexture;
		previewItem = item;
		itemName.text = item.name;
		description.text = string.Empty;
		if (!string.IsNullOrEmpty(item.description))
		{
			description.text = item.description;
		}
		sellButton.gameObject.SetActive(value: false);
		removeFromMarketButton.gameObject.SetActive(value: false);
		if (item.shopInventoryID != 0 && item.authorProfileID == MVGameControllerBase.Game.LocalPlayer.ProfileID)
		{
			sellButtonText.text = TM._("Update");
			removeFromMarketButton.gameObject.SetActive(value: true);
			sellButton.gameObject.SetActive(value: true);
			addingToMarket = false;
		}
		else if (item.authorProfileID == MVGameControllerBase.Game.LocalPlayer.ProfileID)
		{
			sellButtonText.text = TM._("Sell");
			sellButton.gameObject.SetActive(value: true);
			addingToMarket = true;
		}
		else
		{
			MVGameControllerBase.Game.ReceivedItemFromQuery += OnLoadMarketPlaceItem;
			MVGameControllerBase.OperationRequests.RequestMarketPlaceItem(item.itemID);
		}
	}

	public void AddToMarket()
	{
		if (MVGameControllerBase.Game.LocalPlayer.Level < MVGameControllerBase.Game.MarketPlaceLevel)
		{
			string txt = TM._("You can not add item to your shop before reaching level: ") + MVGameControllerBase.Game.MarketPlaceLevel;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(txt, "Error: ");
			});
		}
		else if (string.IsNullOrEmpty(itemName.text) || string.IsNullOrEmpty(description.text))
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Item name and description required to sell item."), TM._("Error: "));
			});
		}
		else
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
			MVGameControllerBase.OperationRequests.RequestAddItemToMarketPlace(previewItem.itemID, itemName.text, description.text);
		}
	}

	public void RemoveFromMarket()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnRemoveFromMarketplace));
		MVGameControllerBase.OperationRequests.RequestRemoveItemFromMarketPlace(previewItem.itemID);
	}

	private void OnRemoveFromMarketplace(bool success)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Remove(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnRemoveFromMarketplace));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		string txt = string.Format(TM._("Failed to remove {0} from your shop"), itemName.text);
		if (success)
		{
			txt = string.Format(TM._("Successfully removed {0} from your shop"), itemName.text);
			MVGameControllerBase.EditModeUI.PlayerInventoryRepository.UpdateShopInventoryID(previewItem.itemID, 0);
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(txt, string.Empty);
		});
	}

	private void OnAddToMarketplaceReturn(bool success)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Remove(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
		previewItem.name = itemName.text;
		previewItem.description = description.text;
		if (success)
		{
			string txt = ((!addingToMarket) ? string.Format(TM._("Successfully updated {0} in your shop."), previewItem.name) : string.Format(TM._("Successfully added {0} to your shop."), previewItem.name));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(txt, string.Empty);
			});
		}
		else
		{
			string txt2 = ((!addingToMarket) ? string.Format(TM._("Failed to update {0} in your shop."), previewItem.name) : string.Format(TM._("Failed to add {0} to your shop."), previewItem.name));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(txt2, string.Empty);
			});
		}
	}

	private void OnLoadMarketPlaceItem(object sender, ReceivedItemFromQueryEventArgs e)
	{
		MVGameControllerBase.Game.ReceivedItemFromQuery -= OnLoadMarketPlaceItem;
		BytePacker koGaMaData = e.KoGaMaData;
		KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(new BytePacker(previewItem.data), readRuntimeValues: false);
		koGaMaPackageClient.InventoryInitialize();
		KoGaMaPackageClient koGaMaPackageClient2 = new KoGaMaPackageClient(koGaMaData, readRuntimeValues: false);
		koGaMaPackageClient2.InventoryInitialize();
		float num = KoGaMaPackageClient.Compare(koGaMaPackageClient2, koGaMaPackageClient);
		koGaMaPackageClient.Destroy();
		koGaMaPackageClient2.Destroy();
		compareSlider.Progress = 1f - num;
		float num2 = 1f - CommonValues.CompareThreshold;
		thresholdCaret.anchoredPosition = new Vector2(num2 * sliderTransform.rect.width, 0f);
		compareSlider.gameObject.SetActive(value: true);
		if (1f - num <= 1f - CommonValues.CompareThreshold)
		{
			compareText.text = TM._("Item is not different enough from original item.");
			return;
		}
		compareText.text = TM._("Item is sellable.");
		sellButton.gameObject.SetActive(value: true);
	}
}
