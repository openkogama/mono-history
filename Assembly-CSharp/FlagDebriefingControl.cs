using System;

public static class FlagDebriefingControl
{
	public static Action OnFlagDebriefing;

	public static void StartFlagDebriefing()
	{
		if (OnFlagDebriefing != null)
		{
			OnFlagDebriefing();
		}
	}
}
