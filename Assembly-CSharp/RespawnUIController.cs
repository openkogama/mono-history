using System;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUIController : MonoBehaviour
{
	[SerializeField]
	private NotificationFade fader;

	[SerializeField]
	private Image timerFill;

	[SerializeField]
	private Text restartText;

	[SerializeField]
	private NotificationFade resetUIFader;

	[SerializeField]
	private NotificationFade buttonFader;

	[SerializeField]
	private Image readyToPlayTimerFill;

	[SerializeField]
	private GameObject readyToPlayTimerObject;

	[SerializeField]
	private CanvasGroup resetUICanvasGroup;

	private float waitTime;

	private const float delayDuration = 1.2f;

	private const float briefingDuration = 2.8f;

	private bool isDeathBriefActive;

	public void OnResetToSpawnPoint()
	{
		if (buttonFader.IsPaused && resetUIFader.gameObject.activeSelf)
		{
			resetUIFader.Activate();
			buttonFader.Unpause();
			readyToPlayTimerObject.SetActive(value: true);
			MVGameControllerBase.WOCM.AvatarLocal.AvatarRespawnHandler.ShouldRespawnAsGhost = false;
			FlagDebriefingControl.ResetToSpawnPoint();
			restartText.text = "Respawning at start...";
		}
	}

	public void OnPlayPressed()
	{
		if (buttonFader.IsPaused)
		{
			MVGameControllerDesktop.LockCursorManager.CursorLock = true;
			buttonFader.Unpause();
			if (resetUIFader.gameObject.activeSelf)
			{
				resetUIFader.Activate();
			}
			readyToPlayTimerObject.SetActive(value: true);
			MVGameControllerBase.WOCM.AvatarLocal.AvatarRespawnHandler.ShouldRespawnAsGhost = false;
		}
	}

	private void Start()
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

	private void Initialize()
	{
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		avatarLocal.OnSuicide = (Action)Delegate.Combine(avatarLocal.OnSuicide, new Action(OnLocalAvatarSuicide));
		MVRuntimeDataVariable avatarModeTypeFlags = MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags;
		avatarModeTypeFlags.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarModeTypeFlags.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnAvatarStateChanged));
		NotificationFade notificationFade = fader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(OnFadeFinished));
	}

	private void LateInitialize()
	{
		Initialize();
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(Initialize));
	}

	private void OnAvatarStateChanged(object state)
	{
		AvatarModeTypes avatarModeTypes = (AvatarModeTypes)state;
		if (isDeathBriefActive && avatarModeTypes != AvatarModeTypes.Hidden && avatarModeTypes != AvatarModeTypes.Dead)
		{
			fader.Deactivate();
			fader.gameObject.SetActive(value: false);
			gameObject.SetActive(value: false);
			isDeathBriefActive = false;
			resetUICanvasGroup.alpha = 1f;
		}
	}

	private void Update()
	{
		if (waitTime + 1.2f < Time.time)
		{
			if (!isDeathBriefActive)
			{
				fader.gameObject.SetActive(value: true);
				fader.Activate();
				isDeathBriefActive = true;
			}
			float num = waitTime + 1.2f;
			timerFill.fillAmount = 1f - (Time.time - num) / 2.8f;
			readyToPlayTimerFill.fillAmount = 1f - (Time.time - num) / 2.8f;
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.NotificationAcceptFriendshipRequest))
		{
			OnResetToSpawnPoint();
		}
	}

	private void OnFadeFinished()
	{
		fader.gameObject.SetActive(value: false);
		gameObject.SetActive(value: false);
		isDeathBriefActive = false;
	}

	private void OnLocalAvatarSuicide()
	{
		if (!FlagDebriefingControl.IsInFlagDebriefing)
		{
			WinningConditionControl.TryGetPrioritizedStat(out var statType);
			MVCheckpoint checkpoint = MVGameControllerBase.Game.LocalPlayer.GetCheckpoint();
			if (statType != GameStatCounterType.TimeAttackFlag || checkpoint == null)
			{
				resetUIFader.gameObject.SetActive(value: false);
			}
			else
			{
				resetUIFader.gameObject.SetActive(value: true);
			}
			waitTime = Time.time;
			gameObject.SetActive(value: true);
			buttonFader.Activate();
			buttonFader.PauseAt(0f);
			readyToPlayTimerObject.SetActive(value: false);
			MVGameControllerBase.WOCM.AvatarLocal.AvatarRespawnHandler.ShouldRespawnAsGhost = true;
			if (checkpoint != null)
			{
				restartText.text = "Respawning at checkpoint...";
			}
			else
			{
				restartText.text = "Respawning at start...";
			}
		}
	}
}
