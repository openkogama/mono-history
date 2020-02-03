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

	private float startTime;

	private float timeUntilGhostMode;

	private bool shouldPop;

	private bool shouldShowPlayButtonAfterUnblocked;

	public void OpenMenu()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		MVGameControllerBase.PlayModeUI.InLobbyState = true;
	}

	public void Initialize(float timeUntilGhostMode)
	{
		this.timeUntilGhostMode = timeUntilGhostMode;
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
		if (GamePassesManager.GamePassesActive)
		{
			GamePassesUI gamePassesUI = Object.Instantiate(gamePassesUIPrefab);
			gamePassesUI.transform.SetParent(transform, worldPositionStays: false);
			gamePassesUI.Initialize();
			if (!GamePassProgressionController.IsProgressionEnabled || !GamePassesManager.GamePassesActive)
			{
				gamePassesUI.gameObject.SetActive(value: false);
			}
			else
			{
				int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
				if (gamePointAmountShown < progressionGamePoints)
				{
					gamePassesUI.ReplayGainEffect(gamePointAmountShown, progressionGamePoints);
				}
			}
		}
		if (GamePassesManager.PlayerPlanetData != null)
		{
			GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
			if (previewGamePassTier != GamePassTier.Tier0)
			{
				ShowContinueTierPopup(previewGamePassTier);
			}
		}
		if (MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion)
		{
			shouldShowPlayButtonAfterUnblocked = true;
			respawnButton.gameObject.SetActive(value: false);
		}
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
		timerFill.fillAmount = 1f - (Time.time - startTime) / timeUntilGhostMode;
		readyToPlayTimerFill.fillAmount = 1f - (Time.time - startTime) / timeUntilGhostMode;
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
			MVGameControllerDesktop.LockCursorManager.CursorLock = true;
			if (MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.SpawnRoleMode.Value == SpawnRoleModeType.Dead)
			{
				buttonFader.Unpause();
				readyToPlayTimerObject.SetActive(value: true);
				resetButtonFader.Activate();
				boostFader.Activate();
				menuButtonFader.Activate();
				MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetRespawnWhenPossible();
			}
			else
			{
				MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.Spawn();
			}
		}
	}

	private void OnAvatarStateChanged(SpawnRoleModeType mode)
	{
		if (mode != SpawnRoleModeType.Hidden && mode != SpawnRoleModeType.Dead)
		{
			shouldPop = true;
		}
	}

	private void ShowContinueTierPopup(GamePassTier previewTier)
	{
		ContinueTierBoostPopup continueTierBoostPopup = Object.Instantiate(continueTierBoostPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(continueTierBoostPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		continueTierBoostPopup.Initialize((int)previewTier);
	}
}
