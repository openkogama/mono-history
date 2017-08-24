using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class WinningConditionDebriefing : MonoBehaviour, IDebriefing
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
		RenderPlayerToRenderTexture(scoreActorEntries[0].actorNumber);
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
		debriefing.SetWinnerImage(scoreTeamEntries[0].team, captureCamera.RenderCam.targetTexture);
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
		debriefing.SetWinningConditionSprite(currentWinningConditions[WinningConditionType.Time]);
		debriefing.SetWinnerImage(MVTeam.None, captureCamera.RenderCam.targetTexture);
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
			MVInputWrapper.IsShortcutKeysSuppressed = true;
			MVInputWrapper.IsInGameInputSuppressed = true;
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
		MVPlayer mVPlayer = MVGameControllerBase.Game.MVPlayerContainer[actorNr];
		if (mVPlayer != null)
		{
			debriefing.SetWinnerText(mVPlayer.Username);
			if (captureCamera != null)
			{
				UnityEngine.Object.Destroy(captureCamera.gameObject);
			}
			captureCamera = UnityEngine.Object.Instantiate(captureCameraPrefab);
			captureCamera.CapturePlayer(mVPlayer);
			debriefing.SetWinnerImage(MVTeam.Blue, captureCamera.RenderCam.targetTexture);
		}
		else
		{
			Debug.LogWarning("Winning player can't be found. Probably left game session");
		}
	}
}
