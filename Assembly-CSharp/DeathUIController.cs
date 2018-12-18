using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class DeathUIController : MonoBehaviour
{
	[SerializeField]
	private Text deathReason;

	[SerializeField]
	private NotificationFade fader;

	[SerializeField]
	private Image timerFill;

	[SerializeField]
	private NotificationFade buttonFader;

	[SerializeField]
	private Image readyToPlayTimerFill;

	[SerializeField]
	private GameObject readyToPlayTimerObject;

	[SerializeField]
	private ScoreBoardSingleBase scoreBoardSingle;

	[SerializeField]
	private ScoreBoardTeamBase scoreBoardTeam;

	[SerializeField]
	private LocalPlayerScore localPlayeScore;

	private GameStatCounterType statType;

	private float waitTime;

	private const float delayDuration = 1.2f;

	private const float briefingDuration = 2.8f;

	private bool isDeathBriefActive;

	private void Awake()
	{
		gameObject.SetActive(value: false);
		if (MVGameControllerBase.WOCM.AvatarLocal == null)
		{
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Combine(mVPlayerContainer.OnLocalPlayerReady, new Action(LateInitialize));
		}
		else
		{
			Initialize();
		}
	}

	private void LateInitialize()
	{
		Initialize();
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(Initialize));
	}

	private void Initialize()
	{
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		avatarLocal.OnKilled = (Action<string>)Delegate.Combine(avatarLocal.OnKilled, new Action<string>(OnLocalAvatarKilled));
		MVRuntimeDataVariable avatarModeTypeFlags = MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags;
		avatarModeTypeFlags.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarModeTypeFlags.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnAvatarStateChanged));
		FlagDebriefingControl.OnFlagDebriefingEnd = (Action)Delegate.Combine(FlagDebriefingControl.OnFlagDebriefingEnd, new Action(EndDeathBriefing));
		NotificationFade notificationFade = fader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(OnFadeFinished));
		WinningConditionControl.TryGetPrioritizedStat(out statType);
		scoreBoardSingle.Initialize(statType);
		scoreBoardTeam.Initialize(statType);
		localPlayeScore.Initialize();
	}

	private void OnAvatarStateChanged(object state)
	{
		AvatarModeTypes avatarModeTypes = (AvatarModeTypes)state;
		if (isDeathBriefActive && avatarModeTypes != AvatarModeTypes.Hidden && avatarModeTypes != AvatarModeTypes.Dead)
		{
			EndDeathBriefing();
		}
	}

	private void EndDeathBriefing()
	{
		fader.Deactivate();
		fader.gameObject.SetActive(value: false);
		buttonFader.Deactivate();
		gameObject.SetActive(value: false);
		isDeathBriefActive = false;
	}

	private void Update()
	{
		if (waitTime + 1.2f < Time.time)
		{
			if (!isDeathBriefActive)
			{
				fader.gameObject.SetActive(value: true);
				fader.Activate();
				HandleScoreBoardVisibility();
				localPlayeScore.Activate();
				SendCurrentProgressNotification();
				isDeathBriefActive = true;
			}
			float num = waitTime + 1.2f;
			timerFill.fillAmount = 1f - (Time.time - num) / 2.8f;
			if (timerFill.fillAmount <= 0f)
			{
				OnFadeFinished();
			}
			readyToPlayTimerFill.fillAmount = 1f - (Time.time - num) / 2.8f;
		}
	}

	private void OnFadeFinished()
	{
		if (fader.gameObject.activeSelf)
		{
			fader.gameObject.SetActive(value: false);
			isDeathBriefActive = false;
			gameObject.SetActive(value: false);
		}
	}

	private void OnLocalAvatarKilled(string text)
	{
		if (!FlagDebriefingControl.IsInFlagDebriefing)
		{
			StartDeathBriefing(text);
		}
	}

	private void HandleScoreBoardVisibility()
	{
		WinningConditionControl.TryGetPrioritizedStat(out var gameStatCounterType);
		if (gameStatCounterType == GameStatCounterType.None)
		{
			if (scoreBoardSingle.gameObject.activeSelf)
			{
				scoreBoardSingle.gameObject.SetActive(value: false);
			}
			if (scoreBoardTeam.gameObject.activeSelf)
			{
				scoreBoardTeam.gameObject.SetActive(value: false);
			}
		}
		else if (MVGameControllerBase.Game.TeamManager.GetTeamList().Count > 1)
		{
			if (!scoreBoardTeam.gameObject.activeSelf)
			{
				scoreBoardTeam.gameObject.SetActive(value: true);
			}
			if (scoreBoardSingle.gameObject.activeSelf)
			{
				scoreBoardSingle.gameObject.SetActive(value: false);
			}
			if (gameStatCounterType != statType)
			{
				statType = gameStatCounterType;
				scoreBoardTeam.ChangeStatType(statType);
			}
			else
			{
				scoreBoardTeam.ReSortScoreBoard();
			}
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
			if (gameStatCounterType != statType)
			{
				statType = gameStatCounterType;
				scoreBoardSingle.ChangeStatType(statType);
			}
			else
			{
				scoreBoardSingle.ReSortScoreBoard();
			}
		}
	}

	private void SendCurrentProgressNotification()
	{
		WinningConditionControl.TryGetPrioritizedStat(out var gameStatCounterType);
		if (gameStatCounterType != GameStatCounterType.None)
		{
			Dictionary<object, object> data = new Dictionary<object, object>();
			NotificationController.PushNotification(NotificationType.CurrentProgress, data);
		}
	}

	private void StartDeathBriefing(string deathText)
	{
		waitTime = Time.time;
		gameObject.SetActive(value: true);
		buttonFader.Activate();
		buttonFader.PauseAt(0f);
		readyToPlayTimerObject.SetActive(value: false);
		deathReason.text = deathText;
		MVGameControllerBase.WOCM.AvatarLocal.AvatarRespawnHandler.ShouldRespawnAsGhost = true;
	}

	public void OnPressPlay()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		buttonFader.Unpause();
		readyToPlayTimerObject.SetActive(value: true);
		MVGameControllerBase.WOCM.AvatarLocal.AvatarRespawnHandler.ShouldRespawnAsGhost = false;
	}
}
