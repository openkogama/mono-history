using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPNotification : Notification
{
	[SerializeField]
	private Text AmountLabel;

	[SerializeField]
	private NotificationSlideOut slider;

	protected override NotificationLifetime Lifetime => NotificationLifetime.Low;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		string text = data[(byte)1].ToString();
		AmountLabel.text = text + " " + data[(byte)4].ToString() + " XP!";
	}

	protected override void Update()
	{
		timeSinceStart += Time.deltaTime;
		if (timeSinceStart >= (float)Lifetime)
		{
			pool.Return(this);
		}
	}
}
