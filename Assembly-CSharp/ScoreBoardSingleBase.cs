using System;
using System.Collections.Generic;

public class ScoreBoardSingleBase : ScoreBoardBase
{
	public override void OnStatsChange(int actorNumber, int scoreCount)
	{
		if (MVGameControllerBase.Game.MVPlayerContainer.ContainsKey(actorNumber))
		{
			MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(actorNumber);
			if (IsNewScoreBetter(scoreCount, scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].Score))
			{
				SortNewScore(playerUnsafe.Username, actorNumber, scoreCount);
			}
		}
	}

	public override void Initialize(GameStatCounterType statType)
	{
		base.Initialize(statType);
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Combine(mVPlayerContainer.OnPlayerListChanged, new Action(OnPlayerListChanged));
		HandleParticipantListChanged();
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			OnStatsChange(value.ActorNr, value.GetGameStat(statType));
		}
	}

	public override void ReSortScoreBoard()
	{
		base.ReSortScoreBoard();
		AddPlayersToScoreBoard();
	}

	protected void OnPlayerListChanged()
	{
		HandleParticipantListChanged();
		AddPlayersToScoreBoard();
	}

	protected override void UnSubscribeToCallbacks()
	{
		base.UnSubscribeToCallbacks();
		if (MVGameControllerBase.Game != null)
		{
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Remove(mVPlayerContainer.OnPlayerListChanged, new Action(OnPlayerListChanged));
		}
	}

	protected override void HandleParticipantListChanged()
	{
		for (int i = 0; i < scoreBoardPlayerData.Count && i < MVGameControllerBase.Game.MVPlayerContainer.Count; i++)
		{
			scoreBoardPlayerData[i].Background.gameObject.SetActive(value: true);
		}
		for (int j = MVGameControllerBase.Game.MVPlayerContainer.Count; j < scoreBoardPlayerData.Count; j++)
		{
			scoreBoardPlayerData[j].ScoreText.text = WinningConditionControl.MakeIntoScoreText(0, statType);
			scoreBoardPlayerData[j].NameText.text = string.Empty;
			scoreBoardPlayerData[j].Background.transform.SetAsLastSibling();
		}
	}

	private void AddPlayersToScoreBoard()
	{
		foreach (KeyValuePair<int, MVPlayer> item in MVGameControllerBase.Game.MVPlayerContainer)
		{
			if (item.Value != null)
			{
				int actorCount = MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(statType, item.Value.Team, item.Value.ActorNr);
				if (IsNewScoreBetter(actorCount, scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].Score))
				{
					SortNewScore(item.Value.Username, item.Value.ActorNr, actorCount);
				}
			}
		}
	}

	private void OnEnable()
	{
		ResetScoreBoard();
		OnPlayerListChanged();
	}
}
