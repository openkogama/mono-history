using System;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamMenuButton : MonoBehaviour
{
	[SerializeField]
	private TeamMenu teamMenuPrefab;

	[SerializeField]
	private GameObject buttonEnabler;

	[SerializeField]
	private Image teamImage;

	private void Start()
	{
		buttonEnabler.SetActive(value: false);
		MVRuntimeDataVariable avatarModeTypeFlags = MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags;
		avatarModeTypeFlags.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarModeTypeFlags.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AvatarStateChanged));
		SetButtonTeamColor();
	}

	private void AvatarStateChanged(object state)
	{
		int num = (int)state;
		if ((num & 4) > 0)
		{
			bool flag = MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
			buttonEnabler.SetActive(flag);
			if (flag)
			{
				SetButtonTeamColor();
			}
		}
	}

	private void SetButtonTeamColor()
	{
		MVTeam teamFromActorNr = MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(MVGameControllerBase.Game.LocalPlayerActorNumber);
		Color teamColor = Styles.GetTeamColor(teamFromActorNr);
		teamImage.color = teamColor;
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
			x.Push(newTeamMenu.gameObject, UIPushOption.Blocking | UIPushOption.HideAll, null, UIGroupFlags.InventoryUI);
		});
	}
}
