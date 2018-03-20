using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameCoinMenuController : MonoBehaviour
{
	[SerializeField]
	private GameCoinBoostShopDialog gameCoinBoostShopDialogPrefab;

	[SerializeField]
	private Button startStopBoostButton;

	[SerializeField]
	private Text boostLeftText;

	[SerializeField]
	private Image boostIndicator;

	[SerializeField]
	private Sprite pauseImage;

	[SerializeField]
	private Sprite playImage;

	private void Awake()
	{
		startStopBoostButton.onClick.AddListener(ToggleGameCoinBoostState);
		MVGameCoinManager gameCoinManager = MVGameControllerBase.Game.GameCoinManager;
		gameCoinManager.BoostStateChanged = (Action<int, bool>)Delegate.Combine(gameCoinManager.BoostStateChanged, new Action<int, bool>(OnBoostingChanged));
	}

	private void OnEnable()
	{
		OnBoostingChanged(MVGameControllerBase.Game.GameCoinManager.BoostLeft, MVGameControllerBase.Game.GameCoinManager.BoostEnabled);
	}

	private void Update()
	{
		if (boostLeftText.gameObject.activeInHierarchy)
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, MVGameControllerBase.Game.GameCoinManager.BoostLeft);
			string text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
			boostLeftText.text = text;
		}
	}

	private void ToggleGameCoinBoostState()
	{
		bool flag = !MVGameControllerBase.Game.GameCoinManager.BoostEnabled;
		if (!MVGameControllerBase.Game.GameCoinManager.Active && flag)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create("To use game coin boost go to a game where game coins are used", "No game coins");
			});
			return;
		}
		MVGameControllerBase.OperationRequests.SetGameCoinBoostState(flag);
		if (flag)
		{
			boostIndicator.sprite = pauseImage;
		}
		else
		{
			boostIndicator.sprite = playImage;
		}
	}

	private void OnBoostingChanged(int boostTimeLeft, bool boosting)
	{
		SetAllToNotVisible();
		if (boostTimeLeft != 0)
		{
			startStopBoostButton.gameObject.SetActive(value: true);
			if (boosting)
			{
				AwayMonitor.IdleKickEnabled = false;
			}
		}
	}

	private void SetAllToNotVisible()
	{
		startStopBoostButton.gameObject.SetActive(value: false);
	}
}
