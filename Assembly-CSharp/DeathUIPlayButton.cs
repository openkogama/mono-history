using UnityEngine;
using UnityEngine.EventSystems;

public class DeathUIPlayButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	[SerializeField]
	private DeathUIController deathUIController;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			deathUIController.OnPressPlay();
		}
	}
}
