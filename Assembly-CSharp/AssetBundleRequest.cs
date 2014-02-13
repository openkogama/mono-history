using UnityEngine;

internal class AssetBundleRequest
{
	private GameObject gameObject;

	private AssetBundleFetcher fetcher;

	private string bundlePath;

	private BundleDownloadedCallback bundleReceiveCallback;

	private bool isDone;

	private bool autoRetry;

	private int retryCounter;

	private float timestamp;

	private AssetBundleMgr requestMgr;

	public bool IsDone => isDone;

	public float Timestamp => timestamp;

	public string BundlePath => bundlePath;

	internal AssetBundleRequest(string bundlePath, BundleDownloadedCallback bundleReceiveCallback, bool autoRetry, AssetBundleMgr mgr)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		gameObject = new GameObject();
		fetcher = gameObject.AddComponent<AssetBundleFetcher>();
		((Object)fetcher).name = "Asset Bundle Fetcher";
		this.bundlePath = bundlePath;
		this.bundleReceiveCallback = bundleReceiveCallback;
		this.autoRetry = autoRetry;
		requestMgr = mgr;
		timestamp = Time.time;
	}

	internal AssetBundleRequest(string bundlePath, BundleDownloadedCallback bundleReceiveCallback, float time, int retryCounter, AssetBundleMgr mgr)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		gameObject = new GameObject();
		fetcher = gameObject.AddComponent<AssetBundleFetcher>();
		((Object)fetcher).name = "Asset Bundle Fetcher";
		this.bundlePath = bundlePath;
		this.bundleReceiveCallback = bundleReceiveCallback;
		autoRetry = true;
		requestMgr = mgr;
		timestamp = time;
		this.retryCounter = retryCounter;
	}

	internal void DoFetch()
	{
		fetcher.DoFetch(requestMgr.RootUrl, bundlePath, SystemCallback);
	}

	private void SystemCallback(AssetBundle assetBundle, string bundlePath)
	{
		if ((Object)(object)assetBundle == (Object)null)
		{
			if (autoRetry)
			{
				requestMgr.AddToRetryQueue(bundlePath, bundleReceiveCallback, retryCounter);
			}
			else
			{
				Debug.Log((object)"AssetBundleRequest failed. No auto-retry.");
				requestMgr.FinalizeRequest(bundlePath, assetBundle);
			}
		}
		else
		{
			requestMgr.FinalizeRequest(bundlePath, assetBundle);
		}
		Object.Destroy((Object)(object)fetcher);
		Object.Destroy((Object)(object)gameObject);
		isDone = true;
	}
}
