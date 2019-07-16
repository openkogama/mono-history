using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public class TeamMenuButton : MonoBehaviour
{
	[SerializeField]
	private TeamMenu teamMenuPrefab;

	[SerializeField]
	private GameObject buttonEnabler;

	private void Start()
	{
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange += AvatarStateChanged;
		MVGameControllerBase.Game.TeamManager.OnTeamAdded += TeamChanged;
		MVGameControllerBase.Game.TeamManager.OnTeamRemoved += TeamChanged;
		SetButtonIsActive();
	}

	private void AvatarStateChanged(SpawnRoleModeType state)
	{
		if ((state & SpawnRoleModeType.Hidden) > SpawnRoleModeType.None)
		{
			SetButtonIsActive();
		}
	}

	private void TeamChanged(object sender, TeamEventArgs eventArgs)
	{
		SetButtonIsActive();
	}

	private void SetButtonIsActive()
	{
		bool active = MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
		buttonEnabler.SetActive(active);
	}

	public void ShowTeamMenu()
	{
		TeamMenu newTeamMenu = Object.Instantiate(teamMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(newTeamMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.RemoveFromGame();
		Debug.Log("ShowTeamMenu");
	}
}
