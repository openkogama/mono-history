using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePassesDetailHoldTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	[SerializeField]
	private GamePassesShopDetails gamePassesShopDetails;

	[SerializeField]
	private GamePassTier tierToShowDetailsFor;

	public void OnPointerEnter(PointerEventData eventData)
	{
		gamePassesShopDetails.OnTierDetailEnter(tierToShowDetailsFor);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		gamePassesShopDetails.OnTierDetailExit(tierToShowDetailsFor);
	}
}
