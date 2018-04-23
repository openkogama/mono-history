using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public class NotificationObjectPool : MonoBehaviour
{
	[SerializeField]
	private List<NotificationObjectPoolElement> Elements = new List<NotificationObjectPoolElement>();

	private List<Notification> Instances = new List<Notification>();

	private List<Notification> ActiveInstances = new List<Notification>();

	public Action OnActiveInstancesChanged;

	public int ActivateInstancesCount => ActiveInstances.Count;

	public bool CanInstantiateNotificationType(NotificationType notificationType)
	{
		if (!Instances.Find((Notification x) => (byte)x.Type == (byte)notificationType) && !ActiveInstances.Find((Notification x) => (byte)x.Type == (byte)notificationType))
		{
			return false;
		}
		return true;
	}

	private void Awake()
	{
		foreach (NotificationObjectPoolElement element in Elements)
		{
			for (int i = 0; i < element.Instances; i++)
			{
				Notification notification = UnityEngine.Object.Instantiate(element.Prefab);
				notification.transform.SetParent(transform);
				notification.gameObject.SetActive(value: false);
				notification.pool = this;
				Instances.Add(notification);
			}
		}
	}

	public void ReturnAllExistingNotifications()
	{
		for (int num = ActiveInstances.Count - 1; num >= 0; num--)
		{
			Notification notification = ActiveInstances[num];
			Return(notification);
		}
	}

	public Notification GetPanel(NotificationType type)
	{
		if (!Instances.Find((Notification x) => (byte)x.Type == (byte)type))
		{
			Debug.LogWarning("Could not find notification type: " + type);
			return CreateTempPanel(type);
		}
		int index = Instances.FindIndex((Notification x) => (byte)x.Type == (byte)type);
		Notification notification = Instances[index];
		notification.pool = this;
		notification.gameObject.SetActive(value: true);
		AddToActiveInstances(Instances[index]);
		Instances.RemoveAt(index);
		return notification;
	}

	public void Return(Notification notification)
	{
		notification.gameObject.SetActive(value: false);
		notification.transform.SetParent(transform);
		RemoveFromActiveInstances(notification);
		Instances.Add(notification);
	}

	private Notification CreateTempPanel(NotificationType type)
	{
		Notification notification = UnityEngine.Object.Instantiate(Elements.First((NotificationObjectPoolElement x) => x.Prefab.Type == type).Prefab);
		if (notification == null)
		{
			Debug.LogError("Couldn't find temp panel type " + type);
			return null;
		}
		notification.pool = this;
		AddToActiveInstances(notification);
		notification.gameObject.SetActive(value: true);
		return notification;
	}

	private void AddToActiveInstances(Notification notification)
	{
		ActiveInstances.Add(notification);
		if (OnActiveInstancesChanged != null)
		{
			OnActiveInstancesChanged();
		}
	}

	private void RemoveFromActiveInstances(Notification notification)
	{
		ActiveInstances.Remove(notification);
		if (OnActiveInstancesChanged != null)
		{
			OnActiveInstancesChanged();
		}
	}
}
