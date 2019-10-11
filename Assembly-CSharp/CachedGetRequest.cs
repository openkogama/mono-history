using System;
using UnityEngine.Networking;

public class CachedGetRequest : GetRequest
{
	public CachedGetRequest(string path, Action<UnityWebRequest> callback, WWWRequestPriority requestPriority)
		: base(path, callback, requestPriority)
	{
	}

	public virtual bool FoundInCache(AsyncWWWManager.Cache cache)
	{
		if (cache.TryGet(path, out var cachedGetRequest))
		{
			if (callback != null)
			{
				cachedGetRequest.AddToCallback(callback);
			}
			return true;
		}
		cache.Add(path, this);
		return false;
	}

	protected void AddToCallback(Action<UnityWebRequest> callbackOther)
	{
		if (isDone)
		{
			callbackOther(request);
		}
		else
		{
			callback = (Action<UnityWebRequest>)Delegate.Combine(callback, callbackOther);
		}
	}
}
