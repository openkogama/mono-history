using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject.HighlightSystem;
using MV.WorldObject.HighlightSystem.HighlightPayloads;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BundleTab : TabMenuButtonBase
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private float offsetX;

	[SerializeField]
	private float lerpTime = 1f;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private RawImage levelBadge;

	[SerializeField]
	private Text timeLimitText;

	[SerializeField]
	private GameObject redDot;

	[SerializeField]
	private Text redDotCount;

	private Texture2D badgeTextureAsset;

	private float startPos = 200f;

	private float startTime;

	private int highlightId = -1;

	public override void Initialize(int tabId, string categoryName)
	{
		startPos = rectTransform.anchoredPosition.x;
		List<Highlight<HighlightAccessoryBundle>> highLights = HighlightManager.GetHighLights<HighlightAccessoryBundle>(HighlightType.AccessoryBundle);
		AccessoryBundleClient accessoryBundleClient = AccessoryDataManager.AccessoryBundleClient;
		for (int i = 0; i < highLights.Count; i++)
		{
			if (highLights[i].highlightData.bundleId == accessoryBundleClient.accessoryBundleID)
			{
				redDot.SetActive(value: true);
				redDotCount.text = accessoryBundleClient.accessoryBundleItems.Count.ToString();
				highlightId = highLights[i].id;
				break;
			}
		}
		button.onClick.AddListener(() =>
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (ITabSelected x, BaseEventData y) =>
			{
				x.TabSelected(tabId);
			});
		});
		if (AccessoryDataManager.AccessoryBundleClient.timelimit.IsTimeLimited)
		{
			TimeSpan timeLeft = AccessoryDataManager.AccessoryBundleClient.timelimit.GetTimeLeft();
			timeLimitText.text = $"{timeLeft.Days}d {timeLeft.Hours}h";
		}
		int level = AccessoryDataManager.AccessoryBundleClient.level;
		if (level > 0)
		{
			if (LevelingManager.IsInitialized)
			{
				SetLevelBadge();
			}
			else
			{
				LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Combine(LevelingManager.OnLevelingInitialized, new UnityAction(SetLevelBadge));
			}
		}
	}

	private void SetLevelBadge()
	{
		LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Remove(LevelingManager.OnLevelingInitialized, new UnityAction(SetLevelBadge));
		BadgeManager.GetBadgeTexture(AccessoryDataManager.AccessoryBundleClient.level, OnBadgeLoaded);
	}

	private void OnDestroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(OnBadgeLoaded);
		badgeTextureAsset = null;
	}

	private void OnBadgeLoaded(UnityWebRequest www)
	{
		if (!(levelBadge == null))
		{
			badgeTextureAsset = DownloadHandlerTexture.GetContent(www);
			if (badgeTextureAsset != null)
			{
				levelBadge.texture = badgeTextureAsset;
				levelBadge.gameObject.SetActive(value: true);
			}
		}
	}

	public override void SetAsSelected()
	{
		icon.color = Styles.GetColor(ColorStyle.SelectedTab);
		if (redDot.activeInHierarchy)
		{
			redDot.SetActive(value: false);
			HighlightManager.SetHighlightToSeen(highlightId);
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
		{
			x.OpenCategoryScreen(canSortByInventory: false);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
		{
			x.DisplayCategoryFeatures(AccessoryCategoryClient.Bundles);
		});
		gameObject.SetActive(value: true);
		StopAllCoroutines();
		StartCoroutine(LerpToSize(rectTransform.rect.width + offsetX));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IBundleController x, BaseEventData y) =>
		{
			x.ShowBundle();
		});
	}

	public override void SetAsDeselected()
	{
		icon.color = Styles.GetColor(ColorStyle.OffWhite);
		gameObject.SetActive(value: true);
		StopAllCoroutines();
		StartCoroutine(LerpToSize(startPos));
	}

	private void OnDisable()
	{
		Vector2 anchoredPosition = rectTransform.anchoredPosition;
		anchoredPosition.x = startPos;
		rectTransform.anchoredPosition = anchoredPosition;
	}

	private IEnumerator LerpToSize(float size)
	{
		startTime = Time.time;
		Vector2 pos = rectTransform.anchoredPosition;
		float xPos = pos.x;
		while (Time.time - startTime < lerpTime)
		{
			pos.x = Mathf.Lerp(xPos, size, (Time.time - startTime) / lerpTime);
			rectTransform.anchoredPosition = pos;
			yield return 0;
		}
		pos.x = Mathf.Lerp(xPos, size, 1f);
		rectTransform.anchoredPosition = pos;
	}
}
