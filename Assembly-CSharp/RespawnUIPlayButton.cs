using UnityEngine;
using UnityEngine.EventSystems;

public class RespawnUIPlayButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	[SerializeField]
	private RespawnUIController respawnUIController;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			respawnUIController.OnPlayPressed();
		}
	}
}
