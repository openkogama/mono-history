using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnRoleSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IEventSystemHandler
{
	[SerializeField]
	private SpawnRoleMenu spawnRoleMenu;

	private bool isMouseOver;

	public void OnPointerUp(PointerEventData eventData)
	{
		if (isMouseOver && eventData.button == PointerEventData.InputButton.Left)
		{
			spawnRoleMenu.OnSelectButtonPressed();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (isMouseOver && eventData.button == PointerEventData.InputButton.Left)
		{
			spawnRoleMenu.OnSelectButtonPressed();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isMouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isMouseOver = false;
	}
}
