using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public class NotificationObjectPool : MonoBehaviour
{
	[SerializeField]
	private List<NotificationObjectPoolElement> Elements = new List<NotificationObjectPoolElement>();

	private List<Notification> Instances = new List<Notification>();

	private void Awake()
	{
		foreach (NotificationObjectPoolElement element in Elements)
		{
			for (int i = 0; i < element.Instances; i++)
			{
				Notification notification = Object.Instantiate(element.Prefab);
				notification.transform.SetParent(transform);
				notification.gameObject.SetActive(value: false);
				Instances.Add(notification);
			}
		}
	}

	public Notification GetPanel(NotificationType type)
	{
		if (!Instances.Find((Notification x) => (byte)x.Type == (byte)type))
		{
			return CreateTempPanel(type);
		}
		int index = Instances.FindIndex((Notification x) => (byte)x.Type == (byte)type);
		Notification notification = Instances[index];
		notification.SelfDestroy = false;
		notification.pool = this;
		notification.gameObject.SetActive(value: true);
		Instances.Remove(Instances[index]);
		return notification;
	}

	public void Return(Notification notification)
	{
		notification.gameObject.SetActive(value: false);
		notification.transform.SetParent(transform);
		Instances.Add(notification);
	}

	private Notification CreateTempPanel(NotificationType type)
	{
		Notification notification = Object.Instantiate(Elements.First((NotificationObjectPoolElement x) => x.Prefab.Type == type).Prefab);
		if (notification == null)
		{
			Debug.LogError("Couldn't find notification type " + type);
		}
		notification.gameObject.SetActive(value: true);
		return notification;
	}
}
