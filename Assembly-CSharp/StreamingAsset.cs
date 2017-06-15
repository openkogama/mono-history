using System;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public abstract class StreamingAsset<AssetType, PreviewType> : StreamingAsset where AssetType : UnityEngine.Object where PreviewType : UnityEngine.Object
{
	private AssetType asset;

	public AssetType Asset
	{
		get
		{
			return asset;
		}
		set
		{
			asset = value;
			if (onAssetSetAction != null)
			{
				onAssetSetAction();
			}
		}
	}

	protected void Start()
	{
		if (string.IsNullOrEmpty(url))
		{
			Debug.LogError("StreamedAsset is missing a reference.");
		}
		else
		{
			DownloadWhenPossible();
		}
	}

	private void DownloadWhenPossible()
	{
		if (Urls.StreamingAssetUrlReady())
		{
			Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Remove(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(DownloadWhenPossible));
			Download(url, onAssetSetAction);
		}
		else
		{
			Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Combine(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(DownloadWhenPossible));
		}
	}

	protected override void OnDownloadFinished(WWW www)
	{
		Debug.Log("StreamingAsset - Download finished:\n" + www.url);
		string error = www.error;
		if (string.IsNullOrEmpty(error))
		{
			Asset = StreamingAsset.UnpackBundle<AssetType>(www);
			if (Asset == null)
			{
				Debug.LogError("Download failed. Asset is null after assignement. Most likely asset is of incompatible type.");
			}
		}
		else
		{
			Debug.LogError(error + "\nDownload failed. " + www.url);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public static implicit operator AssetType(StreamingAsset<AssetType, PreviewType> a)
	{
		return a.asset;
	}
}
public abstract class StreamingAsset : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	protected string url = "NOT SET";

	protected UnityAction onAssetSetAction;

	private static string assetBundleUrl;

	public string Url
	{
		get
		{
			return url;
		}
		set
		{
			url = value;
		}
	}

	public static string AssetBundleUrl
	{
		get
		{
			if (assetBundleUrl == null)
			{
				assetBundleUrl = Urls.StreamingAssets + "AssetBundles/";
			}
			return assetBundleUrl;
		}
	}

	protected abstract void OnAssetSet();

	public static AssetType UnpackBundle<AssetType>(WWW www) where AssetType : UnityEngine.Object
	{
		AssetType[] array = www.assetBundle.LoadAllAssets<AssetType>();
		return array[0];
	}

	public static string DBUrlToServerUrl(string url)
	{
		int num = url.LastIndexOf('/');
		string text = url.Substring(0, num);
		string text2 = url.Substring(num, url.Length - num).ToLowerInvariant();
		return text + text2;
	}

	protected void Download(string url, UnityAction onAssetSetAction)
	{
		url += MVGameControllerBase.KoGaMaSettings.WebCacheInvalidationCodeStr;
		Debug.Log("StreamingAsset - download started:\n" + AssetBundleUrl + url);
		this.onAssetSetAction = (UnityAction)Delegate.Combine(this.onAssetSetAction, new UnityAction(OnAssetSet));
		AsyncWWWManager.WWWRequest(new CachedGetRequest(AssetBundleUrl + url, OnDownloadFinished, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	protected abstract void OnDownloadFinished(WWW www);

	protected virtual void OnDestroy()
	{
		AsyncWWWManager.UnsubscribeWWWRequest(OnDownloadFinished);
	}
}
