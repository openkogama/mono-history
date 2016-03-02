using System.Collections.Generic;
using MV.Common;

public static class JoinUIUpdater
{
	public static Queue<MVEventCodes> joinOperations = new Queue<MVEventCodes>();

	public static void UpdateJoinStateForUI(MVEventCodes opCode)
	{
		if (MVGameControllerBase.JoinState != MVJoinState.Playing)
		{
			joinOperations.Enqueue(opCode);
		}
	}
}
