using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPNotification : Notification
{
	[SerializeField]
	private Text AmountLabel;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		AmountLabel.text = data[(byte)4].ToString() + " " + (string)data[(byte)1];
		Lifetime = 8f;
	}
}
