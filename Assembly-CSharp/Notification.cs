using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public abstract class Notification : MonoBehaviour
{
	public delegate void NotificationDelegate(Notification notification);

	public bool selfDestroy;

	public NotificationType Type;

	[HideInInspector]
	public NotificationObjectPool pool;

	[SerializeField]
	protected RectTransform ContentBase;

	private float timeSinceStart;

	protected abstract NotificationLifetime Lifetime { get; }

	public float Progress => timeSinceStart / (float)Lifetime;

	public event NotificationDelegate OnNotificationClosedEnd;

	public virtual void Initialize(Dictionary<object, object> data)
	{
		timeSinceStart = 0f;
	}

	protected virtual void Update()
	{
		timeSinceStart += Time.deltaTime;
		if (timeSinceStart >= (float)Lifetime)
		{
			if (OnNotificationClosedEnd != null)
			{
				OnNotificationClosedEnd(this);
			}
			if (selfDestroy)
			{
				Object.Destroy(gameObject);
			}
			else
			{
				pool.Return(this);
			}
		}
	}

	protected void Close()
	{
		timeSinceStart = (float)(Lifetime + 1);
	}
}
