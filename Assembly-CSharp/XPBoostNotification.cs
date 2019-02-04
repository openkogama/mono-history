using System.Collections.Generic;
using MV.WorldObject.Subscription;
using MV.WorldObject.Subscription.SubscriptionRules;
using UnityEngine;
using UnityEngine.UI;

public class XPBoostNotification : Notification
{
	private enum XPBoostStateType : byte
	{
		NotInitializedState,
		WaitBeforeStartState,
		StartState,
		HighlightState,
		ClosingDownState,
		EndState
	}

	[SerializeField]
	private Text boostAmountText;

	[SerializeField]
	private Text UserNameText1;

	[SerializeField]
	private Text UserNameText2;

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

	[SerializeField]
	private AnimationCurve slideUserNameOutCurve;

	[SerializeField]
	private AnimationCurve slideUserNameInCurve;

	private const float waitBeforeStartDuration = 1f;

	private const float startDuration = 1.5f;

	private const float memberHighlightDuration = 2f;

	private const float closeDuration = 0.5f;

	private const float boostEffectOnStartDelay = 0.208334f;

	private float stateStartTime;

	private List<MVPlayer> memberList = new List<MVPlayer>();

	private int currentMemberBeingHighlighted = -1;

	private XPBoostStateType currentState;

	private float backgroundOriginalWidth;

	private float backgroundOriginalHeigt;

	private Vector2 backgroundOriginalSizeDelta;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		int actorNr = MVGameControllerBase.Game.LocalPlayer.ActorNr;
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			if (value != null && value.ActorNr != actorNr && MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(value.ActorNr, out var player) && player.SubscriptionRules.HasBenefit(SubscriptionBenefit.XPBoost))
			{
				memberList.Add(player);
			}
		}
		if (memberList.Count <= 0)
		{
			CloseNotification();
			return;
		}
		currentMemberBeingHighlighted = 0;
		UserNameText1.text = memberList[currentMemberBeingHighlighted].UserProfileData.UserName;
		timeSinceStart -= 1.5f + 2f * (float)memberList.Count + 0.5f;
		backgroundOriginalWidth = backGround.rect.width;
		backgroundOriginalHeigt = backGround.rect.height;
		backgroundOriginalSizeDelta = backGround.sizeDelta;
		UpdateBoostAmount();
		ChangeState(XPBoostStateType.WaitBeforeStartState);
	}

	protected override void Update()
	{
		HandleState();
	}

	private void CloseNotification()
	{
		if (gameObject.activeSelf)
		{
			gameObject.SetActive(value: false);
		}
		pool.Return(this);
	}

	private void UpdateBoostAmount()
	{
		int totalXPBoost = MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.GetRule<XpBooster>(SubscriptionBenefit.XPBoost).GetTotalXPBoost(currentMemberBeingHighlighted + 1);
		boostAmountText.text = "+" + totalXPBoost + "%  XP";
	}

	private void ChangeState(XPBoostStateType newState)
	{
		currentState = newState;
		stateStartTime = Time.time;
	}

	private void HandleState()
	{
		float currentStateTime = Time.time - stateStartTime;
		switch (currentState)
		{
		case XPBoostStateType.WaitBeforeStartState:
			HandleWaitBeforeStart(currentStateTime);
			break;
		case XPBoostStateType.StartState:
			HandleStartState(currentStateTime);
			break;
		case XPBoostStateType.HighlightState:
			HandleHighlightState(currentStateTime);
			break;
		case XPBoostStateType.ClosingDownState:
			HandleClosingDownState(currentStateTime);
			break;
		case XPBoostStateType.NotInitializedState:
		case XPBoostStateType.EndState:
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
			ChangeState(XPBoostStateType.StartState);
			content.SetActive(value: true);
			Update();
		}
	}

	private void HandleStartState(float currentStateTime)
	{
		UpdateBoostTextEffect(currentStateTime - 0.208334f);
		FadeOut(0.5f - currentStateTime);
		CloseNotification(0.5f - currentStateTime);
		if (currentStateTime > 1.5f)
		{
			StartNextState();
		}
	}

	private void StartNextState()
	{
		if (IsMemberHighlightingDone())
		{
			ChangeState(XPBoostStateType.ClosingDownState);
			return;
		}
		ChangeState(XPBoostStateType.HighlightState);
		StartNewMemberHighlight();
	}

	private void StartNewMemberHighlight()
	{
		currentMemberBeingHighlighted++;
		if (currentMemberBeingHighlighted % 2 == 0)
		{
			UserNameText1.text = memberList[currentMemberBeingHighlighted].UserProfileData.UserName;
		}
		else
		{
			UserNameText2.text = memberList[currentMemberBeingHighlighted].UserProfileData.UserName;
		}
		UpdateBoostAmount();
	}

	private bool IsMemberHighlightingDone()
	{
		return memberList.Count == currentMemberBeingHighlighted + 1;
	}

	private void HandleHighlightState(float currentStateTime)
	{
		if (currentMemberBeingHighlighted > 0)
		{
			UpdateBoostTextEffect(currentStateTime);
			SwapMemberName(currentStateTime);
		}
		if (currentStateTime > 2f)
		{
			StartNextState();
		}
	}

	private void HandleClosingDownState(float currentStateTime)
	{
		UpdateBoostTextEffect(currentStateTime);
		FadeOut(currentStateTime);
		CloseNotification(currentStateTime);
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

	private void CloseNotification(float stateProgress)
	{
		Vector2 sizeDelta = new Vector2
		{
			x = closeShrinkXEffectCurve.Evaluate(stateProgress) * backgroundOriginalWidth - backgroundOriginalWidth,
			y = closeShrinkYEffectCurve.Evaluate(stateProgress) * backgroundOriginalHeigt - backgroundOriginalHeigt
		};
		sizeDelta += backgroundOriginalSizeDelta;
		backGround.sizeDelta = sizeDelta;
	}

	private void SwapMemberName(float stateProgress)
	{
		SlideOutName(stateProgress);
		SlideInName(stateProgress);
	}

	private void SlideOutName(float stateProgress)
	{
		Vector3 localPosition = ((currentMemberBeingHighlighted % 2 != 0) ? UserNameText1.transform.localPosition : UserNameText2.transform.localPosition);
		localPosition.x = slideUserNameOutCurve.Evaluate(stateProgress);
		if (currentMemberBeingHighlighted % 2 == 0)
		{
			UserNameText2.transform.localPosition = localPosition;
		}
		else
		{
			UserNameText1.transform.localPosition = localPosition;
		}
	}

	private void SlideInName(float stateProgress)
	{
		Vector3 localPosition = ((currentMemberBeingHighlighted % 2 != 0) ? UserNameText2.transform.localPosition : UserNameText1.transform.localPosition);
		localPosition.x = slideUserNameInCurve.Evaluate(stateProgress);
		if (currentMemberBeingHighlighted % 2 == 0)
		{
			UserNameText1.transform.localPosition = localPosition;
		}
		else
		{
			UserNameText2.transform.localPosition = localPosition;
		}
	}
}
