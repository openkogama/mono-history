using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnRoleMenuScrollHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IEventSystemHandler
{
	[SerializeField]
	private SpawnRoleMenu spawnRoleMenu;

	public void OnBeginDrag(PointerEventData data)
	{
		spawnRoleMenu.OnBeginDrag();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		spawnRoleMenu.OnEndDrag();
	}

	public void OnDrag(PointerEventData eventData)
	{
		spawnRoleMenu.OnDrag();
	}
}
