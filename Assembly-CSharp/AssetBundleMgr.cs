using System.Collections.Generic;
using UnityEngine;

public class AssetBundleMgr
{
	private class AssetBundleRequestPool
	{
		private AssetBundleRequest[] requests;

		internal AssetBundleRequestPool(int poolSize)
		{
			requests = new AssetBundleRequest[poolSize];
		}

		internal void HandleRequest(ref Queue<AssetBundleRequest> requestQueue)
		{
			if (requestQueue.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < requests.Length; i++)
			{
				if (requests[i] == null || requests[i].IsDone)
				{
					requests[i] = requestQueue.Dequeue();
					requests[i].DoFetch();
					break;
				}
			}
		}
	}

	private Queue<AssetBundleRequest> requestQueue;

	private Queue<AssetBundleRequest> highPriorityRequestQueue;

	private Queue<AssetBundleRequest> retryQueue;

	private AssetBundleRequestPool requestPool;

	private AssetBundleCache cache;

	private string rootUrl;

	private int retryCount;

	private float retryInterval;

	private object queueLock = new object();

	private bool initialized;

	public string RootUrl => rootUrl;

	public bool Initialize(string rootUrl, int poolSize, int retryCount, float retryIntervalInSecs)
	{
		if (rootUrl.IndexOf("file://") != 0 && rootUrl.IndexOf("http://") != 0)
		{
			Debug.LogWarning((object)"AssetBundleMgr rootUrl not valid!");
			return false;
		}
		this.rootUrl = rootUrl;
		this.retryCount = retryCount;
		retryInterval = retryIntervalInSecs;
		requestQueue = new Queue<AssetBundleRequest>();
		highPriorityRequestQueue = new Queue<AssetBundleRequest>();
		retryQueue = new Queue<AssetBundleRequest>();
		requestPool = new AssetBundleRequestPool(poolSize);
		cache = new AssetBundleCache();
		initialized = true;
		return true;
	}

	public void RequestAssetBundle(string bundlePath, BundleDownloadedCallback bundleReceiveCallback, bool autoRetry, bool highPriority)
	{
		if (!initialized)
		{
			Debug.LogError((object)"Attempt to request AssetBundle, but AssetBundleMgr not initialized!");
			return;
		}
		lock (queueLock)
		{
			if (cache.IsBundleRequested(bundlePath))
			{
				cache.SubscribeToNode(bundlePath, bundleReceiveCallback);
				return;
			}
			cache.AddRequest(bundlePath);
			cache.SubscribeToNode(bundlePath, bundleReceiveCallback);
			AssetBundleRequest item = new AssetBundleRequest(bundlePath, bundleReceiveCallback, autoRetry, this);
			if (highPriority)
			{
				highPriorityRequestQueue.Enqueue(item);
			}
			else
			{
				requestQueue.Enqueue(item);
			}
		}
	}

	internal void UnsubscribeBundleCallback(string bundlePath, BundleDownloadedCallback bundleReceiveCallback)
	{
		cache.UnsubscribeFromNode(bundlePath, bundleReceiveCallback);
	}

	internal AssetBundle GetBundle(string bundlePath)
	{
		return cache.GetBundle(bundlePath);
	}

	internal void AddToRetryQueue(string bundlePath, BundleDownloadedCallback callback, int retryCounter)
	{
		lock (queueLock)
		{
			if (retryCounter < retryCount)
			{
				Debug.LogWarning((object)("AssetBundleRequest failed [" + bundlePath + "]: Retry no: " + (retryCounter + 1) + " scheduled"));
				AssetBundleRequest item = new AssetBundleRequest(bundlePath, callback, Time.time + retryInterval, retryCounter + 1, this);
				retryQueue.Enqueue(item);
			}
			else
			{
				Debug.Log((object)("Request for " + bundlePath + " failed permanently after " + retryCount + " retries..."));
				Debug.LogError((object)("Request for bundle failed permanently after " + retryCount + " retries..."));
				FinalizeRequest(bundlePath, null);
			}
		}
	}

	internal void FinalizeRequest(string bundlePath, AssetBundle assetBundle)
	{
		cache.UpdateCache(bundlePath, assetBundle);
	}

	public void Update()
	{
		if (!initialized)
		{
			return;
		}
		if (retryQueue.Count > 0 && Time.time > retryQueue.Peek().Timestamp)
		{
			lock (queueLock)
			{
				AssetBundleRequest item = retryQueue.Dequeue();
				requestQueue.Enqueue(item);
			}
		}
		if (highPriorityRequestQueue.Count > 0)
		{
			lock (queueLock)
			{
				requestPool.HandleRequest(ref highPriorityRequestQueue);
				return;
			}
		}
		if (requestQueue.Count > 0)
		{
			lock (queueLock)
			{
				requestPool.HandleRequest(ref requestQueue);
			}
		}
	}
}
