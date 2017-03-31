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
	}

	public const int Retries = 3;

	private static int maxRequests = 4;

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

	public static void WWWRequest(AsyncWebRequest asyncRequest)
	{
		if (dispose)
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

	public static void UnsubscribeWWWRequest(Action<WWW> callback)
	{
		foreach (AsyncWebRequest item in activeRequest)
		{
			if (item.Callback == callback)
			{
				item.Callback = null;
			}
		}
	}

	public static void UnsubscribeWWWRequest(Action<WWW, UnityEngine.Object> callback)
	{
		foreach (AsyncWebRequest item in activeRequest)
		{
			if (item is StreamingAssetRequestTempHack streamingAssetRequestTempHack)
			{
				streamingAssetRequestTempHack.CallbackHack = null;
			}
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
