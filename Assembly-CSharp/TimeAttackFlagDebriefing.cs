using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class TimeAttackFlagDebriefing : MonoBehaviour
{
	[SerializeField]
	private ScoreBoardSingleBase scoreBoardSingle;

	[SerializeField]
	private ScoreBoardTeamBase scoreBoardTeam;

	[SerializeField]
	private LocalPlayerScore localPlayerScore;

	[SerializeField]
	private GameObject sunshineObject;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private CanvasGroup scoreBoardCanvasGroup;

	[SerializeField]
	private Image countdownFill;

	[SerializeField]
	private Button playButton;

	private bool isDebriefingOn;

	private bool isWaitingForStart;

	private bool isExitingDebriefing;

	private float waitStartTime;

	private AvatarModeTypes previousAvatarModeType = AvatarModeTypes.Hidden;

	private float countdownEndTime;

	private const float waitDuration = 4f;

	private void Start()
	{
		MVRuntimeDataVariable avatarModeTypeFlags = MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags;
		avatarModeTypeFlags.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarModeTypeFlags.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnAvatarStateChanged));
		gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVRuntimeDataVariable avatarModeTypeFlags = MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags;
			avatarModeTypeFlags.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(avatarModeTypeFlags.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnAvatarStateChanged));
		}
	}

	private void Update()
	{
		if (isWaitingForStart)
		{
			if (MVGameControllerBase.CameraController.CurCamera.CameraType != CameraType.TimeAttackFlagCountdownCamera)
			{
				MVGameControllerBase.CameraController.PushCamera(CameraType.TimeAttackFlagCountdownCamera);
			}
			if (waitStartTime + 4f < Time.time && MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState != MVGameStateType.RoundEnded)
			{
				MVGameControllerBase.PlayModeUI.InLobbyState = false;
				MVGameControllerDesktop.LockCursorManager.CursorLock = true;
				isDebriefingOn = false;
				isWaitingForStart = false;
				gameObject.SetActive(value: false);
				countdownEndTime = Time.time;
				scoreBoardCanvasGroup.alpha = 0f;
				FlagDebriefingControl.EndFlagCountDown();
				MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
				MVGameControllerBase.WOCM.AvatarLocal.InteractableLocal.AddModifier(AvatarModifierPackageType.SpawnProtection);
			}
		}
		else
		{
			if (MVGameControllerBase.CameraController.CurCamera.CameraType == CameraType.TimeAttackFlagDebriefingCamera)
			{
				float num = 36f;
				MVGameControllerBase.CameraController.CurCamera.transform.Rotate(new Vector3(0f, num * Time.deltaTime, 0f));
			}
			UpdateButton();
		}
	}

	public void Initialize(int captureTime)
	{
		isExitingDebriefing = false;
		isDebriefingOn = true;
		canvasGroup.alpha = 1f;
		scoreBoardCanvasGroup.alpha = 1f;
		MVGameControllerBase.PlayModeUI.InLobbyState = true;
		scoreBoardSingle.Initialize(GameStatCounterType.TimeAttackFlag);
		scoreBoardTeam.Initialize(GameStatCounterType.TimeAttackFlag);
		localPlayerScore.Initialize();
		playButton.interactable = true;
		bool flag = WinningConditionControl.IsNewScoreBetter(captureTime, GetTopPlayerScore(), GameStatCounterType.TimeAttackFlag);
		HandleScoreBoardVisibility(captureTime);
		localPlayerScore.Activate();
		sunshineObject.SetActive(flag);
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.TimeAttackFlagDebriefing);
		SendNotification(captureTime, flag);
	}

	public void InitializeCountDown()
	{
		if (!isDebriefingOn)
		{
			isDebriefingOn = true;
			MVGameControllerBase.PlayModeUI.InLobbyState = false;
			LockCursor();
			canvasGroup.alpha = 0f;
			scoreBoardCanvasGroup.alpha = 1f;
			scoreBoardSingle.Initialize(GameStatCounterType.TimeAttackFlag);
			scoreBoardTeam.Initialize(GameStatCounterType.TimeAttackFlag);
			localPlayerScore.Initialize();
			MVPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
			HandleScoreBoardVisibility(MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.TimeAttackFlag, localPlayer.Team, localPlayer.ActorNr));
			localPlayerScore.Activate();
			isWaitingForStart = true;
			waitStartTime = Time.time - 1f;
			SendCountDownNotification();
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.TimeAttackFlagDebriefing);
		}
	}

	public void OnPressPlay()
	{
		isExitingDebriefing = true;
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			playButton.interactable = false;
		}
	}

	public void OnLeavePlayMode()
	{
		EndDebriefingEarly();
	}

	public void OnRoundEnd()
	{
		if (isActiveAndEnabled)
		{
			FlagDebriefingControl.EndFlagDebriefing();
			isDebriefingOn = false;
			isWaitingForStart = false;
			gameObject.SetActive(value: false);
			countdownEndTime = Time.time;
			scoreBoardCanvasGroup.alpha = 0f;
			FlagDebriefingControl.EndFlagCountDown();
			previousAvatarModeType = AvatarModeTypes.Hidden;
		}
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

	private void ResetPlayer()
	{
		MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
		((MVAvatarLocal)MVGameControllerBase.Game.LocalPlayer.Avatar).ResetAvatar();
		((MVAvatarLocal)MVGameControllerBase.Game.LocalPlayer.Avatar).SetToSpawnTransform();
		MVGameControllerBase.CameraController.CurCamera.Reset();
	}

	private void SendNotification(int captureTime, bool isBestTime)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)1, GetDebriefingText(captureTime, isBestTime));
		dictionary.Add((byte)4, captureTime);
		dictionary.Add((byte)18, true);
		NotificationController.PushNotification(NotificationType.TimeAttackFlagDebriefing, dictionary);
	}

	private void SendCountDownNotification()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)18, true);
		NotificationController.PushNotification(NotificationType.TimeAttackFlagCountDown, dictionary);
	}

	private string GetDebriefingText(int captureTime, bool isBestTime)
	{
		if (isBestTime)
		{
			return TM._("Best Time!");
		}
		MVPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		int actorCount = MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.TimeAttackFlag, localPlayer.Team, localPlayer.ActorNr);
		if (WinningConditionControl.IsNewScoreBetter(captureTime, actorCount, GameStatCounterType.TimeAttackFlag))
		{
			return TM._("Your Best Time");
		}
		return string.Empty;
	}

	private int GetTopPlayerScore()
	{
		int num = 0;
		foreach (KeyValuePair<int, MVPlayer> item in MVGameControllerBase.Game.MVPlayerContainer)
		{
			if (item.Value != null)
			{
				int actorNr = item.Value.ActorNr;
				int actorCount = MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.TimeAttackFlag, item.Value.Team, actorNr);
				if (WinningConditionControl.IsNewScoreBetter(actorCount, num, GameStatCounterType.TimeAttackFlag))
				{
					num = actorCount;
				}
			}
		}
		return num;
	}

	private void OnAvatarStateChanged(object state)
	{
		if (isWaitingForStart || countdownEndTime + 4f > Time.time)
		{
			return;
		}
		AvatarModeTypes avatarModeTypes = (AvatarModeTypes)state;
		WinningConditionControl.TryGetPrioritizedStat(out var statType);
		if (!isDebriefingOn && statType == GameStatCounterType.TimeAttackFlag && avatarModeTypes == AvatarModeTypes.Playing && (previousAvatarModeType == AvatarModeTypes.Hidden || previousAvatarModeType == AvatarModeTypes.Dead))
		{
			MVCheckpoint checkpoint = MVGameControllerBase.Game.LocalPlayer.GetCheckpoint();
			if (checkpoint == null)
			{
				FlagDebriefingControl.StartFlagCountDown();
			}
		}
		previousAvatarModeType = avatarModeTypes;
	}

	private void UpdateButton()
	{
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		if (flag)
		{
			countdownFill.fillAmount = MVGameControllerBase.Game.NetworkGameStateListener.CountdownInPercentage;
			if (!countdownFill.gameObject.activeSelf)
			{
				countdownFill.gameObject.SetActive(value: true);
			}
		}
		else if (countdownFill.gameObject.activeSelf)
		{
			countdownFill.gameObject.SetActive(value: false);
		}
		if (!flag && isExitingDebriefing)
		{
			ExitDebriefing();
		}
	}

	private void ExitDebriefing()
	{
		canvasGroup.alpha = 0f;
		isWaitingForStart = true;
		isExitingDebriefing = false;
		waitStartTime = Time.time;
		ResetPlayer();
		MVGameControllerBase.WOCM.AvatarLocal.Body.Animation.Play("Idle");
		FlagDebriefingControl.EndFlagDebriefing();
		if (previousAvatarModeType != AvatarModeTypes.Hidden)
		{
			LockCursor();
		}
		if (previousAvatarModeType != AvatarModeTypes.Playing)
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
		}
		MVGameControllerBase.WOCM.AvatarLocal.InteractableLocal.AddModifier(AvatarModifierPackageType.SpawnProtection);
	}

	private void EndDebriefingEarly()
	{
		if (isActiveAndEnabled)
		{
			FlagDebriefingControl.EndFlagDebriefing();
			MVGameControllerDesktop.LockCursorManager.CursorLock = true;
			isDebriefingOn = false;
			isWaitingForStart = false;
			gameObject.SetActive(value: false);
			countdownEndTime = Time.time;
			scoreBoardCanvasGroup.alpha = 0f;
			FlagDebriefingControl.EndFlagCountDown();
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
		}
	}

	private void LockCursor()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
	}
}
