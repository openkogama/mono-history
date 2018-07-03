using System.Collections.Generic;
using MV.Common;

public static class JoinUIUpdater
{
	public static Queue<MVEventCodes> JoinEventCodes = new Queue<MVEventCodes>();

	private static MVEventCodes latestJoinEvent = MVEventCodes.RequestMaterials;

	public static void UpdateJoinStateForUI(MVEventCodes eventCode)
	{
		if (eventCode != MVEventCodes.SyncronizePing && (latestJoinEvent != MVEventCodes.GameSnapshotData || eventCode == MVEventCodes.SetActorReady))
		{
			JoinEventCodes.Enqueue(eventCode);
			latestJoinEvent = eventCode;
		}
	}
}
