using System;
using UnityEngine;

public class MVGUIMarketPlaceActionDialog : UXCustomDialogBox
{
	private const float LOADING_CIRCLE_SPEED = 12f;

	public UXPlane loadingCircle;

	private void Update()
	{
		loadingCircle.transform.Rotate(Vector3.forward, 12f * Time.deltaTime * 57.29578f);
	}

	public void SellItem(int itemID, string name, string description, int silverPrice)
	{
		MVNetworkGame game = MVGameController.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
		MVGameController.Game.RequestAddItemToMarketPlace(itemID, name, description, silverPrice);
	}

	public void RemoveItem(int itemID)
	{
		MVNetworkGame game = MVGameController.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
		MVGameController.Game.RequestRemoveItemFromMarketPlace(itemID);
	}

	private void OnAddToMarketplaceReturn(bool success)
	{
		MVNetworkGame game = MVGameController.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Remove(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
		if (success)
		{
			OnPositiveClose();
		}
		else
		{
			OnNegativeClose();
		}
		DialogFactory.CloseDialog();
	}
}
