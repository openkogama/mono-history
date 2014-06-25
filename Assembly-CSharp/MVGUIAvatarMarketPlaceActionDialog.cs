using System;
using UnityEngine;

public class MVGUIAvatarMarketPlaceActionDialog : UXCustomDialogBox
{
	private const float LOADING_CIRCLE_SPEED = 12f;

	public UXPlane loadingCircle;

	private void Update()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((Component)loadingCircle).transform.RotateAroundLocal(Vector3.forward, 12f * Time.deltaTime);
	}

	public void SellAvatar(int woID, int priceSilver, string name, byte[] imageData)
	{
		MVNetworkGame game = MVGameController.Instance.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
		MVGameController.Instance.Game.AddAvatarToAvatarShopInventory(woID, priceSilver, name, imageData);
	}

	public void DeleteAvatar(int avatarID)
	{
		MVNetworkGame game = MVGameController.Instance.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
		MVGameController.Instance.Game.DeleteAvatarFromShopInventory(avatarID);
	}

	private void OnAddToMarketplaceReturn(bool success)
	{
		MVNetworkGame game = MVGameController.Instance.Game;
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
