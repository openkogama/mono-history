using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public abstract class StreamingAsset<AssetType, PreviewType> : StreamingAsset where AssetType : UnityEngine.Object where PreviewType : UnityEngine.Object
{
	[SerializeField]
	[Tooltip("If true, bundle will be cached in memory, and never unloaded. It will also require a unique bundle name. If false, bundle will be destroyed and resources freed on destruction.")]
	protected bool useCache = true;

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
		if (www != null && string.IsNullOrEmpty(www.error))
		{
			if (!useCache)
			{
				Asset = StreamingAsset.UnpackBundle_NonCached<AssetType>(www, this);
			}
			else
			{
				Asset = StreamingAsset.UnpackBundle_Cached<AssetType>(www);
			}
		}
	}

	public static implicit operator AssetType(StreamingAsset<AssetType, PreviewType> a)
	{
		return a.asset;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
}
public abstract class StreamingAsset : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	protected string url = "NOT SET";

	protected UnityAction onAssetSetAction;

	private static HashSet<WWW> cachedAssetBundles = new HashSet<WWW>();

	private static string assetBundleUrl = null;

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

	public static void ClearCache()
	{
		foreach (WWW cachedAssetBundle in cachedAssetBundles)
		{
			try
			{
				cachedAssetBundle.assetBundle.Unload(unloadAllLoadedObjects: true);
			}
			catch (Exception ex)
			{
				Debug.LogError("StreamingAsset bundle unload failed: " + ex.Message);
			}
		}
		cachedAssetBundles.Clear();
		cachedAssetBundles.TrimExcess();
	}

	private static AssetType UnpackBundle<AssetType>(WWW www) where AssetType : UnityEngine.Object
	{
		AssetType[] array = www.assetBundle.LoadAllAssets<AssetType>();
		if (array.Length == 0)
		{
			Debug.LogError("Download failed. Asset is null after assignment. Most likely asset is of incompatible type.");
			return (AssetType)null;
		}
		if (array.Length > 1)
		{
			Debug.LogWarning(www.url + "\nThere are multiple objects in bundle. Only the first asset will be used, and the download will take longer.");
		}
		return array[0];
	}

	public static AssetType UnpackBundle_Cached<AssetType>(WWW www) where AssetType : UnityEngine.Object
	{
		string text = www.url;
		AssetType result = UnpackBundle<AssetType>(www);
		cachedAssetBundles.Add(www);
		return result;
	}

	protected static AssetType UnpackBundle_NonCached<AssetType>(WWW www, MonoBehaviour coroutineHost) where AssetType : UnityEngine.Object
	{
		AssetType result = UnpackBundle<AssetType>(www);
		coroutineHost.StartCoroutine(DelayedUnload(www));
		return result;
	}

	protected static IEnumerator DelayedUnload(WWW www)
	{
		yield return null;
		www.assetBundle.Unload(unloadAllLoadedObjects: false);
		Resources.UnloadUnusedAssets();
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
		this.onAssetSetAction = (UnityAction)Delegate.Combine(this.onAssetSetAction, new UnityAction(OnAssetSet));
		AsyncWWWManager.WWWRequest(new CachedAssetBundleRequest(AssetBundleUrl + url, OnDownloadFinished, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	protected void Download_NonCached(string url, UnityAction onAssetSetAction)
	{
		this.onAssetSetAction = (UnityAction)Delegate.Combine(this.onAssetSetAction, new UnityAction(OnAssetSet));
		AsyncWWWManager.WWWRequest(new AssetBundleRequest(AssetBundleUrl + url, OnDownloadFinished, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	protected abstract void OnDownloadFinished(WWW www);

	protected virtual void OnDestroy()
	{
		onAssetSetAction = (UnityAction)Delegate.Remove(onAssetSetAction, new UnityAction(OnAssetSet));
		AsyncWWWManager.UnsubscribeWWWRequest(OnDownloadFinished);
	}
}
