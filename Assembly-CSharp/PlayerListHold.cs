using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class PlayerListHold : PlayerListBase
{
	private struct PlayerElementHoldData
	{
		public int score;

		public PlayerElementHold playerElement;
	}

	[Serializable]
	private class ScoreIconDef
	{
		public GameStatCounterType scoreType;

		public GameObject icon;
	}

	private int score;

	private int playerCount;

	private MVTeam team;

	private GameStatCounterType typeToDisplay;

	private List<PlayerElementHoldData> playerElementList;

	[SerializeField]
	private RectTransform contentPanel;

	[SerializeField]
	private PlayerElementHold playerElementPrefab;

	[SerializeField]
	private TeamTab teamTab;

	[SerializeField]
	private List<ScoreIconDef> winningConditionIcons;

	public override MVTeam Team => team;

	public override int PlayerCount => playerCount;

	public override int Score => score;

	public override void Initialize(MVTeam team, int score, GameStatCounterType typeToDisplay)
	{
		this.score = score;
		this.team = team;
		this.typeToDisplay = typeToDisplay;
		teamTab.Initialize(team, typeToDisplay);
		playerElementList = new List<PlayerElementHoldData>();
		for (int i = 0; i < winningConditionIcons.Count; i++)
		{
			winningConditionIcons[i].icon.SetActive(winningConditionIcons[i].scoreType == typeToDisplay);
		}
	}

	public override void Add(MVPlayer player)
	{
		PlayerElementHold playerElementHold = UnityEngine.Object.Instantiate(playerElementPrefab);
		playerElementHold.transform.SetParent(contentPanel, worldPositionStays: false);
		playerElementHold.gameObject.SetActive(value: true);
		int gameStat = player.GetGameStat(typeToDisplay);
		playerElementHold.Initialize(player, typeToDisplay, gameStat);
		playerCount++;
		PlayerElementHoldData playerElementHoldData = CreatePlayerElementHoldData(gameStat, playerElementHold);
		SortAfterScore(playerElementHoldData);
	}

	private PlayerElementHoldData CreatePlayerElementHoldData(int score, PlayerElementHold playerElementHold)
	{
		return new PlayerElementHoldData
		{
			score = score,
			playerElement = playerElementHold
		};
	}

	private void SortAfterScore(PlayerElementHoldData playerElementHoldData)
	{
		bool flag = false;
		for (int i = 0; i < playerElementList.Count; i++)
		{
			if (!flag)
			{
				if (WinningConditionControl.IsNewScoreBetter(playerElementHoldData.score, playerElementList[i].score, typeToDisplay))
				{
					playerElementList.Insert(i, playerElementHoldData);
					flag = true;
				}
			}
			else
			{
				playerElementList[i].playerElement.transform.SetAsLastSibling();
			}
		}
		if (!flag)
		{
			playerElementList.Add(playerElementHoldData);
		}
		for (int j = 0; j < playerElementList.Count; j++)
		{
			playerElementList[j].playerElement.UpdateScoreIndex();
		}
	}
}
