using System;
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
		MVRuntimeDataVariable avatarModeTypeFlags = MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags;
		avatarModeTypeFlags.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarModeTypeFlags.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AvatarStateChanged));
		SetButtonIsActive();
	}

	private void AvatarStateChanged(object state)
	{
		int num = (int)state;
		if ((num & 4) > 0)
		{
			SetButtonIsActive();
		}
	}

	private void SetButtonIsActive()
	{
		bool active = MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
		buttonEnabler.SetActive(active);
	}

	public void ShowTeamMenu()
	{
		TeamMenu newTeamMenu = UnityEngine.Object.Instantiate(teamMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(newTeamMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}
}
