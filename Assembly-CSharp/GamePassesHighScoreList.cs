using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.GamePassSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesHighScoreList : MonoBehaviour
{
	private enum GamePassesHighScoreListState
	{
		Inactive,
		LoadingLocalPlayerHighScoreList,
		LoadingTopPlayerHighScoreList,
		ShowingLocalPlayerHighScoreList,
		ShowingTopPlayersHighScoreList
	}

	private struct HighScoreListData
	{
		public List<HighScoreEntry> scoreList;

		public int topRank;
	}

	[SerializeField]
	private GamePassesHighScoreElement highScoreElementPrefab;

	[SerializeField]
	private GameObject touristInformationPopup;

	[SerializeField]
	private Transform contentList;

	[SerializeField]
	private GameObject waitForHighScore;

	[SerializeField]
	private Text headerText;

	[SerializeField]
	private Text loadingText;

	[SerializeField]
	private int scoreElementShowCapacity = 13;

	[SerializeField]
	private int middlePosition = 7;

	[SerializeField]
	private float moveAmountPerElement = 101f;

	[SerializeField]
	private float topPadding = 40f;

	[SerializeField]
	private EmbeddedPlayerConfig embeddedPlayerConfig;

	private Dictionary<GamePassesHighScoreListState, HighScoreListData> highScoreListDatas = new Dictionary<GamePassesHighScoreListState, HighScoreListData>();

	private GamePassesHighScoreListState currentState;

	private void Start()
	{
		GamePassesHighScoreUpdateManager.OnHighScoreUpdate = (Action<HighScoreDatas>)Delegate.Combine(GamePassesHighScoreUpdateManager.OnHighScoreUpdate, new Action<HighScoreDatas>(OnHighScoreUpdate));
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			SetToTopPlayerHighScore();
		}
		else
		{
			SetToLocalPlayerHighScore();
		}
		EmbeddedSiteConfigData currentSiteData = embeddedPlayerConfig.GetCurrentSiteData();
		bool flag = currentSiteData.allowsModals || currentSiteData.allowsOpenInNewTab || currentSiteData.allowsRedirectToWebpage;
		if (MVGameControllerBase.IsTouristSession && flag)
		{
			GameObject informationPopup = UnityEngine.Object.Instantiate(touristInformationPopup);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(informationPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
			});
		}
	}

	private void OnDestroy()
	{
		GamePassesHighScoreUpdateManager.OnHighScoreUpdate = (Action<HighScoreDatas>)Delegate.Remove(GamePassesHighScoreUpdateManager.OnHighScoreUpdate, new Action<HighScoreDatas>(OnHighScoreUpdate));
	}

	private void SetToLocalPlayerHighScore()
	{
		headerText.text = "High Scores";
		if (highScoreListDatas.ContainsKey(GamePassesHighScoreListState.ShowingLocalPlayerHighScoreList))
		{
			currentState = GamePassesHighScoreListState.ShowingLocalPlayerHighScoreList;
			waitForHighScore.SetActive(value: false);
			CreateHighScoreList();
		}
		else
		{
			LoadLocalPlayerHighScore();
		}
	}

	private void LoadLocalPlayerHighScore()
	{
		waitForHighScore.SetActive(value: true);
		loadingText.text = "Loading High Scores...";
		currentState = GamePassesHighScoreListState.LoadingLocalPlayerHighScoreList;
		MVGameControllerBase.OperationRequests.GetHighScoreList();
	}

	private void SetToTopPlayerHighScore()
	{
		headerText.text = "Top Players";
		if (highScoreListDatas.ContainsKey(GamePassesHighScoreListState.ShowingTopPlayersHighScoreList))
		{
			currentState = GamePassesHighScoreListState.ShowingTopPlayersHighScoreList;
			waitForHighScore.SetActive(value: false);
			CreateHighScoreList();
		}
		else
		{
			LoadTopPlayerHighScore();
		}
	}

	private void LoadTopPlayerHighScore()
	{
		waitForHighScore.SetActive(value: true);
		loadingText.text = "Loading Top Players Scores...";
		currentState = GamePassesHighScoreListState.LoadingTopPlayerHighScoreList;
		MVGameControllerBase.OperationRequests.GetTopHighScoreList();
	}

	private void OnHighScoreUpdate(HighScoreDatas newHighScoreData)
	{
		LoadDone();
		CreateHighScoreListData(newHighScoreData);
		CreateHighScoreList();
	}

	private void LoadDone()
	{
		waitForHighScore.SetActive(value: false);
		if (currentState == GamePassesHighScoreListState.LoadingLocalPlayerHighScoreList)
		{
			currentState = GamePassesHighScoreListState.ShowingLocalPlayerHighScoreList;
		}
		if (currentState == GamePassesHighScoreListState.LoadingTopPlayerHighScoreList)
		{
			currentState = GamePassesHighScoreListState.ShowingTopPlayersHighScoreList;
		}
	}

	private void CreateHighScoreListData(HighScoreDatas newHighScoreData)
	{
		List<HighScoreEntry> list = SortHighScoreEntry(newHighScoreData.highScores);
		int topRank = newHighScoreData.topRank;
		int topRank2 = CalculateRealTopRank(topRank, list);
		HighScoreListData value = new HighScoreListData
		{
			scoreList = list,
			topRank = topRank2
		};
		highScoreListDatas.Add(currentState, value);
	}

	private List<HighScoreEntry> SortHighScoreEntry(List<HighScoreEntry> listToSort)
	{
		List<HighScoreEntry> list = new List<HighScoreEntry>();
		for (int i = 0; i < listToSort.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				if (listToSort[i].gamePoints > list[j].gamePoints)
				{
					list.Insert(j, listToSort[i]);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(listToSort[i]);
			}
		}
		return list;
	}

	private void CreateHighScoreList()
	{
		int topRank = highScoreListDatas[currentState].topRank;
		List<HighScoreEntry> scoreList = highScoreListDatas[currentState].scoreList;
		ClearHighScoreElements();
		CreateHighScoreElements(scoreList, topRank);
		HandlePlayerNotCentered();
	}

	private int CalculateRealTopRank(int playerRank, List<HighScoreEntry> highScoresEntries)
	{
		if (playerRank == 1)
		{
			return playerRank;
		}
		int playerPosition = GetPlayerPosition(highScoresEntries);
		return playerRank - playerPosition;
	}

	private int GetPlayerPosition(List<HighScoreEntry> highScoresEntries)
	{
		int result = 0;
		for (int i = 0; i < highScoresEntries.Count; i++)
		{
			if (highScoresEntries[i].profileID == MVGameControllerBase.Game.LocalPlayer.ProfileID)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private void HandlePlayerNotCentered()
	{
		if (currentState == GamePassesHighScoreListState.ShowingTopPlayersHighScoreList)
		{
			return;
		}
		int playerPosition = GetPlayerPosition(highScoreListDatas[currentState].scoreList);
		if (playerPosition > scoreElementShowCapacity)
		{
			int num = playerPosition + 1 - middlePosition;
			int num2 = playerPosition - (highScoreListDatas[currentState].scoreList.Count - middlePosition);
			if (num2 < 0)
			{
				num2 = num;
			}
			int num3 = Mathf.Min(num, num2);
			Vector3 localPosition = contentList.transform.localPosition;
			localPosition.y = topPadding + (float)num3 * moveAmountPerElement + (float)middlePosition * moveAmountPerElement;
			contentList.transform.localPosition = localPosition;
		}
	}

	private void CreateHighScoreElements(List<HighScoreEntry> listOfScores, int topRank)
	{
		for (int i = 0; i < listOfScores.Count; i++)
		{
			int userRank = i + topRank;
			int gamePoints = listOfScores[i].gamePoints;
			string username = listOfScores[i].username;
			int profileID = listOfScores[i].profileID;
			GamePassesHighScoreElement gamePassesHighScoreElement = UnityEngine.Object.Instantiate(highScoreElementPrefab);
			bool isSubscriber = listOfScores[i].isSubscriber;
			gamePassesHighScoreElement.Initialize(userRank, username, gamePoints, profileID, isSubscriber);
			gamePassesHighScoreElement.transform.SetParent(contentList, worldPositionStays: false);
			if (i == 0)
			{
				gamePassesHighScoreElement.DeactivateTopBorder();
			}
		}
	}

	private void ClearHighScoreElements()
	{
		for (int num = contentList.childCount - 1; num >= 0; num--)
		{
			Transform child = contentList.GetChild(num);
			child.parent = null;
			UnityEngine.Object.Destroy(child);
		}
	}

	public void Exit()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
