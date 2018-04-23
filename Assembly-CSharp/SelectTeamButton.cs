using UnityEngine;
using UnityEngine.EventSystems;

public class SelectTeamButton : MonoBehaviour
{
	[SerializeField]
	private GameObject button;

	[SerializeField]
	private TeamMenu teamMenuPrefab;

	private void Start()
	{
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			button.SetActive(value: true);
		}
	}

	public void ShowTeamMenu()
	{
		TeamMenu newTeamMenu = Object.Instantiate(teamMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(newTeamMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker);
		});
	}
}
