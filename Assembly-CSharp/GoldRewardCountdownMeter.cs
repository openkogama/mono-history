using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GoldRewardCountdownMeter : MonoBehaviour
{
	[SerializeField]
	private GameObject goldRewardCountdownUI;

	[SerializeField]
	private ProgressBar countdownProgressBar;

	[SerializeField]
	private Text countdownText;

	[SerializeField]
	private GameObject goldRewardClaimableUI;

	[SerializeField]
	private GamePassesTextBubble tipBubble;

	[SerializeField]
	private GameObject claimGoldRewardPopupPrefab;

	private bool isDone;

	private void Start()
	{
		UpdateCountdownVisibility();
		tipBubble.Activate(TM._("Claim gold when countdown complete"));
	}

	private void OnEnable()
	{
		UpdateCountdownVisibility();
	}

	private void Update()
	{
		if (IsGoldRewardCountdownActive())
		{
			UpdateCountDownProgress();
		}
		else
		{
			UpdateCountdownVisibility();
		}
	}

	private void UpdateCountdownVisibility()
	{
		if (goldRewardCountdownUI.activeSelf != IsGoldRewardCountdownActive())
		{
			goldRewardCountdownUI.SetActive(IsGoldRewardCountdownActive());
		}
	}

	private bool IsGoldRewardCountdownActive()
	{
		return MVGameControllerBase.GoldRewardManager.CanGetGoldReward() && MVGameControllerBase.GoldRewardManager.IsCountingDownGoldReward && !MVGameControllerBase.GoldRewardManager.IsGoldRewardDone;
	}

	private void UpdateCountDownProgress()
	{
		float goldRewardCountdownProgressPercentage = MVGameControllerBase.GoldRewardManager.GetGoldRewardCountdownProgressPercentage();
		countdownProgressBar.Progress = goldRewardCountdownProgressPercentage;
		float goldRewardTimeLeft = MVGameControllerBase.GoldRewardManager.GetGoldRewardTimeLeft();
		if (goldRewardTimeLeft > 0f)
		{
			countdownText.text = (Mathf.FloorToInt(MVGameControllerBase.GoldRewardManager.GetGoldRewardTimeLeft()) + 1).ToString();
			return;
		}
		countdownText.text = GetClaimText();
		if (!goldRewardClaimableUI.activeSelf)
		{
			goldRewardClaimableUI.SetActive(value: true);
		}
		if (!isDone)
		{
			tipBubble.Activate(TM._("Go to menu to claim"));
		}
		isDone = true;
	}

	private string GetClaimText()
	{
		return TM._("CLAIM");
	}

	public void TryShowClaimGoldRewardPopup()
	{
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
