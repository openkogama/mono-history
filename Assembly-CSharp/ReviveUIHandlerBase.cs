using System;
using Assets.Scripts.AdIntegration;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class ReviveUIHandlerBase : MonoBehaviour
{
	[SerializeField]
	protected Button continueButton;

	[SerializeField]
	private Image timerFill;

	[SerializeField]
	private Text timerText;

	[SerializeField]
	protected ContinueButtonLockCursor continuePopup;

	[SerializeField]
	protected RawImage targetTexture;

	[SerializeField]
	protected NotificationPopup errorNotification;

	private bool watchAdClicked;

	private float started;

	private float duration;

	protected abstract void OnRewardedAdWatched(RewardedAdResult result);

	protected abstract void OnAdFinishedContinue();

	public virtual void Initialize(UnityAction onContinueClicked)
	{
		continueButton.onClick.AddListener(onContinueClicked);
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnReviveTimeElapsed += ReviveTimeElapsed;
		duration = MVGameControllerBase.LocalPlayer.ReviveTimeout;
		started = Time.time;
	}

	private void ReviveTimeElapsed()
	{
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnReviveTimeElapsed -= ReviveTimeElapsed;
		if (!watchAdClicked)
		{
			OnRewardedAdWatched(RewardedAdResult.ErrorTimeout);
		}
	}

	protected virtual void Update()
	{
		if (Time.time - started < duration)
		{
			float num = 1f - (Time.time - started) / duration;
			timerFill.fillAmount = num;
			timerText.text = Mathf.FloorToInt(num * duration).ToString();
		}
		else
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.ReviveTimeElapsed();
			enabled = false;
			timerFill.fillAmount = 0f;
			timerText.text = "0";
		}
	}

	protected virtual void RoundEnded(IWinningCondition condition)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(RoundEnded));
	}

	protected virtual void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(RoundEnded));
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnReviveTimeElapsed -= ReviveTimeElapsed;
		}
	}

	public virtual void OnWatchAdClicked()
	{
		Debug.Log("REVIVE STARTED");
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(RoundEnded));
		watchAdClicked = true;
		MVGameControllerBase.AdManager.RequestRewardedAd(OnRewardedAdWatched, AdContext.Revive);
	}
}
