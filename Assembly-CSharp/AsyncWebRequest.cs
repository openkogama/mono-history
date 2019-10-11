using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class AsyncWebRequest
{
	protected enum State
	{
		Created,
		Running,
		Waiting
	}

	protected Action<UnityWebRequest> callback;

	protected readonly string path;

	protected int retries = AsyncWWWManager.Retries;

	protected TimeSpan currentTimeout = new TimeSpan(0L);

	protected DateTime retryTime = DateTime.Now;

	protected State state;

	protected UnityWebRequest request;

	protected bool isDone;

	public readonly WWWRequestPriority requestPriority;

	protected static bool CacheCompatibility => CheckCacheCompatibility();

	public Action<UnityWebRequest> Callback
	{
		get
		{
			return callback;
		}
		set
		{
			callback = value;
		}
	}

	protected AsyncWebRequest(string path, Action<UnityWebRequest> callback, WWWRequestPriority requestPriority)
	{
		this.requestPriority = requestPriority;
		this.path = path;
		this.callback = callback;
	}

	private static bool CheckCacheCompatibility()
	{
		return true;
	}

	public bool Update()
	{
		switch (state)
		{
		case State.Created:
			GotoRunState();
			break;
		case State.Running:
			if (UpdateRunningState())
			{
				return true;
			}
			break;
		case State.Waiting:
			if (IsWaitingStateDone())
			{
				GotoRunState();
			}
			break;
		}
		return false;
	}

	public void Dispose()
	{
		if (request != null)
		{
			request.Dispose();
		}
	}

	private void GotoRunState()
	{
		request = Create();
		state = State.Running;
		request.SendWebRequest();
	}

	protected virtual bool UpdateRunningState()
	{
		bool flag = request.isDone;
		if (flag)
		{
			if (!ReadyToDoCallback())
			{
				return false;
			}
			isDone = true;
			try
			{
				if (callback != null)
				{
					callback(request);
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

	protected bool ReadyToDoCallback()
	{
		if (request.error != null)
		{
			bool flag = true;
			if (int.TryParse(request.error[0].ToString(), out var result) && result == 4)
			{
				flag = false;
			}
			if (retries > 0 && flag)
			{
				retries--;
				retryTime = DateTime.Now;
				currentTimeout = new TimeSpan(0, 0, 0, AsyncWWWManager.RetryTimeouts[retries]);
				state = State.Waiting;
				foreach (KeyValuePair<string, string> responseHeader in request.GetResponseHeaders())
				{
					Debug.LogFormat("{0} {1}", responseHeader.Key, responseHeader.Value);
				}
				request = Create();
				return false;
			}
			Debug.LogWarning("Failed url: " + request.url);
			Debug.LogError(request.error);
		}
		return true;
	}

	private bool IsWaitingStateDone()
	{
		if (DateTime.Now - retryTime > currentTimeout)
		{
			return true;
		}
		return false;
	}

	protected abstract UnityWebRequest Create();

	public override string ToString()
	{
		return path;
	}
}
