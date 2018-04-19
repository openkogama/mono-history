using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class PlayerList : PlayerListBase
{
	private struct PlayerElementData
	{
		public int score;

		public PlayerElement playerElement;
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

	private List<PlayerElementData> playerElementList;

	[SerializeField]
	private RectTransform contentPanel;

	[SerializeField]
	private PlayerElement playerElementPrefab;

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
		playerElementList = new List<PlayerElementData>();
		for (int i = 0; i < winningConditionIcons.Count; i++)
		{
			winningConditionIcons[i].icon.SetActive(winningConditionIcons[i].scoreType == typeToDisplay);
		}
	}

	public override void Add(MVPlayer player)
	{
		PlayerElement playerElement = UnityEngine.Object.Instantiate(playerElementPrefab);
		playerElement.transform.SetParent(contentPanel, worldPositionStays: false);
		playerElement.gameObject.SetActive(value: true);
		int gameStat = player.GetGameStat(typeToDisplay);
		playerElement.Initialize(player, typeToDisplay, gameStat);
		playerCount++;
		PlayerElementData playerElementData = CreatePlayerElementHoldData(gameStat, playerElement);
		SortAfterScore(playerElementData);
	}

	private PlayerElementData CreatePlayerElementHoldData(int score, PlayerElement playerElement)
	{
		return new PlayerElementData
		{
			score = score,
			playerElement = playerElement
		};
	}

	private void SortAfterScore(PlayerElementData playerElementData)
	{
		bool flag = false;
		for (int i = 0; i < playerElementList.Count; i++)
		{
			if (!flag)
			{
				if (WinningConditionControl.IsNewScoreBetter(playerElementData.score, playerElementList[i].score, typeToDisplay))
				{
					playerElementList.Insert(i, playerElementData);
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
			playerElementList.Add(playerElementData);
		}
		for (int j = 0; j < playerElementList.Count; j++)
		{
			playerElementList[j].playerElement.UpdateScoreIndex();
		}
	}
}
