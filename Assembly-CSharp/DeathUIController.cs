using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
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

	[SerializeField]
	private DeathUIBoostMenuController boostMenuPrefab;

	private GameStatCounterType statType;

	private float waitTime;

	private const float delayDuration = 1.2f;

	private const float briefingDuration = 2.8f;

	private bool isDeathBriefActive;

	private void Awake()
	{
		gameObject.SetActive(value: false);
		Initialize();
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.SpawnRoleDataMediatorLocal.OnKilled -= OnLocalPlayerKilled;
			MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange -= OnAvatarStateChanged;
			FlagDebriefingControl flagDebriefingControl = MVGameControllerBase.FlagDebriefingControl;
			flagDebriefingControl.OnFlagDebriefingEnd = (Action)Delegate.Remove(flagDebriefingControl.OnFlagDebriefingEnd, new Action(EndDeathBriefing));
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnRoundEnd));
			NotificationFade notificationFade = fader;
			notificationFade.OnFinished = (Action)Delegate.Remove(notificationFade.OnFinished, new Action(OnFadeFinished));
		}
	}

	private void Initialize()
	{
		MVGameControllerBase.SpawnRoleDataMediatorLocal.OnKilled += OnLocalPlayerKilled;
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange += OnAvatarStateChanged;
		FlagDebriefingControl flagDebriefingControl = MVGameControllerBase.FlagDebriefingControl;
		flagDebriefingControl.OnFlagDebriefingEnd = (Action)Delegate.Combine(flagDebriefingControl.OnFlagDebriefingEnd, new Action(EndDeathBriefing));
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnRoundEnd));
		NotificationFade notificationFade = fader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(OnFadeFinished));
		WinningConditionControl.TryGetPrioritizedStat(out statType);
		scoreBoardSingle.Initialize(statType);
		scoreBoardTeam.Initialize(statType);
		localPlayeScore.Initialize();
	}

	private void OnAvatarStateChanged(SpawnRoleModeType mode)
	{
		if (isDeathBriefActive && mode != SpawnRoleModeType.Hidden && mode != SpawnRoleModeType.Dead)
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
		if (fader.gameObject.activeSelf)
		{
			fader.gameObject.SetActive(value: false);
			isDeathBriefActive = false;
			gameObject.SetActive(value: false);
		}
	}

	private void OnLocalPlayerKilled(int localPlayerActorNr, int dmgDealerActorNr, PlayerKilledByType damageType)
	{
		bool shotSelf = localPlayerActorNr == dmgDealerActorNr;
		Color color;
		Color color2;
		if (MVGameControllerBase.Game.TeamManager.GetTeamList().Count > 1)
		{
			color = Styles.GetTeamColor(MVGameControllerBase.Game.MVPlayerContainer[localPlayerActorNr].Team);
			color2 = Styles.GetTeamColor(MVGameControllerBase.Game.MVPlayerContainer[dmgDealerActorNr].Team);
		}
		else
		{
			color = Styles.GetColor(ColorStyle.Gray);
			color2 = Styles.GetColor(ColorStyle.Gray);
		}
		string userName = MVGameControllerBase.Game.MVPlayerContainer[localPlayerActorNr].UserProfileData.UserName;
		string userName2 = MVGameControllerBase.Game.MVPlayerContainer[dmgDealerActorNr].UserProfileData.UserName;
		string deathText = string.Format(KillNotification.GetKillText(damageType, shotSelf), Styles.ColorToHex(color), userName, Styles.ColorToHex(color2), userName2);
		if (!MVGameControllerBase.FlagDebriefingControl.IsInFlagDebriefing)
		{
			StartDeathBriefing(deathText);
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
	}

	private void OnRoundEnd(IWinningCondition winningCondition)
	{
		EndDeathBriefing();
	}

	public void OnPressPlay()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		buttonFader.Unpause();
		readyToPlayTimerObject.SetActive(value: true);
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetRespawnWhenPossible();
	}
}
