using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerScore : MonoBehaviour
{
	[SerializeField]
	private int scoreBoardCount;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Text playerNameText;

	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private Text rankingText;

	public void Initialize()
	{
		playerNameText.text = MVGameControllerBase.Game.LocalPlayer.Username;
	}

	public void Activate()
	{
		WinningConditionControl.TryGetPrioritizedStat(out var statType);
		if (statType != GameStatCounterType.None)
		{
			int actorNr = MVGameControllerBase.Game.LocalPlayer.ActorNr;
			MVTeam team = MVGameControllerBase.Game.LocalPlayer.Team;
			int actorCount = MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(statType, team, actorNr);
			int localPlayerRanking = GetLocalPlayerRanking(statType, team, actorNr, actorCount);
			if (localPlayerRanking > scoreBoardCount)
			{
				Show(statType, team, localPlayerRanking, actorCount);
			}
			else
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	private int GetLocalPlayerRanking(GameStatCounterType statType, MVTeam localTeam, int localActorNumber, int localScore)
	{
		int num = 1;
		foreach (KeyValuePair<int, MVPlayer> item in MVGameControllerBase.Game.MVPlayerContainer)
		{
			if (item.Value == null)
			{
				continue;
			}
			int actorNr = item.Value.ActorNr;
			if (actorNr != localActorNumber)
			{
				int actorCount = MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(statType, item.Value.Team, actorNr);
				if (WinningConditionControl.IsNewScoreBetter(actorCount, localScore, statType))
				{
					num++;
				}
			}
		}
		return num;
	}

	private void Show(GameStatCounterType statType, MVTeam localTeam, int currentRanking, int localScore)
	{
		gameObject.SetActive(value: true);
		rankingText.text = currentRanking.ToString();
		scoreText.text = WinningConditionControl.MakeIntoScoreText(localScore, statType);
		background.color = Styles.GetTeamColor(localTeam);
	}
}
