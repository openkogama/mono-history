using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationAreaSingleInstanceQueue : NotificationArea
{
	private struct NotificationQueueData
	{
		public NotificationType notificationType;

		public Dictionary<object, object> data;

		public float startTime;
	}

	private Queue<NotificationQueueData> enqueuedNotifications = new Queue<NotificationQueueData>();

	private bool shouldSkipDequeueCallback;

	private const float decayTime = 10f;

	public override void InstantiateNotification(NotificationType notificationType, Dictionary<object, object> data)
	{
		bool flag = false;
		if (data.ContainsKey((byte)18))
		{
			flag = (bool)data[(byte)18];
		}
		if (flag)
		{
			ShowNotification(notificationType, data);
		}
		else if (objectPool.ActivateInstancesCount > 0)
		{
			QueueNotification(notificationType, data);
		}
		else
		{
			ShowNotification(notificationType, data);
		}
	}

	private void Awake()
	{
		NotificationObjectPool notificationObjectPool = objectPool;
		notificationObjectPool.OnActiveInstancesChanged = (Action)Delegate.Combine(notificationObjectPool.OnActiveInstancesChanged, new Action(OnActiveInstancesChanged));
	}

	private void QueueNotification(NotificationType notificationType, Dictionary<object, object> data)
	{
		NotificationQueueData item = new NotificationQueueData
		{
			notificationType = notificationType,
			data = data,
			startTime = Time.time
		};
		enqueuedNotifications.Enqueue(item);
	}

	private void ShowNotification(NotificationType notificationType, Dictionary<object, object> data)
	{
		shouldSkipDequeueCallback = true;
		objectPool.ReturnAllExistingNotifications();
		shouldSkipDequeueCallback = false;
		Notification panel = objectPool.GetPanel(notificationType);
		if (panel == null)
		{
			Debug.LogWarning("Notification is null");
			return;
		}
		panel.transform.SetParent(contentHolderTransform, worldPositionStays: false);
		panel.Initialize(data);
	}

	private void OnActiveInstancesChanged()
	{
		if (shouldSkipDequeueCallback || objectPool.ActivateInstancesCount > 0 || enqueuedNotifications.Count == 0)
		{
			return;
		}
		while (enqueuedNotifications.Count > 0)
		{
			NotificationQueueData notificationQueueData = enqueuedNotifications.Dequeue();
			float num = Time.time - notificationQueueData.startTime;
			if (!(num > 10f))
			{
				ShowNotification(notificationQueueData.notificationType, notificationQueueData.data);
			}
		}
	}
}
