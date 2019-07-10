using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
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

	[SerializeField]
	private DeathUIBoostMenuController boostMenuPrefab;

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
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetRespawnWhenPossible();
			MVGameControllerBase.FlagDebriefingControl.ResetToSpawnPoint();
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
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetRespawnWhenPossible();
		}
	}

	private void Start()
	{
		gameObject.SetActive(value: false);
		Initialize();
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.SpawnRoleDataMediatorLocal.OnSuicide -= OnLocalAvatarSuicide;
			MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange -= OnAvatarStateChanged;
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnRoundEnd));
		}
	}

	private void Initialize()
	{
		MVGameControllerBase.SpawnRoleDataMediatorLocal.OnSuicide += OnLocalAvatarSuicide;
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange += OnAvatarStateChanged;
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnRoundEnd));
		NotificationFade notificationFade = fader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(OnFadeFinished));
	}

	private void OnAvatarStateChanged(SpawnRoleModeType mode)
	{
		if (isDeathBriefActive && mode != SpawnRoleModeType.Hidden && mode != SpawnRoleModeType.Dead)
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
		if (waitTime + 1.2f < Time.time && !isDeathBriefActive && MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState != MVGameStateType.RoundEnded)
		{
			isDeathBriefActive = true;
			float num = waitTime;
			float timeUntilGhostMode = num + 1.2f + 2.8f - Time.time;
			DeathUIBoostMenuController boostMenu = UnityEngine.Object.Instantiate(boostMenuPrefab);
			boostMenu.Initialize(timeUntilGhostMode);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(boostMenu.gameObject, UIPushOption.HideAll, null, UIGroupFlags.GameObjectUI);
			});
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
		if (!MVGameControllerBase.FlagDebriefingControl.IsInFlagDebriefing)
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

	private void OnRoundEnd(IWinningCondition winningCondition)
	{
		fader.Deactivate();
		fader.gameObject.SetActive(value: false);
		gameObject.SetActive(value: false);
		isDeathBriefActive = false;
		resetUICanvasGroup.alpha = 1f;
	}
}
