using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeathUIBoostMenuController : MonoBehaviour
{
	[SerializeField]
	private Image timerFill;

	[SerializeField]
	private Text restartText;

	[SerializeField]
	private NotificationFade fader;

	[SerializeField]
	private NotificationFade buttonFader;

	[SerializeField]
	private NotificationFade resetButtonFader;

	[SerializeField]
	private NotificationFade boostFader;

	[SerializeField]
	private NotificationFade menuButtonFader;

	[SerializeField]
	private Image readyToPlayTimerFill;

	[SerializeField]
	private GameObject readyToPlayTimerObject;

	[SerializeField]
	private PointerDownController respawnButton;

	[SerializeField]
	private PointerDownController resetButton;

	[SerializeField]
	private BoostMenuController boostMenu;

	[SerializeField]
	private GamePassesUI gamePassesUIPrefab;

	[SerializeField]
	private ContinueTierBoostPopup continueTierBoostPopupPrefab;

	[SerializeField]
	private ContinueButtonLockCursor continueButtonLockCursor;

	private float startTime;

	private float timeUntilGhostMode;

	private bool shouldPop;

	private bool shouldShowPlayButtonAfterUnblocked;

	private bool wantsToPlay;

	private bool isGhost;

	public void Initialize(float timeUntilGhostMode)
	{
		this.timeUntilGhostMode = Mathf.Max(timeUntilGhostMode, 0f);
		if (timeUntilGhostMode <= 0f)
		{
			readyToPlayTimerFill.gameObject.SetActive(value: false);
			timerFill.gameObject.SetActive(value: false);
			if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.Value != SpawnRoleModeType.Hidden)
			{
				MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SpawnAsGhost();
			}
			isGhost = true;
		}
		startTime = Time.time;
		fader.Activate();
		fader.ShouldHideWhenDone = false;
		respawnButton.Initialize(OnRespawn);
		resetButton.Initialize(OnResetToSpawnPoint);
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange += OnAvatarStateChanged;
		boostMenu.Initialize();
		MVCheckpoint checkpoint = MVGameControllerBase.Game.LocalPlayer.GetCheckpoint();
		buttonFader.Activate();
		buttonFader.PauseAt(0f);
		readyToPlayTimerObject.SetActive(value: false);
		if (checkpoint != null)
		{
			restartText.text = "Respawning at checkpoint...";
		}
		else
		{
			resetButtonFader.gameObject.SetActive(value: false);
			restartText.text = "Respawning at start...";
		}
		int gamePointAmountShown = GamePointGainEffectManager.GamePointAmountShown;
		if (!GamePassesManager.GamePassesActive)
		{
			return;
		}
		GamePassesUI gamePassesUI = Object.Instantiate(gamePassesUIPrefab);
		gamePassesUI.transform.SetParent(transform, worldPositionStays: false);
		gamePassesUI.Initialize();
		if (!GamePassProgressionController.IsProgressionEnabled || !GamePassesManager.GamePassesActive)
		{
			gamePassesUI.gameObject.SetActive(value: false);
			return;
		}
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		if (gamePointAmountShown < progressionGamePoints)
		{
			gamePassesUI.ReplayGainEffect(gamePointAmountShown, progressionGamePoints);
		}
	}

	public void OpenMenu()
	{
		if (buttonFader.IsPaused)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IDeathPromotionSelector x, BaseEventData y) =>
			{
				x.TryShowPromotion(ReadyToEnterMenu);
			});
		}
	}

	private void ReadyToEnterMenu(bool promotionPushedToStack)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		MVGameControllerBase.PlayModeUI.InLobbyState = true;
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange -= OnAvatarStateChanged;
		}
	}

	private void Update()
	{
		float num = 1f - (Time.time - startTime) / timeUntilGhostMode;
		timerFill.fillAmount = num;
		readyToPlayTimerFill.fillAmount = num;
		bool isBlocked = false;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			isBlocked = x.IsUIElementBlocked(gameObject);
		});
		if (shouldShowPlayButtonAfterUnblocked && !isBlocked)
		{
			shouldShowPlayButtonAfterUnblocked = false;
			respawnButton.gameObject.SetActive(value: true);
			buttonFader.Activate();
			buttonFader.PauseAt(0f);
		}
		if (shouldPop && !isBlocked)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
		if (!isGhost && num <= 0f)
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SpawnAsGhost();
			isGhost = true;
		}
		if (wantsToPlay && num <= 0f)
		{
			wantsToPlay = false;
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.EnterPlayingState();
		}
	}

	private void OnEnable()
	{
		buttonFader.Activate();
		buttonFader.PauseAt(0f);
	}

	private void OnResetToSpawnPoint()
	{
		MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
		restartText.text = "Respawning at start...";
		if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.Value == SpawnRoleModeType.Hidden)
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetToSpawnPoint();
		}
		OnRespawn();
	}

	private void OnRespawn()
	{
		if (buttonFader.IsPaused)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IDeathPromotionSelector x, BaseEventData y) =>
			{
				x.TryShowPromotion(ReadyToSpawn);
			});
			bool isBlocked = false;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				isBlocked = x.IsUIElementBlocked(gameObject);
			});
			shouldShowPlayButtonAfterUnblocked = isBlocked;
			respawnButton.gameObject.SetActive(value: false);
		}
	}

	private void ReadyToSpawn(bool promotionPushedToStack)
	{
		float num = 1f - (Time.time - startTime) / timeUntilGhostMode;
		if (MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.SpawnRoleMode.Value == SpawnRoleModeType.Dead || num > 0f)
		{
			buttonFader.Unpause();
			readyToPlayTimerObject.SetActive(value: true);
			resetButtonFader.Activate();
			boostFader.Activate();
			menuButtonFader.Activate();
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetRespawnWhenPossible();
			wantsToPlay = true;
		}
		else
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.EnterPlayingState();
		}
	}

	private void LockCursorAndPop()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.EnterPlayingState();
	}

	private void OnAvatarStateChanged(SpawnRoleModeType mode)
	{
		if (mode != SpawnRoleModeType.Hidden && mode != SpawnRoleModeType.Dead)
		{
			shouldPop = true;
		}
	}
}
