using System.Collections.Generic;
using MV.WorldObject.Subscription;

public class ScoreBoardSingleBase : ScoreBoardBase
{
	public override void OnStatsChange(int actorNumber, int scoreCount)
	{
		if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(actorNumber, out var player))
		{
			bool activateMemberUI = player.SubscriptionRules.HasBenefit(SubscriptionBenefit.XPBoost);
			if (IsNewScoreBetter(scoreCount, scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].Score))
			{
				SortNewScore(player.UserProfileData.UserName, actorNumber, scoreCount, activateMemberUI);
			}
		}
	}

	public override void Initialize(GameStatCounterType statType)
	{
		base.Initialize(statType);
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
				bool activateMemberUI = item.Value.SubscriptionRules.HasBenefit(SubscriptionBenefit.XPBoost);
				if (IsNewScoreBetter(actorCount, scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].Score))
				{
					SortNewScore(item.Value.UserProfileData.UserName, item.Value.ActorNr, actorCount, activateMemberUI);
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
