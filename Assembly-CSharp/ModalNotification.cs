using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalNotification : Notification
{
	[SerializeField]
	private Text text;

	[SerializeField]
	private Image image;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		text.text = (string)data[(byte)1];
		Lifetime = (int)data[(byte)2];
		if (data.ContainsKey((byte)3))
		{
			image.sprite = (Sprite)data[(byte)3];
		}
		Vector3 localScale = image.transform.localScale;
		localScale.x = (float)data[(byte)4];
		image.transform.localScale = localScale;
	}
}
