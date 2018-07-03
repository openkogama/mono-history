using System;
using System.Text;

namespace MV.WorldObject.Accessories;

public class AccessoryTimelimit
{
	public int timeLimit;

	public DateTime timeLimitStartTime;

	public bool IsTimeLimited
	{
		get
		{
			if (timeLimit != 0)
			{
				return true;
			}
			return false;
		}
	}

	public TimeSpan GetTimeLeft()
	{
		if (!IsTimeLimited)
		{
			return TimeSpan.MaxValue;
		}
		TimeSpan timeSpan = DateTime.UtcNow - timeLimitStartTime;
		TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, timeLimit, 0);
		return timeSpan2 - timeSpan;
	}

	public bool GetHasTimeLeft()
	{
		if (!IsTimeLimited)
		{
			return true;
		}
		return GetTimeLeft().TotalSeconds > 0.0;
	}

	public override string ToString()
	{
		if (!IsTimeLimited)
		{
			return "No timelimit";
		}
		return ToPrettyFormat(GetTimeLeft());
	}

	public static string ToPrettyFormat(TimeSpan span)
	{
		if (span == TimeSpan.Zero)
		{
			return "0 minutes";
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (span.Days > 0)
		{
			stringBuilder.AppendFormat("{0} day{1} ", span.Days, (span.Days > 1) ? "s" : string.Empty);
		}
		if (span.Hours > 0)
		{
			stringBuilder.AppendFormat("{0} hour{1} ", span.Hours, (span.Hours > 1) ? "s" : string.Empty);
		}
		if (span.Minutes > 0)
		{
			stringBuilder.AppendFormat("{0} minute{1} ", span.Minutes, (span.Minutes > 1) ? "s" : string.Empty);
		}
		return stringBuilder.ToString();
	}
}
