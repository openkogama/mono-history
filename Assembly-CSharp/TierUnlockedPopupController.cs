using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TierUnlockedPopupController : MonoBehaviour
{
	[SerializeField]
	private Image Background;

	[SerializeField]
	private TierUnlockedPopupContentTierUnlocked PopupContentTierUnlockedPrefab;

	[SerializeField]
	private TierUnlockedPopupContentXP PopupContentXPPrefab;

	[SerializeField]
	private TierUnlockedPopupContentCreatorSupport PopupContentCreatorSupportPrefab;

	[SerializeField]
	private float fadeDuration;

	[SerializeField]
	private float bounceEffectDuration;

	[SerializeField]
	private AnimationCurve bounceEffect;

	[SerializeField]
	private AnimationCurve fadeEffect;

	private List<TierUnlockedPopupContentBase> popupContentList;

	private int currentContentBeingShowed;

	private GamePassTier unlockedTier;

	private bool isPoppingCountdownStarted;

	private float popTime;

	private Color interpolateToColor;

	private float interpolateColorStartTime;

	private float bounceEffectStartTime;

	private float fadeEffectStartTime;

	public void Initialize(GamePassTier unlockedTier, bool wasPurchased)
	{
		this.unlockedTier = unlockedTier;
		popupContentList = new List<TierUnlockedPopupContentBase>();
		TierUnlockedPopupContentTierUnlocked tierUnlockedPopupContentTierUnlocked = Object.Instantiate(PopupContentTierUnlockedPrefab);
		tierUnlockedPopupContentTierUnlocked.transform.SetParent(transform, worldPositionStays: false);
		popupContentList.Add(tierUnlockedPopupContentTierUnlocked);
		TierUnlockedPopupContentXP tierUnlockedPopupContentXP = Object.Instantiate(PopupContentXPPrefab);
		tierUnlockedPopupContentXP.transform.SetParent(transform, worldPositionStays: false);
		tierUnlockedPopupContentXP.gameObject.SetActive(value: false);
		popupContentList.Add(tierUnlockedPopupContentXP);
		if (wasPurchased)
		{
			TierUnlockedPopupContentCreatorSupport tierUnlockedPopupContentCreatorSupport = Object.Instantiate(PopupContentCreatorSupportPrefab);
			tierUnlockedPopupContentCreatorSupport.transform.SetParent(transform, worldPositionStays: false);
			tierUnlockedPopupContentCreatorSupport.gameObject.SetActive(value: false);
			popupContentList.Add(tierUnlockedPopupContentCreatorSupport);
		}
		StartNewPopupContent(0);
		Background.color = popupContentList[0].BackgroundColor;
	}

	private void Update()
	{
		if (isPoppingCountdownStarted && Time.time >= popTime)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
		if (currentContentBeingShowed < popupContentList.Count)
		{
			if (currentContentBeingShowed > 0)
			{
				float t = Time.time - interpolateColorStartTime;
				Background.color = Color.Lerp(popupContentList[currentContentBeingShowed - 1].BackgroundColor, popupContentList[currentContentBeingShowed].BackgroundColor, t);
			}
			float newScale = bounceEffect.Evaluate((Time.time - bounceEffectStartTime) / bounceEffectDuration);
			popupContentList[currentContentBeingShowed].UpdateScale(newScale);
			float newAlpha = fadeEffect.Evaluate((Time.time - fadeEffectStartTime) / fadeDuration);
			popupContentList[currentContentBeingShowed].UpdateAlpha(newAlpha);
		}
	}

	private void OnStartingToDissappear()
	{
		currentContentBeingShowed++;
		if (currentContentBeingShowed < popupContentList.Count)
		{
			StartNewPopupContent(currentContentBeingShowed);
			return;
		}
		isPoppingCountdownStarted = true;
		popTime = Time.time + fadeDuration;
	}

	private void StartNewPopupContent(int index)
	{
		popupContentList[index].gameObject.SetActive(value: true);
		popupContentList[index].Initialize(unlockedTier, OnStartingToDissappear);
		interpolateColorStartTime = Time.time;
		bounceEffectStartTime = Time.time;
		fadeEffectStartTime = Time.time + (popupContentList[index].DisplayTime - fadeDuration);
		float newScale = bounceEffect.Evaluate((Time.time - bounceEffectStartTime) / bounceEffectDuration);
		popupContentList[currentContentBeingShowed].UpdateScale(newScale);
		float newAlpha = fadeEffect.Evaluate((Time.time - fadeEffectStartTime) / fadeDuration);
		popupContentList[currentContentBeingShowed].UpdateAlpha(newAlpha);
	}
}
