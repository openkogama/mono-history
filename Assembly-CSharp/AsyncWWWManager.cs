using System.Collections.Generic;

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

	public static readonly float[] RetryTimeouts = new float[3] { 30f, 20f, 10f };

	private static Queue<AsyncWebRequest> requests = new Queue<AsyncWebRequest>();

	private static HashSet<AsyncWebRequest> activeRequest = new HashSet<AsyncWebRequest>();

	private static Cache cache = new Cache();

	public static void WWWRequest(AsyncWebRequest asyncRequest)
	{
		if (asyncRequest is CachedGetRequest)
		{
			CachedGetRequest cachedGetRequest = (CachedGetRequest)asyncRequest;
			if (cachedGetRequest.FoundInCache(cache))
			{
				return;
			}
		}
		requests.Enqueue(asyncRequest);
	}

	public static void Update()
	{
		while (activeRequest.Count < maxRequests && requests.Count > 0)
		{
			activeRequest.Add(requests.Dequeue());
		}
		HashSet<AsyncWebRequest> hashSet = new HashSet<AsyncWebRequest>();
		foreach (AsyncWebRequest item in activeRequest)
		{
			if (item.Update())
			{
				hashSet.Add(item);
			}
		}
		foreach (AsyncWebRequest item2 in hashSet)
		{
			activeRequest.Remove(item2);
		}
	}
}
