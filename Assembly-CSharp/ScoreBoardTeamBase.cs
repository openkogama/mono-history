using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class ScoreBoardTeamBase : ScoreBoardBase
{
	public override void Initialize(GameStatCounterType statType)
	{
		base.Initialize(statType);
		AddTeamsToScoreBoard();
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			OnStatsChange(value.ActorNr, value.GetGameStat(statType));
		}
	}

	public override void OnStatsChange(int actorNumber, int scoreCount)
	{
		if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(actorNumber, out var player) && IsNewScoreBetter(scoreCount, scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].Score))
		{
			SortNewScore(GetBestPlayerInTeamName(player.Team), (int)player.Team, scoreCount);
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

	protected override void UnSubscribeToCallbacks()
	{
		base.UnSubscribeToCallbacks();
	}

	private void AddTeamsToScoreBoard()
	{
		for (int i = 0; i < MVGameControllerBase.Game.TeamManager.GetTeamList().Count; i++)
		{
			MVTeam mVTeam = MVGameControllerBase.Game.TeamManager.GetTeamList()[i];
			SortNewScore(GetBestPlayerInTeamName(mVTeam), (int)mVTeam, MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(statType, mVTeam));
		}
	}

	protected override Color GetBackgroundColor(int teamId)
	{
		if (teamId == -1)
		{
			return Styles.GetColor(ColorStyle.OffWhiteTransparent);
		}
		Color teamColor = Styles.GetTeamColor((MVTeam)teamId);
		teamColor.a = backgroundAlpha;
		return teamColor;
	}

	protected override void HandleParticipantListChanged()
	{
		for (int i = 0; i < scoreBoardPlayerData.Count && i < MVGameControllerBase.Game.TeamManager.GetTeamList().Count; i++)
		{
			scoreBoardPlayerData[i].Background.gameObject.SetActive(value: true);
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
