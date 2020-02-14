using UnityEngine;
using UnityEngine.EventSystems;

public class RespawnUIPlayButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	[SerializeField]
	private DeathUIController respawnUIController;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			respawnUIController.OnPressPlay();
		}
	}
}
