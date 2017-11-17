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
	}

	private static Action<bool> quitCallback;

	private static int quitTime = 0;

	private static bool isQuiting = false;

	private const int quitTimeOut = 5000;

	private static int maxRequests = 4;

	private static int retries = 3;

	public static readonly int[] RetryTimeouts = new int[3] { 30, 20, 10 };

	private static Dictionary<WWWRequestPriority, Queue<AsyncWebRequest>> requests = new Dictionary<WWWRequestPriority, Queue<AsyncWebRequest>>
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

	private static HashSet<AsyncWebRequest> activeRequest = new HashSet<AsyncWebRequest>();

	private static HashSet<AsyncWebRequest> doneRequests = new HashSet<AsyncWebRequest>();

	private static Cache cache = new Cache();

	private static bool dispose = false;

	public static int Retries => retries;

	public static void WWWRequest(AsyncWebRequest asyncRequest)
	{
		if (dispose || isQuiting)
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

	public static void HandleQuit(Action<bool> quitHandled)
	{
		if (isQuiting)
		{
			Debug.LogError("Handle quit called twice");
			return;
		}
		isQuiting = true;
		requests[WWWRequestPriority.WaitUntilSyncronizingIsDone].Clear();
		retries = 0;
		AddRequestsToActiveRequests(requests[WWWRequestPriority.ExecuteIgnoreAllConstraints], int.MaxValue);
		AddRequestsToActiveRequests(requests[WWWRequestPriority.ExecuteWhileSyncronizing], int.MaxValue);
		quitTime = WaitForTicksLocal.GetEnvironmentTick(0);
		quitCallback = quitHandled;
	}

	public static void UnsubscribeWWWRequest(Action<WWW> callback)
	{
		foreach (AsyncWebRequest item in activeRequest)
		{
			Unsubscribe(item, callback);
		}
		foreach (AsyncWebRequest doneRequest in doneRequests)
		{
			Unsubscribe(doneRequest, callback);
		}
		foreach (KeyValuePair<WWWRequestPriority, Queue<AsyncWebRequest>> request in requests)
		{
			foreach (AsyncWebRequest item2 in request.Value)
			{
				Unsubscribe(item2, callback);
			}
		}
		cache.UnsubscribeCached(callback);
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
		while (activeRequest.Count < maxRequestForQueue && requestQueue.Count > 0)
		{
			activeRequest.Add(requestQueue.Dequeue());
		}
	}

	public static void Update()
	{
		if (dispose)
		{
			return;
		}
		AddRequestsToActiveRequests(requests[WWWRequestPriority.ExecuteIgnoreAllConstraints], int.MaxValue);
		AddRequestsToActiveRequests(requests[WWWRequestPriority.ExecuteWhileSyncronizing], maxRequests);
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			AddRequestsToActiveRequests(requests[WWWRequestPriority.WaitUntilSyncronizingIsDone], maxRequests);
		}
		foreach (AsyncWebRequest item in activeRequest)
		{
			if (item.Update())
			{
				doneRequests.Add(item);
			}
		}
		foreach (AsyncWebRequest doneRequest in doneRequests)
		{
			activeRequest.Remove(doneRequest);
		}
		doneRequests.Clear();
		QuitHandling();
	}

	private static void QuitHandling()
	{
		if (isQuiting && quitCallback != null)
		{
			if (activeRequest.Count == 0)
			{
				quitCallback(obj: true);
				quitCallback = null;
			}
			else if (WaitForTicksLocal.Diff(quitTime) > 5000)
			{
				quitCallback(obj: false);
				quitCallback = null;
			}
		}
	}

	public static void Dispose()
	{
		dispose = true;
		foreach (AsyncWebRequest item in activeRequest)
		{
			item.Dispose();
		}
		activeRequest.Clear();
		foreach (KeyValuePair<WWWRequestPriority, Queue<AsyncWebRequest>> request in requests)
		{
			foreach (AsyncWebRequest item2 in request.Value)
			{
				item2.Dispose();
			}
		}
		requests.Clear();
	}
}
