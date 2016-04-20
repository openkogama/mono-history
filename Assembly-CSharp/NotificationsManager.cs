using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{
	[SerializeField]
	private RectTransform contentPanel;

	[SerializeField]
	private NotificationObjectPool objectPool;

	private void OnEnable()
	{
		NotificationController.Register(this);
	}

	public Notification InstantiateNotification(NotificationType notificationType, Dictionary<object, object> message)
	{
		Notification panel = objectPool.GetPanel(notificationType);
		panel.transform.SetParent(contentPanel, worldPositionStays: false);
		panel.Initialize(message);
		return panel;
	}
}
