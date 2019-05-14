using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PurchasedAccessoryPreviewer : MonoBehaviour
{
	[SerializeField]
	private Image image;

	[SerializeField]
	private StreamedSpriteToImageManual imageLoader;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Image backgroundGlow;

	[SerializeField]
	private float imageDisplayTime;

	[SerializeField]
	private float imageBounceEffectTime;

	[SerializeField]
	private AnimationCurve bounceEffect;

	[SerializeField]
	private AnimationCurve fadeEffect;

	private float currentTime;

	private AccessoryDataClient[] previewData;

	private int currentStreamingAssetIndex;

	private Color targetColorBackground;

	private Color targetColorGlow;

	private int targetHeight;

	public void Initialize(AccessoryDataClient[] previewAccessories)
	{
		targetHeight = Screen.currentResolution.height;
		previewData = previewAccessories;
		RarityStylesDef rarityStylesDef = null;
		rarityStylesDef = ((previewData[currentStreamingAssetIndex].lvl == 0 || previewData[currentStreamingAssetIndex].cost != 0) ? Styles.GetAccessoryColorsFromPrice(previewData[currentStreamingAssetIndex].cost) : Styles.GetAccessoryColorsFromLevel(previewData[currentStreamingAssetIndex].lvl));
		targetColorBackground = rarityStylesDef.backgroundColor;
		targetColorGlow = rarityStylesDef.glowColor;
		string imageUrl = GetImageUrl(previewData[currentStreamingAssetIndex]);
		imageLoader.Download(imageUrl, OnShow);
	}

	private void OnShow()
	{
		StartCoroutine(DisplayAndFadeImages());
	}

	private string GetImageUrl(AccessoryDataClient accessoryDataClient)
	{
		string text = "AvatarAccessory/" + accessoryDataClient.cat.ToString() + "/Images/";
		string[] array = accessoryDataClient.url.Split(new string[1] { "/" }, StringSplitOptions.None);
		array = array[array.Length - 1].Split(new string[1] { "." }, StringSplitOptions.None);
		string text2 = array[0];
		text2 += "Image.unity3d";
		return text + text2.ToLower();
	}

	private IEnumerator DisplayAndFadeImages()
	{
		image.rectTransform.sizeDelta = new Vector2(0f, 0f);
		currentTime = 0f;
		while (currentTime / imageDisplayTime < 1f)
		{
			currentTime += Time.deltaTime;
			EvaluateImageAtTime(currentTime / imageBounceEffectTime, currentTime / imageDisplayTime);
			yield return 0;
		}
		EvaluateImageAtTime(1f, 1f);
		yield return 0;
		if (currentStreamingAssetIndex < previewData.Length - 1)
		{
			EvaluateImageAtTime(0f, 0f);
			currentStreamingAssetIndex++;
			RarityStylesDef rarityStylesDef = ((previewData[currentStreamingAssetIndex].lvl == 0 || previewData[currentStreamingAssetIndex].cost != 0) ? Styles.GetAccessoryColorsFromPrice(previewData[currentStreamingAssetIndex].cost) : Styles.GetAccessoryColorsFromLevel(previewData[currentStreamingAssetIndex].lvl));
			targetColorBackground = rarityStylesDef.backgroundColor;
			targetColorGlow = rarityStylesDef.glowColor;
			StartCoroutine(DisplayAndFadeImages());
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
		yield return 0;
	}

	private void EvaluateImageAtTime(float bounceTime, float colorTime)
	{
		float num = bounceEffect.Evaluate(bounceTime);
		image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * (float)targetHeight);
		image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * (float)targetHeight);
		background.CrossFadeColor(targetColorBackground, colorTime, ignoreTimeScale: false, useAlpha: false);
		backgroundGlow.CrossFadeColor(targetColorGlow, colorTime, ignoreTimeScale: false, useAlpha: false);
		Color color = image.color;
		color.a = fadeEffect.Evaluate(colorTime);
		image.color = color;
	}
}
