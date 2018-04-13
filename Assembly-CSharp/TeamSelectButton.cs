using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamSelectButton : MonoBehaviour
{
	[SerializeField]
	private Image buttonImage;

	[SerializeField]
	private Text teamName;

	[SerializeField]
	private Text playerCountText;

	[SerializeField]
	private Text friendCountText;

	[SerializeField]
	private Image playerImage;

	[SerializeField]
	private GameObject friendIcon;

	[SerializeField]
	private WinningConditionBriefing winningConditionBriefingMenu;

	private TeamData teamData;

	public void Initialize(TeamData teamData)
	{
		this.teamData = teamData;
		Styles.SetStyle(buttonImage, teamData.team);
		playerImage.color = Styles.GetTeamColor(teamData.team, darkTeam: true);
		teamName.text = teamData.team.ToString();
		playerCountText.text = MVGameControllerBase.Game.TeamManager.GetNoOfPlayersInTeam(teamData.team).ToString();
		Dictionary<int, MVPlayer> onlineFriends = MVGameControllerBase.Game.Friends.GetOnlineFriends();
		int num = 0;
		foreach (KeyValuePair<int, MVPlayer> item in onlineFriends)
		{
			if (item.Value.Team == teamData.team)
			{
				num++;
			}
		}
		friendCountText.text = num.ToString();
		if (num == 0)
		{
			friendIcon.SetActive(value: false);
		}
	}

	public void OnTeamSelected()
	{
		if (MVGameControllerBase.Game.LocalPlayer.Team != teamData.team)
		{
			MVGameControllerBase.OperationRequests.SetTeam(teamData.team);
		}
		MVGameControllerBase.Game.LocalPlayer.Team = teamData.team;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (WinningConditionControl.TryGetPrioritizedWinCondition(out var condition))
		{
			WinningConditionBriefing winConMenu = Object.Instantiate(winningConditionBriefingMenu);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(winConMenu.gameObject, UIPushOption.Blocking | UIPushOption.HideAll, null, UIGroupFlags.InventoryUI);
			});
			winConMenu.Initialize(condition);
		}
	}
}
