using System.Collections;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVNetworkReporter(MVWorldObjectClient owner) : MVNetworkObject(owner)
{
	private long lastUpdateTimestamp = -1L;

	private long lastCamUpdateTimestamp = -1L;

	public override void Update(MVNetworkGame game)
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (WorldObject.State == MVWorldObjectState.Dirty)
		{
			if (Mathf.Abs((float)(game.Peer.ServerTimeInMilliSeconds - lastUpdateTimestamp)) > 100f)
			{
				game.UpdateWorldObject(WorldObject.Id, WorldObject.GameObject.transform.localPosition, WorldObject.GameObject.transform.localRotation, TransformPackageType.Interpolate);
				WorldObject.State = MVWorldObjectState.Synced;
				lastUpdateTimestamp = game.Peer.ServerTimeInMilliSeconds;
			}
		}
		else if (WorldObject.State == MVWorldObjectState.StopTransform)
		{
			Debug.Log((object)"Reporter sending stop msg");
			game.UpdateWorldObject(WorldObject.Id, WorldObject.GameObject.transform.localPosition, WorldObject.GameObject.transform.localRotation, TransformPackageType.Stop);
			WorldObject.State = MVWorldObjectState.Synced;
			lastUpdateTimestamp = game.Peer.ServerTimeInMilliSeconds;
		}
		if (WorldObject is MVAvatar && Input.GetKey((KeyCode)323))
		{
			UpdateLineOfFire(game);
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

	public void UpdateLineOfFire(MVNetworkGame game)
	{
		if (Mathf.Abs((float)(game.Peer.ServerTimeInMilliSeconds - lastCamUpdateTimestamp)) > 100f)
		{
			game.UpdateLocalAvatarCam();
			lastCamUpdateTimestamp = game.Peer.ServerTimeInMilliSeconds;
		}
	}
}
