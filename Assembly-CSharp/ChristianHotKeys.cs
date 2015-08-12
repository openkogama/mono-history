using UnityEngine;

public static class ChristianHotKeys
{
	public static void Handle()
	{
		if (MVInputWrapper.DebugGetKey(KeyCode.Alpha8))
		{
			if (MVInputWrapper.DebugGetKeyUp(KeyCode.U))
			{
			}
			if (MVInputWrapper.DebugGetKeyUp(KeyCode.T))
			{
				Debug.Log("Remove");
			}
		}
	}
}
