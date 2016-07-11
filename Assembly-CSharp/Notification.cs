using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public abstract class Notification : MonoBehaviour
{
	public delegate void NotificationDelegate(Notification notification);

	public NotificationType Type;

	[SerializeField]
	private RectTransform ContentBase;

	private float timeSinceStart;

	private bool selfDestroy = true;

	private readonly Vector2 Offset = new Vector2(-250f, 0f);

	private Vector2 Target;

	private float lerpSpeed = 20f;

	public bool SelfDestroy
	{
		get
		{
			return selfDestroy;
		}
		set
		{
			selfDestroy = value;
		}
	}

	public NotificationObjectPool pool { get; set; }

	protected virtual NotificationLifetime Lifetime { get; set; }

	public event NotificationDelegate OnNotificationClosedEnd;

	public virtual void Initialize(Dictionary<object, object> data)
	{
		ContentBase.anchoredPosition = Offset;
		Target = Vector2.zero;
		timeSinceStart = 0f;
		if (Lifetime == 0)
		{
			Lifetime = NotificationLifetime.Low;
		}
	}

	private void Update()
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
		else if (timeSinceStart >= (float)Lifetime - 0.2f)
		{
			Target = Offset;
		}
		ContentBase.anchoredPosition = Vector2.Lerp(ContentBase.anchoredPosition, Target, lerpSpeed * Time.deltaTime);
	}

	protected void Close()
	{
		timeSinceStart = (float)(Lifetime - 1);
		OnClose();
	}

	protected virtual void OnClose()
	{
	}
}
