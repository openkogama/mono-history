using System;
using UnityEngine;

public class MVGUIAvatarMarketPlaceActionDialog : UXCustomDialogBox
{
	private const float LOADING_CIRCLE_SPEED = 12f;

	public UXPlane loadingCircle;

	private void Update()
	{
		loadingCircle.transform.Rotate(Vector3.forward, 12f * Time.deltaTime * 57.29578f);
	}

	public void SellAvatar(int woID, int priceSilver, string name, byte[] imageData)
	{
		MVNetworkGame game = MVGameController.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
		MVGameController.Game.AddAvatarToAvatarShopInventory(woID, priceSilver, name, imageData);
	}

	public void DeleteAvatar(int avatarID)
	{
		MVNetworkGame game = MVGameController.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
		MVGameController.Game.DeleteAvatarFromShopInventory(avatarID);
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
