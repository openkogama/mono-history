using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class WinningConditionDebriefingAndroid : MonoBehaviour, IDebriefing
{
	[Serializable]
	private struct WinningConditionDef
	{
		public WinningConditionType conditionType;

		public Sprite conditionSprite;
	}

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

	[SerializeField]
	private List<WinningConditionDef> winningConditionList;

	private Dictionary<WinningConditionType, Sprite> currentWinningConditions = new Dictionary<WinningConditionType, Sprite>();

	private float fadeTime = 0.3f;

	private void Start()
	{
		for (int i = 0; i < winningConditionList.Count; i++)
		{
			currentWinningConditions.Add(winningConditionList[i].conditionType, winningConditionList[i].conditionSprite);
		}
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningCondition = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningCondition, new Action<IWinningCondition>(OnWinningConditionReceived));
	}

	private void OnWinningConditionReceived(IWinningCondition winningCondition)
	{
		if (MVGameControllerBase.GameMode == MVGameMode.Play || (MVGameControllerBase.GameMode == MVGameMode.Edit && MVGameControllerBase.IEditModeUI.IsInPlayInEditMode))
		{
			GenerateDebriefing(winningCondition);
			HandleXp(winningCondition);
		}
	}

	private void GenerateDebriefing(IWinningCondition winningCondition)
	{
		if (winningCondition is IWinningConditionBriefing)
		{
			((IWinningConditionBriefing)winningCondition).GetDebriefing(this);
		}
	}

	private void HandleXp(IWinningCondition winningCondition)
	{
		if (!winningCondition.IsTeamMode)
		{
			List<ScoreActorEntry> list = winningCondition.HighScores.GenerateActorScores();
			if (list.Count > 0 && list[0].actorNumber == MVGameControllerBase.Game.LocalPlayer.ActorNr)
			{
				GameSessionCounters.Increment(GameSessionCounterType.GameWon);
			}
		}
	}

	public void SetupDebriefing(WinningConditionType winType, HighScores highScores, bool teamMode)
	{
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
		RenderPlayerToRenderTexture(scoreActorEntries[0].actorNumber);
		int num = scoreActorEntries[0].counter;
		if (num == 0)
		{
			foreach (MVPlayer value in MVGameControllerBase.Game.Players.Values)
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
		string additionalInformation = string.Empty;
		switch (counterType)
		{
		case GameStatCounterType.YUp:
			additionalInformation = TM._("Reach highest altitude");
			break;
		case GameStatCounterType.YDown:
			additionalInformation = TM._("Reach lowest altitude");
			break;
		}
		string winValue = FormatCount(counterType, num);
		debriefing.SetWinValue(winValue);
		debriefing.SetWinningConditionSprite(currentWinningConditions[winType]);
		debriefing.SetAdditionalInformation(additionalInformation);
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
		debriefing.SetWinnerText(scoreTeamEntries[0].team.ToString() + " team wins!");
		debriefing.SetWinningConditionSprite(currentWinningConditions[winType]);
		int num = scoreTeamEntries[0].counter;
		if (num == 0)
		{
			foreach (MVPlayer value in MVGameControllerBase.Game.Players.Values)
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
		captureCamera.CapturePlayersInTeam(scoreTeamEntries, CameraClearFlags.Depth, counterType);
		debriefing.SetWinnerImage(captureCamera.RenderCam.targetTexture);
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
		captureCamera.CaptureAllPlayersInGame(CameraClearFlags.Depth);
		debriefing = UnityEngine.Object.Instantiate(noWinnerPrefab);
		debriefing.transform.SetParent(group.gameObject.transform, worldPositionStays: false);
		debriefing.SetWinnerText(TM._("Time's Up!"));
		debriefing.SetWinningConditionSprite(currentWinningConditions[WinningConditionType.Time]);
		debriefing.SetWinnerImage(captureCamera.RenderCam.targetTexture);
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
		StopAllCoroutines();
		if (debriefing != null)
		{
			UnityEngine.Object.Destroy(debriefing.gameObject);
			debriefing = null;
		}
	}

	private IEnumerator WaitForFadeOut()
	{
		while (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState != MVGameStateType.PrepareRound)
		{
			debriefing.SetTimerText(string.Empty);
			yield return 0;
		}
		while ((float)MVGameControllerBase.Game.NetworkGameStateListener.TimeLeftMS / 1000f - fadeTime > 0f)
		{
			debriefing.SetTimerText((Mathf.Ceil(MVGameControllerBase.Game.NetworkGameStateListener.TimeLeftMS / 1000) + 1f).ToString());
			yield return 0;
		}
	}

	private static string FormatCount(GameStatCounterType statType, int count)
	{
		switch (statType)
		{
		case GameStatCounterType.Kill:
		case GameStatCounterType.YUp:
		case GameStatCounterType.YDown:
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

	private void RenderPlayerToRenderTexture(int actorNr)
	{
		if (MVGameControllerBase.Game.Players.TryGetValue(actorNr, out var value))
		{
			debriefing.SetWinnerText(value.Username);
			if (captureCamera != null)
			{
				UnityEngine.Object.Destroy(captureCamera.gameObject);
			}
			captureCamera = UnityEngine.Object.Instantiate(captureCameraPrefab);
			captureCamera.CaptureGO(value.Avatar.GameObject, CameraClearFlags.Color);
			debriefing.SetWinnerImage(captureCamera.RenderCam.targetTexture);
		}
		else
		{
			Debug.LogWarning("Winning player can't be found. Probably left game session");
		}
	}
}
