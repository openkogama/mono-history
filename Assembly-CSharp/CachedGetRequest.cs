using System;
using UnityEngine;

public class CachedGetRequest : GetRequest
{
	public CachedGetRequest(string path, Action<WWW> callback)
		: base(path, callback)
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

	protected void AddToCallback(Action<WWW> callbackOther)
	{
		if (isDone)
		{
			callbackOther(www);
		}
		else
		{
			callback = (Action<WWW>)Delegate.Combine(callback, callbackOther);
		}
	}
}
