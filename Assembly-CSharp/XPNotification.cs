using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPNotification : Notification
{
	[SerializeField]
	private Text AmountLabel;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		AmountLabel.text = data[(byte)4].ToString() + " " + (string)data[(byte)1];
	}
}
