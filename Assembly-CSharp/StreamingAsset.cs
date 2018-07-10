using System;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public abstract class StreamingAsset<AssetType, PreviewType> : StreamingAsset where AssetType : UnityEngine.Object where PreviewType : UnityEngine.Object
{
	[Tooltip("If true, bundle will be cached in memory, and never unloaded. It will also require a unique bundle name. If false, bundle will be destroyed and resources freed on destruction.")]
	[SerializeField]
	private bool useCache = true;

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

	protected virtual void Start()
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

	protected void DownloadWhenPossible()
	{
		if (Urls.StreamingAssetUrlReady())
		{
			Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Remove(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(DownloadWhenPossible));
			if (useCache)
			{
				Download_Cached(url, onAssetSetAction);
			}
			else
			{
				Download_NonCached(url, onAssetSetAction);
			}
		}
		else
		{
			Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Combine(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(DownloadWhenPossible));
		}
	}

	protected override void OnDownloadFinished(WWW www)
	{
		if (string.IsNullOrEmpty(www.error))
		{
			Asset = StreamingAsset.UnpackBundle<AssetType>(www, !useCache);
		}
	}

	protected override void OnDestroy()
	{
		if (!useCache)
		{
			Resources.UnloadUnusedAssets();
		}
		base.OnDestroy();
	}

	public static implicit operator AssetType(StreamingAsset<AssetType, PreviewType> a)
	{
		return a.asset;
	}
}
public abstract class StreamingAsset : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
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

	public static AssetType UnpackBundle<AssetType>(WWW www, bool unloadBundle = false) where AssetType : UnityEngine.Object
	{
		AssetType[] array = www.assetBundle.LoadAllAssets<AssetType>();
		if (array.Length == 0)
		{
			Debug.LogError("Download failed. Asset is null after assignement. Most likely asset is of incompatible type.");
			return (AssetType)null;
		}
		if (array.Length > 1)
		{
			Debug.LogWarning(www.url + "\nThere are multiple objects in bundle. Only the first asset will be used, and the download will take longer.");
		}
		if (unloadBundle)
		{
			www.assetBundle.Unload(unloadAllLoadedObjects: false);
		}
		return array[0];
	}

	public static string DBUrlToServerUrl(string url)
	{
		int num = url.LastIndexOf('/');
		string text = url.Substring(0, num);
		string text2 = url.Substring(num, url.Length - num).ToLowerInvariant();
		return text + text2;
	}

	protected void Download_Cached(string url, UnityAction onAssetSetAction)
	{
		url += MVGameControllerBase.KoGaMaSettings.WebCacheInvalidationCodeStr;
		this.onAssetSetAction = (UnityAction)Delegate.Combine(this.onAssetSetAction, new UnityAction(OnAssetSet));
		AsyncWWWManager.WWWRequest(new CachedGetRequest(AssetBundleUrl + url, OnDownloadFinished, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	protected void Download_NonCached(string url, UnityAction onAssetSetAction)
	{
		url += MVGameControllerBase.KoGaMaSettings.WebCacheInvalidationCodeStr;
		this.onAssetSetAction = (UnityAction)Delegate.Combine(this.onAssetSetAction, new UnityAction(OnAssetSet));
		AsyncWWWManager.WWWRequest(new GetRequest(AssetBundleUrl + url, OnDownloadFinished, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	protected abstract void OnDownloadFinished(WWW www);

	protected virtual void OnDestroy()
	{
		onAssetSetAction = (UnityAction)Delegate.Remove(onAssetSetAction, new UnityAction(OnAssetSet));
		AsyncWWWManager.UnsubscribeWWWRequest(OnDownloadFinished);
	}
}
