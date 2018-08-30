using System;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellAvatarController : MonoBehaviour
{
	[SerializeField]
	private InputField nameField;

	[SerializeField]
	private Button removeButton;

	[SerializeField]
	private Text sellButtonText;

	[SerializeField]
	private AvatarScreenShooter screenShooter;

	private int woID = -1;

	private MvAvatarMetaData metaData;

	private MVBody body;

	public void Initialize(int woID, MVBody currentBody)
	{
		MVGameControllerBase.Game.AvatarMetaDataWoMap.TryGetValue(woID, out var avatarMetaData);
		body = currentBody;
		metaData = avatarMetaData;
		this.woID = woID;
		sellButtonText.text = ((!avatarMetaData.isOnMarketPlace) ? TM._("Sell") : TM._("Update"));
		removeButton.gameObject.SetActive(avatarMetaData.isOnMarketPlace);
		nameField.text = avatarMetaData.name;
	}

	public void OnSellPressed()
	{
		if (IsSelectedBodyValid() && !string.IsNullOrEmpty(nameField.text))
		{
			screenShooter.TakeScreenShot(ScreenShotCallback, body, ignoreAccessories: true);
		}
	}

	private void ScreenShotCallback(Texture2D texture, string successMessage)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplace));
		DataUploadManager.UploadData(texture.EncodeToPNG(), OnImageUploaded);
	}

	private void OnImageUploaded()
	{
		MVGameControllerBase.OperationRequests.AddAvatarToAvatarShopInventory(woID, nameField.text);
	}

	private void OnAddToMarketplace(bool added)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Remove(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplace));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		string text;
		if (added)
		{
			text = ((!metaData.isOnMarketPlace) ? TM._("Avatar is now available in your shop.") : TM._("Avatar updated in your shop."));
			removeButton.gameObject.SetActive(value: true);
			sellButtonText.text = TM._("Update");
		}
		else if (MVGameControllerBase.Game.LocalPlayer.Level >= MVGameControllerBase.Game.MarketPlaceLevel)
		{
			text = ((!metaData.isOnMarketPlace) ? TM._("Failed to put Avatar in your shop.") : TM._("Failed to update Avatar."));
		}
		else
		{
			text = string.Format(TM._("You need to be level {0} to place avatar in shop"), MVGameControllerBase.Game.MarketPlaceLevel);
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(text, string.Empty);
		});
	}

	private void OnPop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private bool IsSelectedBodyValid()
	{
		if (metaData == null)
		{
			return false;
		}
		return metaData.canBeSoldOnMarketPlace;
	}

	public void OnRemovePressed()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnRemoveFromMarketplace));
		MVGameControllerBase.OperationRequests.DeleteAvatarFromShopInventory(woID);
	}

	private void OnRemoveFromMarketplace(bool added)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Remove(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnRemoveFromMarketplace));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		string text = TM._("Failed to remove Avatar from your shop.");
		if (added)
		{
			text = TM._("Avatar removed from your shop.");
			removeButton.gameObject.SetActive(value: false);
			sellButtonText.text = TM._("Sell");
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(text, string.Empty);
		});
	}
}
