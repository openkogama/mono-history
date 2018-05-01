using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public class WinningConditionDebriefing : MonoBehaviour, IDebriefing
{
	[SerializeField]
	private CanvasGroup group;

	[SerializeField]
	private DebriefingWinnerGUI playerWinPrefab;

	[SerializeField]
	private DebriefingWinnerGUI teamWinPrefab;

	[SerializeField]
	private DebriefingWinnerGUI noWinnerPrefab;

	private DebriefingWinnerGUI debriefing;

	[SerializeField]
	private AvatarCapture captureCameraPrefab;

	private AvatarCapture captureCamera;

	private float fadeTime = 0.3f;

	private void Start()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionReceived));
	}

	private void OnWinningConditionReceived(IWinningCondition winningCondition)
	{
		Debug.Log("OnWinningConditionReceived");
		if (MVGameControllerBase.GameMode == MVGameMode.Play || (MVGameControllerBase.GameMode == MVGameMode.Edit && MVGameControllerBase.IEditModeUI.IsInPlayInEditMode))
		{
			GenerateDebriefing(winningCondition);
		}
	}

	private void GenerateDebriefing(IWinningCondition winningCondition)
	{
		Debug.Log("Generate debriefing");
		if (winningCondition is IWinningConditionBriefing)
		{
			((IWinningConditionBriefing)winningCondition).GetDebriefing(this);
		}
	}

	public void SetupDebriefing(WinningConditionType winType, HighScores highScores, bool teamMode)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		if (gameObject.activeInHierarchy)
		{
			if (teamMode)
			{
				SetupDebriefingTeam(winType, highScores.GenerateTeamScores(), highScores.gameStatCounterType);
			}
			else
			{
				SetupDebriefingPlayer(winType, highScores.GenerateActorScores(), highScores.gameStatCounterType);
			}
			group.blocksRaycasts = true;
		}
	}

	private void SetupDebriefingPlayer(WinningConditionType winType, List<ScoreActorEntry> scoreActorEntries, GameStatCounterType counterType)
	{
		if (scoreActorEntries.Count == 0)
		{
			Debug.Log("No winner");
			SetupDebriefingNoWinner();
			return;
		}
		Clear();
		debriefing = UnityEngine.Object.Instantiate(playerWinPrefab);
		debriefing.transform.SetParent(group.gameObject.transform, worldPositionStays: false);
		scoreActorEntries = GetWinningActorsFromScoreActorEntries(scoreActorEntries, counterType);
		List<int> list = new List<int>();
		for (int i = 0; i < scoreActorEntries.Count; i++)
		{
			list.Add(scoreActorEntries[i].actorNumber);
		}
		RenderPlayerToRenderTexture(list);
		int num = scoreActorEntries[0].counter;
		if (num == 0)
		{
			foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
			{
				if (value.ActorNr == scoreActorEntries[0].actorNumber)
				{
					int gameStat = value.GetGameStat(counterType);
					if (gameStat > num)
					{
						num = gameStat;
					}
				}
			}
		}
		string empty = string.Empty;
		string winValue = FormatCount(counterType, num);
		debriefing.SetWinValue(winValue);
		debriefing.SetAdditionalInformation(empty, winType);
		debriefing.ActivateScoreImage(winType);
		StartCoroutine(ShowDebriefingCoroutine());
	}

	private void SetupDebriefingTeam(WinningConditionType winType, List<ScoreTeamEntry> scoreTeamEntries, GameStatCounterType counterType)
	{
		if (scoreTeamEntries.Count == 0)
		{
			Debug.Log("No winner");
			SetupDebriefingNoWinner();
			return;
		}
		Clear();
		debriefing = UnityEngine.Object.Instantiate(teamWinPrefab);
		debriefing.transform.SetParent(group.gameObject.transform, worldPositionStays: false);
		scoreTeamEntries = GetWinningTeamsFromScoreTeamEntries(scoreTeamEntries, counterType);
		if (scoreTeamEntries.Count == 1)
		{
			debriefing.SetWinnerText(scoreTeamEntries[0].team.ToString() + " team wins!");
		}
		else
		{
			string text = "It's a tie";
			if (scoreTeamEntries.Count <= 2)
			{
				text += " between ";
				for (int i = 0; i < scoreTeamEntries.Count; i++)
				{
					text = text + scoreTeamEntries[i].team.ToString() + " team";
					if (i < scoreTeamEntries.Count - 1)
					{
						text += " and ";
					}
				}
			}
			text += "!";
			debriefing.SetWinnerText(text);
		}
		debriefing.ActivateScoreImage(winType);
		int num = scoreTeamEntries[0].counter;
		if (num == 0)
		{
			foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
			{
				if (value.Team == scoreTeamEntries[0].team)
				{
					int gameStat = value.GetGameStat(counterType);
					if (gameStat > num)
					{
						num = gameStat;
					}
				}
			}
		}
		string winValue = FormatCount(counterType, num);
		debriefing.SetWinValue(winValue);
		if (captureCamera != null)
		{
			UnityEngine.Object.Destroy(captureCamera.gameObject);
		}
		captureCamera = UnityEngine.Object.Instantiate(captureCameraPrefab);
		captureCamera.CapturePlayersInTeam(scoreTeamEntries, counterType);
		MVTeam team = MVTeam.None;
		if (scoreTeamEntries.Count == 1)
		{
			team = scoreTeamEntries[0].team;
		}
		debriefing.SetWinnerImage(Styles.GetTeamColor(team), captureCamera.RenderCam.targetTexture);
		StartCoroutine(ShowDebriefingCoroutine());
	}

	private void OnDisable()
	{
		Clear();
	}

	private void SetupDebriefingNoWinner()
	{
		Clear();
		if (captureCamera != null)
		{
			UnityEngine.Object.Destroy(captureCamera.gameObject);
		}
		captureCamera = UnityEngine.Object.Instantiate(captureCameraPrefab);
		captureCamera.CaptureAllPlayersInGame();
		debriefing = UnityEngine.Object.Instantiate(noWinnerPrefab);
		debriefing.transform.SetParent(group.gameObject.transform, worldPositionStays: false);
		debriefing.SetWinnerText(TM._("Time's Up!"));
		debriefing.SetWinnerImage(Styles.GetColor(ColorStyle.DarkNavyBlue), captureCamera.RenderCam.targetTexture);
		debriefing.ActivateScoreImage(WinningConditionType.None);
		StartCoroutine(ShowDebriefingCoroutine());
	}

	private IEnumerator ShowDebriefingCoroutine()
	{
		debriefing.SetTimerText(string.Empty);
		yield return StartCoroutine(pTween.To(fadeTime, 0f, 1f, (float t) =>
		{
			group.alpha = t;
		}));
		yield return StartCoroutine(WaitForFadeOut());
		yield return StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			group.alpha = t;
			if (t == 0f)
			{
				Clear();
			}
		}));
	}

	private void Clear()
	{
		if (captureCamera != null)
		{
			UnityEngine.Object.Destroy(captureCamera.gameObject);
		}
		group.alpha = 0f;
		group.blocksRaycasts = false;
		StopAllCoroutines();
		if (debriefing != null)
		{
			UnityEngine.Object.Destroy(debriefing.gameObject);
			debriefing = null;
		}
	}

	private void Update()
	{
		if (debriefing != null)
		{
			MVInputWrapper.SuppressShortcutKeys();
			MVInputWrapper.SuppressInGameInput();
		}
	}

	private IEnumerator WaitForFadeOut()
	{
		while (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded && MVGameControllerBase.Game.NetworkGameStateListener.TimeLeftMS > 3000)
		{
			debriefing.SetTimerText(MVGameControllerBase.Game.NetworkGameStateListener.CountdownInSeconds.ToString());
			yield return 0;
		}
	}

	private static string FormatCount(GameStatCounterType statType, int count)
	{
		switch (statType)
		{
		case GameStatCounterType.Kill:
		case GameStatCounterType.Collectible:
		case GameStatCounterType.OculusKill:
			return count.ToString();
		default:
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, count);
			return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
		}
		}
	}

	private void RenderPlayerToRenderTexture(List<int> actorNrs)
	{
		List<MVPlayer> list = new List<MVPlayer>();
		for (int i = 0; i < actorNrs.Count; i++)
		{
			if (MVGameControllerBase.Game.MVPlayerContainer.ContainsKey(actorNrs[i]))
			{
				MVPlayer item = MVGameControllerBase.Game.MVPlayerContainer[actorNrs[i]];
				list.Add(item);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j] == null)
			{
				list.RemoveAt(j);
				j--;
			}
		}
		if (list.Count >= 1)
		{
			string empty = string.Empty;
			if (list.Count == 1)
			{
				empty = list[0].Username;
			}
			else
			{
				empty += "It's a tie";
				if (list.Count <= 2)
				{
					empty += " between ";
					for (int k = 0; k < list.Count; k++)
					{
						empty += list[k].Username;
						if (k < list.Count - 1)
						{
							empty += " and ";
						}
					}
				}
				empty += "!";
			}
			debriefing.SetWinnerText(empty);
			if (captureCamera != null)
			{
				UnityEngine.Object.Destroy(captureCamera.gameObject);
			}
			captureCamera = UnityEngine.Object.Instantiate(captureCameraPrefab);
			captureCamera.CapturePlayer(list);
			debriefing.SetWinnerImage(Styles.GetTeamColor(MVTeam.Blue), captureCamera.RenderCam.targetTexture);
		}
		else
		{
			Debug.LogWarning("Winning player can't be found. Probably left game session");
		}
	}

	private List<ScoreTeamEntry> GetWinningTeamsFromScoreTeamEntries(List<ScoreTeamEntry> scoreTeamEntries, GameStatCounterType counterType)
	{
		List<ScoreTeamEntry> list = new List<ScoreTeamEntry>();
		for (int i = 0; i < scoreTeamEntries.Count; i++)
		{
			if (list.Count < 1)
			{
				list.Add(scoreTeamEntries[i]);
			}
			else if (WinningConditionControl.IsNewScoreBetter(scoreTeamEntries[i].counter, list[0].counter, counterType))
			{
				list.Clear();
				list.Add(scoreTeamEntries[i]);
			}
			else if (list[0].counter == scoreTeamEntries[i].counter)
			{
				list.Add(scoreTeamEntries[i]);
			}
		}
		return list;
	}

	private List<ScoreActorEntry> GetWinningActorsFromScoreActorEntries(List<ScoreActorEntry> scoreActorEntries, GameStatCounterType counterType)
	{
		List<ScoreActorEntry> list = new List<ScoreActorEntry>();
		for (int i = 0; i < scoreActorEntries.Count; i++)
		{
			if (list.Count < 1)
			{
				list.Add(scoreActorEntries[i]);
			}
			else if (WinningConditionControl.IsNewScoreBetter(scoreActorEntries[i].counter, list[0].counter, counterType))
			{
				list.Clear();
				list.Add(scoreActorEntries[i]);
			}
			else if (list[0].counter == scoreActorEntries[i].counter)
			{
				list.Add(scoreActorEntries[i]);
			}
		}
		return list;
	}
}
