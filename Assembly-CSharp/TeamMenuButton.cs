using UnityEngine;
using UnityEngine.EventSystems;

public class TeamMenuButton : MonoBehaviour
{
	[SerializeField]
	private TeamMenu teamMenuPrefab;

	public void ShowTeamMenu()
	{
		TeamMenu newTeamMenu = Object.Instantiate(teamMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(newTeamMenu.gameObject, UIPushOption.Blocking | UIPushOption.HideAll, null, UIGroupFlags.InventoryUI);
		});
	}
}
