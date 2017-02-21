using System.Collections.Generic;

public class JetPackOverheatNotification : Notification
{
	protected override NotificationLifetime Lifetime => NotificationLifetime.Low;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
	}
}
