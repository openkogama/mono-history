using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public abstract class Notification : MonoBehaviour
{
	public NotificationType Type;

	[HideInInspector]
	public NotificationObjectPool pool;

	protected float timeSinceStart;

	protected abstract NotificationLifetime Lifetime { get; }

	public float Progress => timeSinceStart / (float)Lifetime;

	public virtual void Initialize(Dictionary<object, object> data)
	{
		timeSinceStart = 0f;
	}

	protected virtual void Update()
	{
		timeSinceStart += Time.deltaTime;
		if (timeSinceStart >= (float)Lifetime)
		{
			pool.Return(this);
			OnReturn();
		}
	}

	public virtual void OnReturn()
	{
	}

	protected void Close()
	{
		timeSinceStart = (float)(Lifetime + 1);
	}
}
