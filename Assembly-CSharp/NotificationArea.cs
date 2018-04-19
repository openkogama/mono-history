using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationArea : MonoBehaviour
{
	[SerializeField]
	protected NotificationObjectPool objectPool;

	[SerializeField]
	protected RectTransform contentHolderTransform;

	public virtual void InstantiateNotification(NotificationType notificationType, Dictionary<object, object> data)
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

	public bool CanInstantiateNotificationType(NotificationType notificationType)
	{
		return objectPool.CanInstantiateNotificationType(notificationType);
	}
}
