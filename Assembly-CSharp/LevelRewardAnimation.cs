using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelRewardAnimation : MonoBehaviour
{
	[SerializeField]
	private RawImage prevLevelBadge;

	[SerializeField]
	private float prevLevelDisplayTime;

	[SerializeField]
	private AnimationCurve prevBadgeBounceEffect;

	[SerializeField]
	private RawImage nextLevelBadge;

	[SerializeField]
	private float nextLevelDisplayTime;

	[SerializeField]
	private AnimationCurve nextBadgeBounceEffect;

	[SerializeField]
	private Image goldImage;

	[SerializeField]
	private float goldImageDisplayTime;

	[SerializeField]
	private AnimationCurve goldBounceEffect;

	[SerializeField]
	private AnimationCurve goldFadeInCurve;

	[SerializeField]
	private AnimationCurve rotateUIYAxisIn;

	[SerializeField]
	private AnimationCurve rotateUIYAxisOut;

	[SerializeField]
	private float rotateUIYAxisTime;

	[SerializeField]
	private Text header;

	[SerializeField]
	private Text goldText;

	[SerializeField]
	private CanvasGroup claimButton;

	[SerializeField]
	private AnimationCurve backgroundRaySizeCurve;

	[SerializeField]
	private Image backgroundRays;

	[SerializeField]
	private int targetSize;

	private List<KeyValuePair<int, int>> rewards = new List<KeyValuePair<int, int>>();

	private KeyValuePair<int, int> currentReward;

	private Texture2D previousBadgeTextureAsset;

	private Texture2D currentBadgeTextureAsset;

	public void Initialize(Dictionary<int, int> levelRewards)
	{
		foreach (KeyValuePair<int, int> levelReward in levelRewards)
		{
			rewards.Add(levelReward);
		}
		rewards = rewards.OrderByDescending((KeyValuePair<int, int> o) => o.Key).ToList();
		header.gameObject.SetActive(value: false);
		header.text = TM._("LEVEL UP!");
		prevLevelBadge.enabled = false;
		nextLevelBadge.enabled = false;
		goldImage.enabled = false;
		OnShow();
	}

	public void OnShow()
	{
		StopAllCoroutines();
		if (rewards.Count <= 0)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			return;
		}
		currentReward = rewards[rewards.Count - 1];
		rewards.RemoveAt(rewards.Count - 1);
		BadgeManager.GetBadgeTexture(currentReward.Key - 1, OnPrevBadgeLoaded);
		BadgeManager.GetBadgeTexture(currentReward.Key, OnNextBadgeLoaded);
		goldText.text = string.Format("{0} " + TM._("GOLD!"), currentReward.Value);
		StartCoroutine(DisplayAndFadePrevBadge());
	}

	private void OnDestroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(OnPrevBadgeLoaded);
		Object.Destroy(previousBadgeTextureAsset);
		BadgeManager.UnsubscribeGetBadgeRequest(OnNextBadgeLoaded);
		Object.Destroy(currentBadgeTextureAsset);
	}

	private void OnPrevBadgeLoaded(WWW www)
	{
		previousBadgeTextureAsset = www.texture;
		if (previousBadgeTextureAsset == null || !string.IsNullOrEmpty(www.error))
		{
			Debug.LogWarning("Error downloading prevLevel badge.");
		}
		prevLevelBadge.texture = previousBadgeTextureAsset;
	}

	private void OnNextBadgeLoaded(WWW www)
	{
		currentBadgeTextureAsset = www.texture;
		if (currentBadgeTextureAsset == null || !string.IsNullOrEmpty(www.error))
		{
			Debug.LogWarning("Error downloading nextLevel badge.");
		}
		nextLevelBadge.texture = currentBadgeTextureAsset;
	}

	private IEnumerator DisplayAndFadePrevBadge()
	{
		prevLevelBadge.rectTransform.sizeDelta = new Vector2(targetSize, targetSize);
		prevLevelBadge.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		prevLevelBadge.enabled = true;
		backgroundRays.transform.localScale = new Vector3(1f, 1f, 1f);
		backgroundRays.enabled = false;
		goldText.gameObject.SetActive(value: false);
		claimButton.gameObject.SetActive(value: false);
		goldImage.enabled = false;
		header.gameObject.SetActive(value: false);
		float currentTime = 0f;
		float scale = 0f;
		while (currentTime / prevLevelDisplayTime < 1f)
		{
			currentTime += Time.deltaTime;
			scale = prevBadgeBounceEffect.Evaluate(currentTime / prevLevelDisplayTime);
			prevLevelBadge.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale * (float)targetSize);
			prevLevelBadge.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale * (float)targetSize);
			yield return 0;
		}
		scale = prevBadgeBounceEffect.Evaluate(1f);
		prevLevelBadge.rectTransform.sizeDelta = new Vector2(scale * (float)targetSize, scale * (float)targetSize);
		currentTime = 0f;
		while (currentTime / rotateUIYAxisTime < 1f)
		{
			currentTime += Time.deltaTime;
			float rotation = rotateUIYAxisOut.Evaluate(currentTime / rotateUIYAxisTime) * 90f;
			prevLevelBadge.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
			yield return 0;
		}
		prevLevelBadge.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
		StartCoroutine(DisplayAndFadeNextBadge());
		yield return 0;
	}

	private IEnumerator DisplayAndFadeNextBadge()
	{
		nextLevelBadge.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
		nextLevelBadge.enabled = true;
		prevLevelBadge.enabled = false;
		float scale = nextBadgeBounceEffect.Evaluate(0f);
		nextLevelBadge.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale * (float)targetSize);
		nextLevelBadge.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale * (float)targetSize);
		float currentTime = 0f;
		while (currentTime / rotateUIYAxisTime < 1f)
		{
			currentTime += Time.deltaTime;
			float rotation = -90f + rotateUIYAxisIn.Evaluate(currentTime / rotateUIYAxisTime) * 90f;
			nextLevelBadge.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
			yield return 0;
		}
		backgroundRays.enabled = true;
		header.gameObject.SetActive(value: true);
		header.text = TM._("LEVEL UP!");
		nextLevelBadge.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		currentTime = 0f;
		while (currentTime / nextLevelDisplayTime < 1f)
		{
			currentTime += Time.deltaTime;
			scale = nextBadgeBounceEffect.Evaluate(currentTime / nextLevelDisplayTime);
			nextLevelBadge.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale * (float)targetSize);
			nextLevelBadge.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale * (float)targetSize);
			float backgroundRayScale = backgroundRaySizeCurve.Evaluate(currentTime / nextLevelDisplayTime);
			backgroundRays.transform.localScale = new Vector3(backgroundRayScale, backgroundRayScale, 1f);
			yield return 0;
		}
		scale = nextBadgeBounceEffect.Evaluate(1f);
		nextLevelBadge.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale * (float)targetSize);
		nextLevelBadge.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale * (float)targetSize);
		header.gameObject.SetActive(value: false);
		currentTime = 0f;
		while (currentTime / rotateUIYAxisTime < 1f)
		{
			currentTime += Time.deltaTime;
			float rotation2 = rotateUIYAxisOut.Evaluate(currentTime / rotateUIYAxisTime) * 90f;
			nextLevelBadge.transform.rotation = Quaternion.Euler(0f, rotation2, 0f);
			yield return 0;
		}
		nextLevelBadge.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
		StartCoroutine(DisplayAndFadeGoldIcon());
		yield return 0;
	}

	private IEnumerator DisplayAndFadeGoldIcon()
	{
		goldImage.enabled = true;
		nextLevelBadge.enabled = false;
		float scale = goldBounceEffect.Evaluate(0f);
		goldImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale * (float)targetSize);
		goldImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale * (float)targetSize);
		goldImage.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
		float currentTime = 0f;
		while (currentTime / rotateUIYAxisTime < 1f)
		{
			currentTime += Time.deltaTime;
			float rotation = -90f + rotateUIYAxisIn.Evaluate(currentTime / rotateUIYAxisTime) * 90f;
			goldImage.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
			yield return 0;
		}
		goldImage.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		header.gameObject.SetActive(value: true);
		header.text = TM._("REWARD!");
		goldText.gameObject.SetActive(value: true);
		claimButton.gameObject.SetActive(value: true);
		claimButton.alpha = 0f;
		currentTime = 0f;
		while (currentTime / goldImageDisplayTime < 1f)
		{
			currentTime += Time.deltaTime;
			scale = goldBounceEffect.Evaluate(currentTime / goldImageDisplayTime);
			goldImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale * (float)targetSize);
			goldImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale * (float)targetSize);
			claimButton.alpha = goldFadeInCurve.Evaluate(currentTime / goldImageDisplayTime);
			yield return 0;
		}
		scale = goldBounceEffect.Evaluate(1f);
		claimButton.alpha = 1f;
		goldImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scale * (float)targetSize);
		goldImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scale * (float)targetSize);
		yield return 0;
	}
}
