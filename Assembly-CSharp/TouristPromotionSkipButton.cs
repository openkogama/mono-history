using UnityEngine;
using UnityEngine.EventSystems;

public class TouristPromotionSkipButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	[SerializeField]
	private TouristPromotionDesktop touristPromotionDesktop;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			touristPromotionDesktop.SkipCallback();
		}
	}
}
