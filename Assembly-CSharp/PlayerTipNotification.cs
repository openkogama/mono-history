using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTipNotification : Notification
{
	[SerializeField]
	private Text tipText;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		tipText.text = (string)data[(byte)1];
	}
}
