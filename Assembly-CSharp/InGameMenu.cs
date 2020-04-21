using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject respawnButton;

	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private TimedPlayReward playReward;

	[SerializeField]
	private AdOfferGold adOfferGold;

	[SerializeField]
	private Image inGameMenuBlockingOverlay;

	[SerializeField]
	private GameObject winningConditionDebriefing;

	[SerializeField]
	private GamePassesUI gamePassesUIPrefab;

	[SerializeField]
	private GameObject claimGoldRewardPopupPrefab;

	[SerializeField]
	private GameObject accessoryShopButton;

	[SerializeField]
	private EmbeddedPlayerConfig embeddedPlayerConfig;

	private GamePassesUI gamePassesUI;

	public void Initialize()
	{
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		bool flag = MVGameControllerBase.EditModeUI == null && !isTouristSession && MVGameControllerBase.GameMode == MVGameMode.Play;
		playReward.gameObject.SetActive(value: false);
		bool flag2 = MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion;
		EmbeddedSiteConfigData currentSiteData = embeddedPlayerConfig.GetCurrentSiteData();
		bool flag3 = currentSiteData.allowsModals || currentSiteData.allowsOpenInNewTab || currentSiteData.allowsRedirectToWebpage;
		touristRegisterButton.SetActive(flag2 && flag3);
		accessoryShopButton.SetActive(!MVGameControllerBase.IsTouristSession || (MVGameControllerBase.IsTouristSession && flag3));
		if (flag)
		{
			playReward.Initialize();
		}
		if (GamePassesManager.GamePassesActive)
		{
			gamePassesUI = Object.Instantiate(gamePassesUIPrefab);
			gamePassesUI.transform.SetParent(transform, worldPositionStays: false);
			gamePassesUI.Initialize();
			if (!GamePassProgressionController.IsProgressionEnabled)
			{
				gamePassesUI.gameObject.SetActive(value: false);
			}
		}
		winningConditionDebriefing.transform.SetAsLastSibling();
	}

	private void Update()
	{
		if (MVGameControllerDesktop.LockCursorManager.CursorLock)
		{
			inGameMenuBlockingOverlay.raycastTarget = true;
		}
		else
		{
			inGameMenuBlockingOverlay.raycastTarget = false;
		}
		if (!MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Playing) && respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: false);
		}
		else if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Playing) && !respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: true);
		}
		MVInputWrapper.SuppressInGameInput();
	}

	private void OnEnable()
	{
		if (gamePassesUI != null)
		{
			gamePassesUI.gameObject.SetActive(GamePassProgressionController.IsProgressionEnabled && GamePassesManager.GamePassesActive);
		}
		if (MVGameControllerBase.GoldRewardManager.CanGetGoldReward() && MVGameControllerBase.GoldRewardManager.GetGoldRewardTimeLeft() <= 0f)
		{
			GameObject claimGoldRewardPopup = Object.Instantiate(claimGoldRewardPopupPrefab);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(claimGoldRewardPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
		}
	}
}
