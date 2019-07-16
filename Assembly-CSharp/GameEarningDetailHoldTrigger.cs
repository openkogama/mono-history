using UnityEngine;
using UnityEngine.EventSystems;

public class GameEarningDetailHoldTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	[SerializeField]
	private GameEarningsMenu earningsMenu;

	[SerializeField]
	private int earningToShowDetailsFor;

	public void OnPointerEnter(PointerEventData eventData)
	{
		earningsMenu.OnHighlightEarning(earningToShowDetailsFor);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		earningsMenu.OnStopHighlightEarning(earningToShowDetailsFor);
	}
}
