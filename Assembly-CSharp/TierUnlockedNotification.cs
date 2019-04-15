using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TierUnlockedNotification : Notification
{
	[SerializeField]
	private Text tierUnlockedText;

	[SerializeField]
	private NotificationSlideOut slider;

	protected override NotificationLifetime Lifetime => NotificationLifetime.Low;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		tierUnlockedText.text = " YOU HAVE UNLOCKED TIER " + data[(byte)4].ToString() + "!";
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
