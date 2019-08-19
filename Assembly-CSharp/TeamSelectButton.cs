using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class TeamSelectButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
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

	private TeamData teamData;

	private UnityAction OnTeamSelected;

	public void Initialize(TeamData teamData, UnityAction OnTeamSelected)
	{
		this.teamData = teamData;
		this.OnTeamSelected = OnTeamSelected;
		buttonImage.color = Styles.GetTeamColor(teamData.team);
		playerImage.color = Styles.GetTeamColor(teamData.team, darkTeam: true);
		teamName.text = teamData.representedName;
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

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (MVGameControllerBase.Game.LocalPlayer.Team != teamData.team)
			{
				MVGameControllerBase.OperationRequests.SetTeam(teamData.team);
				MVGameControllerBase.Game.GameStatCounterManager.RemoveTeamScoreOnActorLeave(MVGameControllerBase.Game.LocalPlayer.ActorNr, MVGameControllerBase.Game.LocalPlayer.Team);
				MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
				MVGameControllerBase.Game.LocalPlayer.Team = teamData.team;
			}
			OnTeamSelected();
		}
	}

	private void StartPlaying()
	{
		if (!FirstTimePressPlayController.HaveBeenPressed)
		{
			FirstTimePressPlayController.OnFirstTimePlayIsPressed();
		}
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		if (MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.WoId != MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId)
		{
			MVGameControllerBase.OperationRequests.SetActiveSpawnRole(MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId);
		}
	}
}
