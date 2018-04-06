using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class ScoreBoardTeamBase : ScoreBoardBase
{
	private void Start()
	{
		MVGameControllerBase.Game.TeamManager.OnTeamAdded += OnTeamListChanged;
		MVGameControllerBase.Game.TeamManager.OnTeamRemoved += OnTeamRemoval;
	}

	public override void Initialize(GameStatCounterType statType)
	{
		base.Initialize(statType);
		AddTeamsToScoreBoard();
		OnTeamListChanged();
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			OnStatsChange(value.ActorNr, value.GetGameStat(statType));
		}
	}

	public override void OnStatsChange(int actorNumber, int scoreCount)
	{
		if (MVGameControllerBase.Game.MVPlayerContainer.ContainsKey(actorNumber))
		{
			MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(actorNumber);
			if (IsNewScoreBetter(scoreCount, scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].Score))
			{
				SortNewScore(GetBestPlayerInTeamName(playerUnsafe.Team), (int)playerUnsafe.Team, scoreCount);
			}
		}
	}

	public override void ReSortScoreBoard()
	{
		base.ReSortScoreBoard();
		AddTeamsToScoreBoard();
	}

	protected string GetBestPlayerInTeamName(MVTeam team)
	{
		List<MVPlayer> playersInTeam = MVGameControllerBase.Game.TeamManager.GetPlayersInTeam(team);
		MVPlayer mVPlayer = null;
		int oldScore = -1;
		for (int i = 0; i < playersInTeam.Count; i++)
		{
			if (playersInTeam[i] != null)
			{
				int actorCount = MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(statType, team, playersInTeam[i].ActorNr);
				if (IsNewScoreBetter(actorCount, oldScore))
				{
					mVPlayer = playersInTeam[i];
					oldScore = actorCount;
				}
			}
		}
		if (mVPlayer == null)
		{
			return string.Empty;
		}
		return mVPlayer.Username;
	}

	protected void OnTeamRemoval(object sender, TeamEventArgs e)
	{
		for (int i = 0; i < scoreBoardPlayerData.Count; i++)
		{
			if (scoreBoardPlayerData[i].Id == (int)e.team)
			{
				scoreBoardPlayerData[i].Score = 0;
				SortNewScore(GetBestPlayerInTeamName(e.team), (int)e.team, 0);
			}
		}
		OnTeamListChanged(sender, e);
	}

	protected void OnTeamListChanged(object sender, TeamEventArgs e)
	{
		OnTeamListChanged();
		AddTeamsToScoreBoard();
	}

	protected void OnTeamListChanged()
	{
		HandleParticipantListChanged();
	}

	protected override void UnSubscribeToCallbacks()
	{
		base.UnSubscribeToCallbacks();
		if (MVGameControllerBase.Game != null)
		{
			MVGameControllerBase.Game.TeamManager.OnTeamAdded -= OnTeamListChanged;
			MVGameControllerBase.Game.TeamManager.OnTeamRemoved -= OnTeamListChanged;
		}
	}

	private void AddTeamsToScoreBoard()
	{
		for (int i = 0; i < MVGameControllerBase.Game.TeamManager.GetTeamList().Count; i++)
		{
			if (WinningConditionControl.CurrentWinningCondition.CanWinningConditionBeFullfilledForTeam(MVGameControllerBase.Game.TeamManager.GetTeamList()[i]))
			{
				SortNewScore(GetBestPlayerInTeamName(MVGameControllerBase.Game.TeamManager.GetTeamList()[i]), (int)MVGameControllerBase.Game.TeamManager.GetTeamList()[i], 0);
			}
		}
	}

	protected override Color GetBackgroundColor(int teamId)
	{
		if (teamId == -1)
		{
			return Styles.GetColor(ColorStyle.OffWhiteTransparent);
		}
		return Styles.GetTeamColor((MVTeam)teamId);
	}

	protected override void HandleParticipantListChanged()
	{
		for (int i = 0; i < scoreBoardPlayerData.Count && i < MVGameControllerBase.Game.TeamManager.GetTeamList().Count; i++)
		{
			if (WinningConditionControl.CurrentWinningCondition.CanWinningConditionBeFullfilledForTeam((MVTeam)scoreBoardPlayerData[i].Id))
			{
				scoreBoardPlayerData[i].Background.gameObject.SetActive(value: true);
				continue;
			}
			scoreBoardPlayerData[i].Background.color = Styles.GetColor(ColorStyle.OffWhiteTransparent);
			scoreBoardPlayerData[i].PlacementText.text = string.Empty;
			scoreBoardPlayerData[i].NameText.text = string.Empty;
			scoreBoardPlayerData[i].ScoreText.text = string.Empty;
			scoreBoardPlayerData[i].Score = -1;
		}
		for (int j = MVGameControllerBase.Game.TeamManager.GetTeamList().Count; j < scoreBoardPlayerData.Count; j++)
		{
			scoreBoardPlayerData[j].Background.color = Styles.GetColor(ColorStyle.OffWhiteTransparent);
			scoreBoardPlayerData[j].PlacementText.text = string.Empty;
			scoreBoardPlayerData[j].NameText.text = string.Empty;
			scoreBoardPlayerData[j].ScoreText.text = string.Empty;
			scoreBoardPlayerData[j].Score = -1;
		}
	}

	protected override bool IsNewScoreBetter(int newScore, int oldScore, int newId, int oldId)
	{
		if (newScore == oldScore)
		{
			if (MVGameControllerBase.Game.TeamManager.GetPlayersInTeam((MVTeam)newId).Count == 0)
			{
				return false;
			}
			if (MVGameControllerBase.Game.TeamManager.GetPlayersInTeam((MVTeam)oldId).Count == 0)
			{
				return true;
			}
		}
		return IsNewScoreBetter(newScore, oldScore);
	}
}
