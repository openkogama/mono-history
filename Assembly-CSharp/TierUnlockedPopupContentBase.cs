using MV.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TierUnlockedPopupContentBase : MonoBehaviour
{
	protected enum PopupState
	{
		notStarted,
		displaying,
		done
	}

	[SerializeField]
	protected Color backgroundColor;

	[SerializeField]
	protected float displayTime;

	[SerializeField]
	protected Text titleText;

	[SerializeField]
	protected NotificationFade titleTextFader;

	[SerializeField]
	protected RectTransform mainContent;

	[SerializeField]
	protected CanvasGroup mainContentCanvasGroup;

	private const float titleSlideAmount = 300f;

	private const float slideDuration = 0.2f;

	private float titleOriginalYPosition;

	private float slideInStartTime;

	private float slideOutStartTime;

	protected PopupState currentState;

	protected float stateStartTime;

	protected UnityAction onDisplayDoneCallback;

	public Color BackgroundColor => backgroundColor;

	public float DisplayTime => displayTime;

	public virtual void Initialize(GamePassTier unlockedGamePassTier, UnityAction onDisplayDoneCallback)
	{
		this.onDisplayDoneCallback = onDisplayDoneCallback;
		stateStartTime = Time.time;
		SetState(PopupState.displaying);
		titleOriginalYPosition = titleText.transform.localPosition.y;
		float y = titleOriginalYPosition - 300f;
		titleText.transform.localPosition = new Vector3(titleText.transform.localPosition.x, y, titleText.transform.localPosition.z);
		titleTextFader.Activate();
		titleTextFader.PauseAt(0.29f);
		slideInStartTime = Time.time;
		slideOutStartTime = Time.time + displayTime - 0.2f;
	}

	public void UpdateScale(float newScale)
	{
		mainContent.localScale = new Vector3(newScale, newScale, 1f);
	}

	public void UpdateAlpha(float newAlpha)
	{
		mainContentCanvasGroup.alpha = newAlpha;
	}

	protected virtual void Update()
	{
		switch (currentState)
		{
		case PopupState.displaying:
			HandleDisplaying();
			break;
		}
	}

	protected virtual void HandleDisplaying()
	{
		HandleSlideTitleText();
		float num = (Time.time - stateStartTime) * (1f / displayTime);
		if (num > 1f + Time.deltaTime)
		{
			onDisplayDoneCallback();
			SetState(PopupState.done);
		}
	}

	protected void SetState(PopupState newState)
	{
		if (currentState != newState)
		{
			currentState = newState;
			stateStartTime = Time.time;
		}
	}

	protected void HandleSlideTitleText()
	{
		float t = (Time.time - slideInStartTime) / 0.2f;
		float y = Mathf.Lerp(titleOriginalYPosition - 300f, titleOriginalYPosition, t);
		titleText.transform.localPosition = new Vector3(titleText.transform.localPosition.x, y, titleText.transform.localPosition.z);
		float num = (Time.time - slideOutStartTime) / 0.2f;
		if (num > 0f)
		{
			y = Mathf.Lerp(titleOriginalYPosition, titleOriginalYPosition + 300f, num);
			titleText.transform.localPosition = new Vector3(titleText.transform.localPosition.x, y, titleText.transform.localPosition.z);
		}
	}
}
