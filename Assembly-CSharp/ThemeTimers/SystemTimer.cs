using System;

namespace ThemeTimers;

public class SystemTimer : ITimer
{
	public float Time { get; private set; }

	public void Update()
	{
		DateTime dateTime = DateTime.UtcNow.AddHours(MVGameControllerBase.Game.TimeZone);
		float num = dateTime.Hour * 60 * 60 * 1000;
		num += (float)(dateTime.Minute * 60 * 1000);
		num += (float)(dateTime.Second * 1000);
		num += (float)dateTime.Millisecond;
		Time = num / 86400000f * 100f;
	}
}
