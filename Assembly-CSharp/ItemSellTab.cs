using System;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSellTab : ManageItemPage
{
	[SerializeField]
	private InputField itemName;

	[SerializeField]
	private InputField description;

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
		previewItem = item;
		itemName.text = item.name;
		description.text = string.Empty;
		if (!string.IsNullOrEmpty(item.description))
		{
			description.text = item.description;
		}
		if (item.shopInventoryID != 0 && item.authorProfileID == MVGameControllerBase.Game.LocalPlayer.ProfileID)
		{
			sellButtonText.text = "Update";
			removeFromMarketButton.gameObject.SetActive(value: true);
			sellButton.gameObject.SetActive(value: true);
			addingToMarket = false;
		}
		else if (item.authorProfileID == MVGameControllerBase.Game.LocalPlayer.ProfileID)
		{
			sellButtonText.text = "Sell";
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
			string txt = TM._("You can not add to marketplace before reaching level: ") + MVGameControllerBase.Game.MarketPlaceLevel;
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
			MVGameControllerBase.OperationRequests.RequestAddItemToMarketPlace(previewItem.itemID, itemName.text, description.text, 0);
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
		string txt = "Failed to remove";
		if (success)
		{
			txt = "Successfully removed ";
			MVGameControllerBase.IEditModeUI.PlayerInventoryRepository.UpdateShopInventoryID(previewItem.itemID, 0);
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(txt + itemName.text + " from marketplace.", string.Empty);
		});
	}

	private void OnAddToMarketplaceReturn(bool success)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Remove(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (success)
		{
			previewItem.name = itemName.text;
			previewItem.description = description.text;
			string txt = ((!addingToMarket) ? "Successfully updated " : "Successfully placed ");
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(txt + previewItem.name + " on marketplace.", string.Empty);
			});
		}
		else
		{
			string txt2 = ((!addingToMarket) ? "Failed to update " : "Failed to place ");
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(txt2 + itemName.text + " on marketplace.", string.Empty);
			});
		}
	}

	private void OnLoadMarketPlaceItem(object sender, ReceivedItemFromQueryEventArgs e)
	{
		MVGameControllerBase.Game.ReceivedItemFromQuery -= OnLoadMarketPlaceItem;
		BytePacker koGaMaData = e.KoGaMaData;
		Debug.Log("new item " + previewItem.itemID);
		KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(new BytePacker(previewItem.data), readRuntimeValues: false);
		KoGaMaPackageClient koGaMaPackageClient2 = new KoGaMaPackageClient(koGaMaData, readRuntimeValues: false);
		float num = KoGaMaPackageClient.Compare(koGaMaPackageClient2, koGaMaPackageClient);
		koGaMaPackageClient.Destroy();
		koGaMaPackageClient2.Destroy();
		compareSlider.Progress = 1f - num;
		float num2 = 1f - CommonValues.CompareThreshold;
		thresholdCaret.anchoredPosition = new Vector2(num2 * sliderTransform.rect.width, 0f);
		compareSlider.gameObject.SetActive(value: true);
		if (1f - num <= 1f - CommonValues.CompareThreshold)
		{
			compareText.text = TM._("Item is not different enough from the original.");
			return;
		}
		compareText.text = TM._("Item is different enough to be sold.");
		sellButton.gameObject.SetActive(value: true);
	}
}
