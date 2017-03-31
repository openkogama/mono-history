using System;
using UnityEngine;

public class StreamingAssetRequestTempHack : CachedGetRequest
{
	private UnityEngine.Object mainAsset;

	private Action<WWW, UnityEngine.Object> callbackTemp;

	public Action<WWW, UnityEngine.Object> CallbackHack
	{
		get
		{
			return callbackTemp;
		}
		set
		{
			callbackTemp = value;
		}
	}

	public StreamingAssetRequestTempHack(string path, Action<WWW, UnityEngine.Object> callbackTemp, WWWRequestPriority requestPriority)
		: base(path, null, requestPriority)
	{
		this.callbackTemp = callbackTemp;
	}

	protected override bool UpdateRunningState()
	{
		bool flag = www.isDone;
		if (flag)
		{
			if (!ReadyToDoCallback())
			{
				return false;
			}
			isDone = true;
			if (mainAsset == null && string.IsNullOrEmpty(www.error))
			{
				mainAsset = www.assetBundle.mainAsset;
			}
			try
			{
				if (callbackTemp != null)
				{
					callbackTemp(www, mainAsset);
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			finally
			{
				callbackTemp = null;
			}
		}
		return flag;
	}

	public override bool FoundInCache(AsyncWWWManager.Cache cache)
	{
		if (cache.TryGet(path, out var cachedGetRequest))
		{
			if (callbackTemp != null)
			{
				StreamingAssetRequestTempHack streamingAssetRequestTempHack = (StreamingAssetRequestTempHack)cachedGetRequest;
				streamingAssetRequestTempHack.AddToCallback(callbackTemp);
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
			callbackTemp = (Action<WWW, UnityEngine.Object>)Delegate.Combine(callbackTemp, callbackOther);
		}
	}

	protected override WWW Create()
	{
		WWW wWW;
		if (MVGameControllerBase.KoGaMaSettings.VersionStreamingAssets >= 0)
		{
			wWW = ((!AsyncWebRequest.UseCaching) ? new WWW(path + "?version=" + MVGameControllerBase.KoGaMaSettings.VersionStreamingAssets) : WWW.LoadFromCacheOrDownload(path + "?version=" + MVGameControllerBase.KoGaMaSettings.VersionStreamingAssets, MVGameControllerBase.KoGaMaSettings.VersionStreamingAssets));
		}
		else
		{
			Debug.LogError("Streaming assets version must be >= 0 to enable WWW.LoadFromCacheOrDownload. Using non-cached approach");
			wWW = new WWW(path + "?version=" + MVGameControllerBase.KoGaMaSettings.VersionStreamingAssets);
		}
		if (wWW == null)
		{
			Debug.LogError("WWW null");
		}
		return wWW;
	}
}
