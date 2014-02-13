using System.Collections.Generic;

public class XPEvents
{
	private static XPEvents instance = new XPEvents();

	private Dictionary<XPEvent, int> xpEventToPoints = new Dictionary<XPEvent, int>
	{
		{
			XPEvent.Kill,
			5
		},
		{
			XPEvent.Flag,
			10
		}
	};

	public static XPEvents Instance => instance;

	private XPEvents()
	{
	}

	public int GetXPAmount(XPEvent id)
	{
		if (!xpEventToPoints.ContainsKey(id))
		{
			return 0;
		}
		return xpEventToPoints[id];
	}
}
