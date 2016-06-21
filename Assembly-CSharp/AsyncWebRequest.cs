using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AsyncWebRequest
{
	protected enum State
	{
		Created,
		Running,
		Waiting
	}

	protected Action<WWW> callback;

	protected readonly string path;

	protected int retries = 3;

	protected float currentTimeout;

	protected float retryTime = Time.realtimeSinceStartup;

	protected State state;

	protected WWW www;

	protected bool isDone;

	public readonly WWWRequestPriority requestPriority;

	protected static bool UseCaching => true;

	protected AsyncWebRequest(string path, Action<WWW> callback, WWWRequestPriority requestPriority)
	{
		this.requestPriority = requestPriority;
		this.path = path;
		this.callback = callback;
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
		if (www != null)
		{
			www.Dispose();
		}
	}

	private void GotoRunState()
	{
		www = Create();
		state = State.Running;
	}

	protected virtual bool UpdateRunningState()
	{
		bool flag = www.isDone;
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
					callback(www);
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
		if (www.error != null)
		{
			bool flag = true;
			if (int.TryParse(www.error[0].ToString(), out var result))
			{
				Debug.Log("errorCodeFirstVal " + result);
				if (result == 4)
				{
					flag = false;
				}
			}
			if (retries > 0 && flag)
			{
				retries--;
				retryTime = Time.time;
				currentTimeout = AsyncWWWManager.RetryTimeouts[retries];
				state = State.Waiting;
				Debug.Log(www.error + " " + www.url + " " + Time.frameCount + " " + AsyncWWWManager.RetryTimeouts[retries]);
				Debug.Log("Response headers");
				foreach (KeyValuePair<string, string> responseHeader in www.responseHeaders)
				{
					Debug.LogFormat("{0} {1}", responseHeader.Key, responseHeader.Value);
				}
				www = Create();
				return false;
			}
			Debug.LogWarning($"{www.url}\n{www.error}");
		}
		return true;
	}

	private bool IsWaitingStateDone()
	{
		if (Time.time - retryTime > currentTimeout)
		{
			return true;
		}
		return false;
	}

	protected abstract WWW Create();
}
