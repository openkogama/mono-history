using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalNotification : Notification
{
	[SerializeField]
	private Text text;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Sprite defaultSprite;

	private NotificationLifetime lifeTime;

	protected override NotificationLifetime Lifetime => lifeTime;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		text.text = (string)data[(byte)1];
		lifeTime = (NotificationLifetime)data[(byte)2];
		if (data.ContainsKey((byte)3))
		{
			image.sprite = (Sprite)data[(byte)3];
		}
		else
		{
			image.sprite = defaultSprite;
		}
		Vector3 localScale = image.transform.localScale;
		localScale.x = -1f;
		image.transform.localScale = localScale;
	}
}
