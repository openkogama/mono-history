using System;
using UnityEngine;

public class StreamingAssetRequestTempHack : CachedGetRequest
{
	private UnityEngine.Object mainAsset;

	private new Action<WWW, UnityEngine.Object> callback;

	public StreamingAssetRequestTempHack(string path, Action<WWW, UnityEngine.Object> callback)
		: base(path, null)
	{
		this.callback = callback;
	}

	protected override bool UpdateRunningState()
	{
		bool flag = www.isDone;
		if (flag)
		{
			if (www.error != null)
			{
				if (retries > 0)
				{
					retries--;
					retryTime = Time.time;
					currentTimeout = AsyncWWWManager.RetryTimeouts[retries];
					state = State.Waiting;
					Debug.Log(www.error + " " + www.url + " " + Time.frameCount + " " + AsyncWWWManager.RetryTimeouts[retries]);
					www = Create();
					return false;
				}
				Debug.LogWarning($"{www.url}\n{www.error}");
			}
			isDone = true;
			if (mainAsset == null)
			{
				mainAsset = www.assetBundle.mainAsset;
			}
			try
			{
				if (callback != null)
				{
					callback(www, mainAsset);
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			finally
			{
				callback = null;
			}
		}
		return flag;
	}

	public override bool FoundInCache(AsyncWWWManager.Cache cache)
	{
		if (cache.TryGet(path, out var cachedGetRequest))
		{
			if (callback != null)
			{
				StreamingAssetRequestTempHack streamingAssetRequestTempHack = (StreamingAssetRequestTempHack)cachedGetRequest;
				streamingAssetRequestTempHack.AddToCallback(callback);
			}
			return true;
		}
		cache.Add(path, this);
		return false;
	}

	public void AddToCallback(Action<WWW, UnityEngine.Object> callbackOther)
	{
		if (isDone)
		{
			callbackOther(www, mainAsset);
		}
		else
		{
			callback = (Action<WWW, UnityEngine.Object>)Delegate.Combine(callback, callbackOther);
		}
	}

	protected override WWW Create()
	{
		WWW wWW;
		if (MVGameControllerBase.VersionStreamingAssets >= 0)
		{
			wWW = (AsyncWebRequest.useCaching ? WWW.LoadFromCacheOrDownload(path + "?version=" + MVGameControllerBase.VersionStreamingAssets, MVGameControllerBase.VersionStreamingAssets) : new WWW(path + "?version=" + MVGameControllerBase.VersionStreamingAssets));
		}
		else
		{
			Debug.LogError("Streaming assets version must be >= 0 to enable WWW.LoadFromCacheOrDownload. Using non-cached approach");
			wWW = new WWW(path + "?version=" + MVGameControllerBase.VersionStreamingAssets);
		}
		if (wWW == null)
		{
			Debug.LogError("WWW null");
		}
		return wWW;
	}
}
