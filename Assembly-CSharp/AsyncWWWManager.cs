using System;
using System.Collections.Generic;
using UnityEngine;

public static class AsyncWWWManager
{
	public class Cache
	{
		private Dictionary<string, CachedGetRequest> cachedRequests = new Dictionary<string, CachedGetRequest>();

		public bool TryGet(string path, out CachedGetRequest cachedGetRequest)
		{
			return cachedRequests.TryGetValue(path, out cachedGetRequest);
		}

		public void Add(string path, CachedGetRequest cachedGetRequest)
		{
			cachedRequests.Add(path, cachedGetRequest);
		}

		public void UnsubscribeCached(Action<WWW> callback)
		{
			foreach (KeyValuePair<string, CachedGetRequest> cachedRequest in cachedRequests)
			{
				Unsubscribe(cachedRequest.Value, callback);
			}
		}

		public void Clear()
		{
			cachedRequests.Clear();
		}
	}

	private class TemporaryHashSet<T> : HashSet<T>, IDisposable
	{
		public void Dispose()
		{
			Clear();
		}
	}

	private static int retries = 3;

	public static readonly int[] RetryTimeouts = new int[3] { 30, 20, 10 };

	private const int quitTimeOut = 5000;

	private const int maxRequests = 4;

	private static Action quitCallback;

	private static int quitTime;

	private static bool isQuiting = false;

	private static readonly Dictionary<WWWRequestPriority, Queue<AsyncWebRequest>> requests = new Dictionary<WWWRequestPriority, Queue<AsyncWebRequest>>
	{
		{
			WWWRequestPriority.WaitUntilSyncronizingIsDone,
			new Queue<AsyncWebRequest>()
		},
		{
			WWWRequestPriority.ExecuteWhileSyncronizing,
			new Queue<AsyncWebRequest>()
		},
		{
			WWWRequestPriority.ExecuteIgnoreAllConstraints,
			new Queue<AsyncWebRequest>()
		}
	};

	private static readonly HashSet<AsyncWebRequest> activeRequests = new HashSet<AsyncWebRequest>();

	private static readonly TemporaryHashSet<AsyncWebRequest> tempHashSet = new TemporaryHashSet<AsyncWebRequest>();

	private static Cache cache = new Cache();

	private static bool isBackgroundUpdate = false;

	public static int Retries => retries;

	public static void WWWRequest(AsyncWebRequest asyncRequest)
	{
		if (isQuiting)
		{
			return;
		}
		if (asyncRequest is CachedGetRequest)
		{
			CachedGetRequest cachedGetRequest = (CachedGetRequest)asyncRequest;
			if (cachedGetRequest.FoundInCache(cache))
			{
				return;
			}
		}
		requests[asyncRequest.requestPriority].Enqueue(asyncRequest);
	}

	public static void ShutDown(Action quitHandled)
	{
		if (!isQuiting)
		{
			isQuiting = true;
			requests[WWWRequestPriority.WaitUntilSyncronizingIsDone].Clear();
			retries = 0;
			AddRequestsToActiveRequests(requests[WWWRequestPriority.ExecuteIgnoreAllConstraints], int.MaxValue);
			AddRequestsToActiveRequests(requests[WWWRequestPriority.ExecuteWhileSyncronizing], int.MaxValue);
			quitTime = WaitForTicksLocal.GetEnvironmentTick(0);
			quitCallback = quitHandled;
		}
		else
		{
			Debug.LogError("Handle quit called twice");
		}
	}

	public static void UnsubscribeWWWRequest(Action<WWW> callback)
	{
		foreach (AsyncWebRequest activeRequest in activeRequests)
		{
			Unsubscribe(activeRequest, callback);
		}
		foreach (KeyValuePair<WWWRequestPriority, Queue<AsyncWebRequest>> request in requests)
		{
			foreach (AsyncWebRequest item in request.Value)
			{
				Unsubscribe(item, callback);
			}
		}
		cache.UnsubscribeCached(callback);
	}

	public static void BackgroundUpdate()
	{
		isBackgroundUpdate = true;
		AddRequestsToActiveRequests(requests[WWWRequestPriority.ExecuteIgnoreAllConstraints], int.MaxValue);
		AddRequestsToActiveRequests(requests[WWWRequestPriority.ExecuteWhileSyncronizing], 4);
		InternalUpdate();
	}

	public static void Update()
	{
		isBackgroundUpdate = false;
		AddRequestsToActiveRequests(requests[WWWRequestPriority.ExecuteIgnoreAllConstraints], int.MaxValue);
		AddRequestsToActiveRequests(requests[WWWRequestPriority.ExecuteWhileSyncronizing], 4);
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			AddRequestsToActiveRequests(requests[WWWRequestPriority.WaitUntilSyncronizingIsDone], 4);
		}
		InternalUpdate();
	}

	public static void Reset()
	{
		retries = 3;
		isQuiting = false;
		foreach (Queue<AsyncWebRequest> value in requests.Values)
		{
			value.Clear();
			value.TrimExcess();
		}
		foreach (AsyncWebRequest activeRequest in activeRequests)
		{
			activeRequest.Dispose();
		}
		activeRequests.Clear();
		tempHashSet.TrimExcess();
		cache.Clear();
	}

	public static void PostResetCleanup()
	{
		if (quitCallback != null)
		{
			Debug.LogWarning("AsyncWWWManager quitCallback is not null.");
			quitCallback = null;
		}
	}

	private static void InternalUpdate()
	{
		UpdateActiveRequests();
		if (isQuiting)
		{
			Quit();
		}
	}

	private static void UpdateActiveRequests()
	{
		using TemporaryHashSet<AsyncWebRequest> temporaryHashSet = tempHashSet;
		foreach (AsyncWebRequest activeRequest in activeRequests)
		{
			if ((activeRequest.requestPriority != WWWRequestPriority.WaitUntilSyncronizingIsDone || !isBackgroundUpdate) && activeRequest.Update())
			{
				temporaryHashSet.Add(activeRequest);
			}
		}
		foreach (AsyncWebRequest item in temporaryHashSet)
		{
			activeRequests.Remove(item);
		}
	}

	private static void Unsubscribe(AsyncWebRequest request, Action<WWW> callback)
	{
		if (request.Callback == callback)
		{
			request.Callback = (Action<WWW>)Delegate.Remove(request.Callback, callback);
		}
	}

	private static void AddRequestsToActiveRequests(Queue<AsyncWebRequest> requestQueue, int maxRequestForQueue)
	{
		while (activeRequests.Count < maxRequestForQueue && requestQueue.Count > 0)
		{
			activeRequests.Add(requestQueue.Dequeue());
		}
	}

	private static void Quit()
	{
		if (quitCallback != null)
		{
			if (activeRequests.Count == 0)
			{
				Debug.Log("AsyncWWWManager did handle all request on quit: " + true);
				quitCallback();
				quitCallback = null;
			}
			else if (WaitForTicksLocal.Diff(quitTime) > 5000)
			{
				Debug.Log("AsyncWWWManager did handle all request on quit: " + false);
				quitCallback();
				quitCallback = null;
			}
		}
	}
}
