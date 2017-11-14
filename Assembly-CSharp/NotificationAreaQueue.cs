using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationAreaQueue : NotificationArea
{
	private class EnqueuedNotification
	{
		public readonly NotificationType notificationType;

		public readonly Dictionary<object, object> data;

		public EnqueuedNotification(NotificationType notificationType, Dictionary<object, object> data)
		{
			this.notificationType = notificationType;
			this.data = data;
		}
	}

	private Queue<EnqueuedNotification> enqueuedNotifications = new Queue<EnqueuedNotification>();

	private void Awake()
	{
		NotificationObjectPool notificationObjectPool = objectPool;
		notificationObjectPool.OnActiveInstancesChanged = (Action)Delegate.Combine(notificationObjectPool.OnActiveInstancesChanged, new Action(OnActiveInstancesChanged));
	}

	private void OnActiveInstancesChanged()
	{
		if (objectPool.ActivateInstancesCount <= 0 && enqueuedNotifications.Count != 0)
		{
			EnqueuedNotification enqueuedNotification = enqueuedNotifications.Dequeue();
			CreateNotification(enqueuedNotification.notificationType, enqueuedNotification.data);
		}
	}

	public override void InstantiateNotification(NotificationType notificationType, Dictionary<object, object> data)
	{
		if (objectPool.ActivateInstancesCount == 0)
		{
			CreateNotification(notificationType, data);
			return;
		}
		foreach (EnqueuedNotification enqueuedNotification in enqueuedNotifications)
		{
			if (enqueuedNotification.notificationType == notificationType && enqueuedNotification.data[(byte)1] == data[(byte)1])
			{
				return;
			}
		}
		enqueuedNotifications.Enqueue(new EnqueuedNotification(notificationType, data));
	}

	private void CreateNotification(NotificationType notificationType, Dictionary<object, object> data)
	{
		Notification panel = objectPool.GetPanel(notificationType);
		if (panel == null)
		{
			Debug.LogWarning("Notification is null");
			return;
		}
		panel.transform.SetParent(contentHolderTransform, worldPositionStays: false);
		panel.Initialize(data);
	}

	private void OnDestroy()
	{
		NotificationObjectPool notificationObjectPool = objectPool;
		notificationObjectPool.OnActiveInstancesChanged = (Action)Delegate.Remove(notificationObjectPool.OnActiveInstancesChanged, new Action(OnActiveInstancesChanged));
	}
}
