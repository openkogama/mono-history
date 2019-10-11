using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StreamedSpriteToImageManual : StreamingAsset<Sprite, Texture2D>
{
	private class StreamedAssetSpriteHandler
	{
		private static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

		public static Sprite GetSprite(string url)
		{
			if (sprites.TryGetValue(url, out var value))
			{
				return value;
			}
			return null;
		}

		public static void CacheSpriteUrl(string url, Sprite sprite)
		{
			sprites[url] = sprite;
		}
	}

	[Header("Dependencies")]
	[SerializeField]
	protected Image image;

	private UnityAction onAssetSetManual;

	protected override void Start()
	{
	}

	public void Download(string url, UnityAction onAssetSet)
	{
		onAssetSetManual = onAssetSet;
		base.url = url;
		onAssetSetAction = (UnityAction)Delegate.Combine(onAssetSetAction, new UnityAction(OnAssetSetCallback));
		DownloadWhenPossible();
	}

	protected override void OnDownloadFinished(UnityWebRequest www)
	{
		if (www != null && string.IsNullOrEmpty(www.error))
		{
			Asset = StreamingAsset.UnpackBundle_Cached<Sprite>(www);
			if (!useCache)
			{
				StartCoroutine("DelayedUnload", www);
			}
		}
	}

	private void OnAssetSetCallback()
	{
		onAssetSetAction = (UnityAction)Delegate.Remove(onAssetSetAction, new UnityAction(OnAssetSetCallback));
		if (onAssetSetManual != null)
		{
			onAssetSetManual();
		}
	}

	public void Reset()
	{
		if (image == null)
		{
			image = GetComponent<Image>();
		}
	}

	protected override void OnAssetSet()
	{
		image.sprite = Asset;
	}
}
