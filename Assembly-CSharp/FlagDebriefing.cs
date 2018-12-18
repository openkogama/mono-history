using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class FlagDebriefing : MonoBehaviour
{
	[SerializeField]
	private Text captureTimeText;

	[SerializeField]
	private Text bestTimeText;

	[SerializeField]
	private NotificationFade fader;

	[SerializeField]
	private GameObject messageObject;

	[SerializeField]
	private ScoreBoardSingleBase scoreBoardSingle;

	[SerializeField]
	private ScoreBoardTeamBase scoreBoardTeam;

	[SerializeField]
	private LocalPlayerScore localPlayerScore;

	private int captureFlagTimeStamp;

	private int avatarStartTime;

	private void Start()
	{
		NotificationFade notificationFade = fader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(OnFadeFinished));
		avatarStartTime = MVGameControllerBase.Game.LocalPlayer.JoinTime;
		scoreBoardSingle.Initialize(GameStatCounterType.TimeAttackFlag);
		scoreBoardTeam.Initialize(GameStatCounterType.TimeAttackFlag);
		localPlayerScore.Initialize();
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerTeamChanged = (Action)Delegate.Combine(mVPlayerContainer.OnLocalPlayerTeamChanged, new Action(OnLocalPlayerChangeTeam));
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
		{
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnLocalPlayerTeamChanged = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerTeamChanged, new Action(OnLocalPlayerChangeTeam));
		}
	}

	private void OnFadeFinished()
	{
		fader.gameObject.SetActive(value: false);
	}

	private void OnLocalAvatarReachFlag()
	{
		WinningConditionControl.TryGetPrioritizedWinCondition(out var condition);
		if (condition == WinningConditionType.TimeAttackFlag)
		{
			int startTime = GetStartTime();
			int num = CalculateCaptureTime(startTime);
			int actorCount = MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.TimeAttackFlag, MVGameControllerBase.Game.LocalPlayer.Team, MVGameControllerBase.Game.LocalPlayer.ActorNr);
			if (WinningConditionControl.IsNewScoreBetter(num, actorCount, GameStatCounterType.TimeAttackFlag))
			{
				string value = WinningConditionControl.MakeIntoScoreText(num, GameStatCounterType.TimeAttackFlag);
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)1, value);
				Dictionary<object, object> data = dictionary;
				NotificationController.PushNotification(NotificationType.TimeAttackFlagDebriefing, data);
				if (messageObject.activeSelf)
				{
					messageObject.SetActive(value: false);
				}
			}
			else
			{
				string text = MakeDebriefingText(num);
				captureTimeText.text = text;
				string text2 = MakeBestTimeText(num);
				bestTimeText.text = text2;
				if (!messageObject.activeSelf)
				{
					messageObject.SetActive(value: true);
				}
			}
			fader.gameObject.SetActive(value: true);
			fader.Activate();
			HandleScoreBoardVisibility(num);
			localPlayerScore.Activate();
		}
		ResetPlayer();
	}

	private int GetStartTime()
	{
		int num = 0;
		num = ((MVGameControllerBase.Game.NetworkGameStateListener.StartTime >= avatarStartTime) ? MVGameControllerBase.Game.NetworkGameStateListener.StartTime : avatarStartTime);
		if (num < captureFlagTimeStamp && captureFlagTimeStamp != 0)
		{
			num = captureFlagTimeStamp;
		}
		return num;
	}

	private int CalculateCaptureTime(int startTime)
	{
		return MVGameControllerBase.Game.ServerTimeInMilliSeconds - startTime;
	}

	private string MakeDebriefingText(int captureTime)
	{
		string empty = string.Empty;
		return "Flag reached in " + WinningConditionControl.MakeIntoScoreText(captureTime, GameStatCounterType.TimeAttackFlag) + "!";
	}

	private string MakeBestTimeText(int captureTime)
	{
		int actorCount = MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.TimeAttackFlag, MVGameControllerBase.Game.LocalPlayer.Team, MVGameControllerBase.Game.LocalPlayer.ActorNr);
		return "Best time: " + WinningConditionControl.MakeIntoScoreText(actorCount, GameStatCounterType.TimeAttackFlag) + "!";
	}

	private void ResetPlayer()
	{
		captureFlagTimeStamp = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
		MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
		((MVAvatarLocal)MVGameControllerBase.Game.LocalPlayer.Avatar).ResetAvatar();
		((MVAvatarLocal)MVGameControllerBase.Game.LocalPlayer.Avatar).SetToSpawnTransform();
		MVGameControllerBase.CameraController.CurCamera.Reset();
	}

	private void HandleScoreBoardVisibility(int score)
	{
		if (MVGameControllerBase.Game.TeamManager.GetTeamList().Count > 1)
		{
			if (!scoreBoardTeam.gameObject.activeSelf)
			{
				scoreBoardTeam.gameObject.SetActive(value: true);
			}
			if (scoreBoardSingle.gameObject.activeSelf)
			{
				scoreBoardSingle.gameObject.SetActive(value: false);
			}
			scoreBoardTeam.ReSortScoreBoard();
			scoreBoardTeam.OnStatsChange(MVGameControllerBase.Game.LocalPlayer.ActorNr, score);
		}
		else
		{
			if (!scoreBoardSingle.gameObject.activeSelf)
			{
				scoreBoardSingle.gameObject.SetActive(value: true);
			}
			if (scoreBoardTeam.gameObject.activeSelf)
			{
				scoreBoardTeam.gameObject.SetActive(value: false);
			}
			scoreBoardSingle.ReSortScoreBoard();
			scoreBoardSingle.OnStatsChange(MVGameControllerBase.Game.LocalPlayer.ActorNr, score);
		}
	}

	private void OnLocalPlayerChangeTeam()
	{
		avatarStartTime = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
	}
}
