using UnityEngine;
using UnityEngine.UI;

public class GamePointGainEffectCountController : MonoBehaviour
{
	private enum SlideState
	{
		Active,
		InActive,
		Showing,
		Hiding
	}

	[SerializeField]
	private Text gamePointAmount;

	[SerializeField]
	private Transform transformToSlide;

	[SerializeField]
	private float slideMoveAmount;

	[SerializeField]
	private float slideDuration;

	[SerializeField]
	private float slideOutDelay;

	[SerializeField]
	private AnimationCurve textEffectCurve;

	private int startValue;

	private int endValue;

	private int gainEffectAmountReached;

	private int gainEffectTotalAmount;

	private float textEffectStartTime;

	private int currentGamePointAmountDisplayed;

	private float originalXPosition;

	private float slideStartTime;

	private float startSlideOutTime;

	private SlideState currentSlideState = SlideState.InActive;

	public void SetupAmountCatchingUp(int endValue, int gainEffectTotalAmount)
	{
		this.endValue = endValue;
		this.gainEffectTotalAmount = gainEffectTotalAmount;
		startValue = currentGamePointAmountDisplayed;
		gainEffectAmountReached = 0;
	}

	public void OnGamePointGainEffectStarted()
	{
		startSlideOutTime = Time.time + slideOutDelay;
		if (currentSlideState == SlideState.InActive || currentSlideState == SlideState.Hiding)
		{
			currentSlideState = SlideState.Showing;
			slideStartTime = Time.time;
		}
	}

	public void OnGamePointGainEffectReached()
	{
		textEffectStartTime = Time.time;
		gainEffectAmountReached++;
		float num = (float)gainEffectAmountReached / (float)gainEffectTotalAmount;
		currentGamePointAmountDisplayed = Mathf.FloorToInt((float)startValue + num * (float)(endValue - startValue));
		gamePointAmount.text = currentGamePointAmountDisplayed.ToString();
	}

	public void UpdateGamePointAmountTextToValue(int updateToValue)
	{
		currentGamePointAmountDisplayed = updateToValue;
		gamePointAmount.text = currentGamePointAmountDisplayed.ToString();
	}

	public void HideCount()
	{
		transformToSlide.localPosition = new Vector3(originalXPosition + slideMoveAmount, transformToSlide.localPosition.y, transformToSlide.localPosition.z);
		currentSlideState = SlideState.InActive;
		slideStartTime = 0f;
		startSlideOutTime = 0f;
	}

	private void Start()
	{
		if (GamePassesManager.GamePassesActive)
		{
			UpdateGamePointAmountTextToValue(GamePassesManager.PlayerPlanetData.highScoreGamePoints);
		}
		originalXPosition = transformToSlide.localPosition.x;
		transformToSlide.localPosition = new Vector3(originalXPosition + slideMoveAmount, transformToSlide.localPosition.y, transformToSlide.localPosition.z);
	}

	private void Update()
	{
		UpdateGamePointAmountTextEffect();
		HandleSlideState();
	}

	private void UpdateGamePointAmountTextEffect()
	{
		float time = Time.time - textEffectStartTime;
		float num = textEffectCurve.Evaluate(time);
		Vector3 localScale = new Vector3(num, num, gamePointAmount.transform.localScale.z);
		gamePointAmount.transform.localScale = localScale;
	}

	private void HandleSlideState()
	{
		switch (currentSlideState)
		{
		case SlideState.Showing:
			HandleSlideIn();
			break;
		case SlideState.Hiding:
			HandleSlideOut();
			break;
		case SlideState.Active:
			if (Time.time > startSlideOutTime)
			{
				currentSlideState = SlideState.Hiding;
				slideStartTime = Time.time;
			}
			break;
		case SlideState.InActive:
			break;
		}
	}

	private void HandleSlideIn()
	{
		float num = (Time.time - slideStartTime) / slideDuration;
		float b = originalXPosition;
		float x = Mathf.Lerp(transformToSlide.localPosition.x, b, num);
		transformToSlide.localPosition = new Vector3(x, transformToSlide.localPosition.y, transformToSlide.localPosition.z);
		if (num > 1f)
		{
			currentSlideState = SlideState.Active;
		}
	}

	private void HandleSlideOut()
	{
		float num = (Time.time - slideStartTime) / slideDuration;
		float b = originalXPosition + slideMoveAmount;
		float x = Mathf.Lerp(transformToSlide.localPosition.x, b, num);
		transformToSlide.localPosition = new Vector3(x, transformToSlide.localPosition.y, transformToSlide.localPosition.z);
		if (num > 1f)
		{
			currentSlideState = SlideState.InActive;
		}
	}
}
