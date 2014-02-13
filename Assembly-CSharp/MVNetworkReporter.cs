using System.Collections;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVNetworkReporter(MVWorldObjectClient owner) : MVNetworkObject(owner)
{
	private long lastUpdateTimestamp = -1L;

	public bool suspendTransformReporting;

	public override void Update(MVNetworkGame game)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (!suspendTransformReporting)
		{
			if (WorldObject.State == MVWorldObjectState.Dirty)
			{
				if (Mathf.Abs((float)(game.ServerTimeInMilliSeconds - lastUpdateTimestamp)) > 100f)
				{
					game.UpdateWorldObject(WorldObject.Id, WorldObject.Position, WorldObject.Rotation, TransformPackageType.Interpolate);
					WorldObject.State = MVWorldObjectState.Synced;
					lastUpdateTimestamp = game.ServerTimeInMilliSeconds;
				}
			}
			else if (WorldObject.State == MVWorldObjectState.StopTransform)
			{
				Debug.Log((object)"Reporter sending stop msg");
				game.UpdateWorldObject(WorldObject.Id, WorldObject.Position, WorldObject.Rotation, TransformPackageType.Stop);
				WorldObject.State = MVWorldObjectState.Synced;
				lastUpdateTimestamp = game.ServerTimeInMilliSeconds;
			}
		}
		SyncRunTimeDataVariables(game);
	}

	public void SyncRunTimeDataVariables(MVNetworkGame game)
	{
		Hashtable hashtable = WorldObject.RuntimeDataVariables.Send();
		if (hashtable.Count > 0)
		{
			game.UpdateWorldObjectRunTimeData(WorldObject.Id, hashtable);
		}
	}
}
