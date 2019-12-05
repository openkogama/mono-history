using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class AndroidInGameMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private TimedPlayReward playReward;

	[SerializeField]
	private GameObject winningConditionDebriefing;

	[SerializeField]
	private GamePassesUI gamePassesUIPrefab;

	[SerializeField]
	private FreeGoldAndroid rewardedAd;

	[SerializeField]
	private GameObject claimGoldRewardPopupPrefab;

	private GamePassesUI gamePassesUI;

	public void Initialize()
	{
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		bool flag = MVGameControllerBase.EditModeUI == null && !isTouristSession && MVGameControllerBase.GameMode == MVGameMode.Play;
		playReward.gameObject.SetActive(value: false);
		bool active = MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion && !MVGameControllerBase.GameSessionData.IsPlayedFromPoki;
		touristRegisterButton.SetActive(active);
		if (flag)
		{
			playReward.Initialize();
		}
		rewardedAd.Initialize();
		gamePassesUI = Object.Instantiate(gamePassesUIPrefab);
		gamePassesUI.transform.SetParent(transform, worldPositionStays: false);
		gamePassesUI.Initialize();
		if (!GamePassProgressionController.IsProgressionEnabled || !GamePassesManager.GamePassesActive)
		{
			gamePassesUI.gameObject.SetActive(value: false);
		}
		winningConditionDebriefing.transform.SetAsLastSibling();
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
