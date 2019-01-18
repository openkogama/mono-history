using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubscriberJoinedNotification : Notification
{
	private enum subscriberJoinedStateType : byte
	{
		NotInitializedState,
		WaitBeforeStartState,
		StartState,
		HighlightState,
		ClosingDownState,
		EndState
	}

	[SerializeField]
	private Text userNameText;

	[SerializeField]
	private Text boostAmountText;

	[SerializeField]
	private RectTransform backGround;

	[SerializeField]
	private CanvasGroup backgroundCanvasGroup;

	[SerializeField]
	private GameObject content;

	[SerializeField]
	private AnimationCurve updateBoostTextEffectCurve;

	[SerializeField]
	private AnimationCurve fadeOutCurve;

	[SerializeField]
	private AnimationCurve closeShrinkYEffectCurve;

	[SerializeField]
	private AnimationCurve closeShrinkXEffectCurve;

	private const float waitBeforeStartDuration = 1f;

	private const float startDuration = 1.5f;

	private const float memberHighlightDuration = 2f;

	private const float closeDuration = 0.5f;

	private const float boostEffectOnStartDelay = 0.208334f;

	private float stateStartTime;

	private subscriberJoinedStateType currentState;

	private float backgroundOriginalWidth;

	private float backgroundOriginalHeigt;

	private Vector2 backgroundOriginalSizeDelta;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		int actorNr = (int)data[(byte)9];
		if (!MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(actorNr, out var player))
		{
			CloseNotification();
			return;
		}
		base.Initialize(data);
		userNameText.text = player.UserProfileData.UserName;
		backgroundOriginalWidth = backGround.rect.width;
		backgroundOriginalHeigt = backGround.rect.height;
		backgroundOriginalSizeDelta = backGround.sizeDelta;
		ChangeState(subscriberJoinedStateType.WaitBeforeStartState);
	}

	protected override void Update()
	{
		HandleState();
	}

	private void OnDisable()
	{
		if (pool != null)
		{
			CloseNotification();
		}
	}

	private void CloseNotification()
	{
		ResetNotificationUI();
		if (gameObject.activeSelf)
		{
			gameObject.SetActive(value: false);
		}
		pool.Return(this);
	}

	private void ResetNotificationUI()
	{
		backGround.sizeDelta = backgroundOriginalSizeDelta;
		currentState = subscriberJoinedStateType.NotInitializedState;
		backgroundCanvasGroup.alpha = 0f;
		content.SetActive(value: false);
	}

	private void ChangeState(subscriberJoinedStateType newState)
	{
		currentState = newState;
		stateStartTime = Time.time;
	}

	private void HandleState()
	{
		float currentStateTime = Time.time - stateStartTime;
		switch (currentState)
		{
		case subscriberJoinedStateType.WaitBeforeStartState:
			HandleWaitBeforeStart(currentStateTime);
			break;
		case subscriberJoinedStateType.StartState:
			HandleStartState(currentStateTime);
			break;
		case subscriberJoinedStateType.HighlightState:
			HandleHighlightState(currentStateTime);
			break;
		case subscriberJoinedStateType.ClosingDownState:
			HandleClosingDownState(currentStateTime);
			break;
		case subscriberJoinedStateType.NotInitializedState:
		case subscriberJoinedStateType.EndState:
			break;
		}
	}

	private void HandleWaitBeforeStart(float currentStateTime)
	{
		if (content.activeSelf)
		{
			content.SetActive(value: false);
		}
		if (currentStateTime > 1f)
		{
			ChangeState(subscriberJoinedStateType.StartState);
			content.SetActive(value: true);
			Update();
		}
	}

	private void HandleStartState(float currentStateTime)
	{
		UpdateBoostTextEffect(currentStateTime - 0.208334f);
		FadeOut(0.5f - currentStateTime);
		CloseNotificationAnimation(0.5f - currentStateTime);
		if (currentStateTime > 1.5f)
		{
			ChangeState(subscriberJoinedStateType.HighlightState);
		}
	}

	private void HandleHighlightState(float currentStateTime)
	{
		if (currentStateTime > 2f)
		{
			ChangeState(subscriberJoinedStateType.ClosingDownState);
		}
	}

	private void HandleClosingDownState(float currentStateTime)
	{
		UpdateBoostTextEffect(currentStateTime);
		FadeOut(currentStateTime);
		CloseNotificationAnimation(currentStateTime);
		if (currentStateTime > 0.5f)
		{
			CloseNotification();
		}
	}

	private void UpdateBoostTextEffect(float stateProgress)
	{
		float num = updateBoostTextEffectCurve.Evaluate(stateProgress);
		Vector3 localScale = new Vector3(num, num, boostAmountText.transform.localScale.z);
		boostAmountText.transform.localScale = localScale;
	}

	private void FadeOut(float stateProgress)
	{
		backgroundCanvasGroup.alpha = fadeOutCurve.Evaluate(stateProgress);
	}

	private void CloseNotificationAnimation(float stateProgress)
	{
		Vector2 sizeDelta = new Vector2
		{
			x = closeShrinkXEffectCurve.Evaluate(stateProgress) * backgroundOriginalWidth - backgroundOriginalWidth,
			y = closeShrinkYEffectCurve.Evaluate(stateProgress) * backgroundOriginalHeigt - backgroundOriginalHeigt
		};
		sizeDelta += backgroundOriginalSizeDelta;
		backGround.sizeDelta = sizeDelta;
	}
}
