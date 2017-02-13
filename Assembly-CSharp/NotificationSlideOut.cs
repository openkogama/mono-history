using UnityEngine;

public class NotificationSlideOut : MonoBehaviour
{
	[SerializeField]
	private Notification notification;

	[SerializeField]
	private Vector2 slideVelocity = new Vector2(-400f, 0f);

	[SerializeField]
	[Range(0f, 1f)]
	private float slideOutStartTime = 0.75f;

	private void OnValidate()
	{
		if (notification == null)
		{
			notification = GetComponent<Notification>();
		}
	}

	protected void Update()
	{
		if (notification.Progress > slideOutStartTime)
		{
			((RectTransform)transform).anchoredPosition += slideVelocity * Time.deltaTime;
		}
	}
}
