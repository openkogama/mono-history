using System.Collections.Generic;
using MV.Common;

public static class JoinUIUpdater
{
	public static Queue<MVOperationCodes> joinOperations = new Queue<MVOperationCodes>();

	public static void UpdateJoinStateForUI(MVOperationCodes opCode)
	{
		if (MVGameControllerBase.JoinState != MVJoinState.Playing)
		{
			joinOperations.Enqueue(opCode);
		}
	}
}
