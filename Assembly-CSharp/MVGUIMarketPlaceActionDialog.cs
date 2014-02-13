using System;
using UnityEngine;

public class MVGUIMarketPlaceActionDialog : UXCustomDialogBox
{
	private const float LOADING_CIRCLE_SPEED = 12f;

	public UXPlane loadingCircle;

	private void Update()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((Component)loadingCircle).transform.RotateAroundLocal(Vector3.forward, 12f * Time.deltaTime);
	}

	public void SellItem(int itemID, string name, string description, int silverPrice)
	{
		MVNetworkGame game = MVGameController.Instance.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
		MVGameController.Instance.Game.RequestAddItemToMarketPlace(itemID, name, description, silverPrice);
	}

	public void RemoveItem(int itemID)
	{
		MVNetworkGame game = MVGameController.Instance.Game;
		game.OnMarketPlaceActionComplete = (MVNetworkGame.OnMarketPlaceActionCompleteDelegate)Delegate.Combine(game.OnMarketPlaceActionComplete, new MVNetworkGame.OnMarketPlaceActionCompleteDelegate(OnAddToMarketplaceReturn));
		MVGameController.Instance.Game.RequestRemoveItemFromMarketPlace(itemID);
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
