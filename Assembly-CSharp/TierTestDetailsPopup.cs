using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TierTestDetailsPopup : MonoBehaviour
{
	[SerializeField]
	private Text tierText;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private GamePassesShop gamePassesShopPrefab;

	private GamePassTier tierToPurchase;

	public void Initialize(GamePassTier tierToPurchase, int price)
	{
		this.tierToPurchase = tierToPurchase;
		Text text = tierText;
		int num = (int)tierToPurchase;
		text.text = num.ToString();
		priceText.text = price.ToString();
	}

	public void ShowTier()
	{
		GamePassesShop gamePassesShop = Object.Instantiate(gamePassesShopPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gamePassesShop.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		gamePassesShop.Initialize(tierToPurchase);
	}

	public void Test()
	{
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		if (gamePassTier != tierToPurchase)
		{
			MVGameControllerBase.OperationRequests.SetTier(tierToPurchase);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
	}

	private void OnEnable()
	{
		if (MVGameControllerBase.LocalPlayer.PlayerPlanetData != null && (int)MVGameControllerBase.LocalPlayer.PlayerPlanetData.gamePassTier >= (int)tierToPurchase)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
	}
}
