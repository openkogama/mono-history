using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVNetworkReporter(MVWorldObjectClient owner) : MVNetworkObject(owner)
{
	private struct SendTransformData(Vector3 position, byte[] rotation) : IEquatable<SendTransformData>
	{
		private Vector3 position = position;

		private byte[] rotation = rotation;

		public bool Equals(SendTransformData other)
		{
			return other.position.Equals(position) && object.Equals(other.rotation, rotation);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			if (obj.GetType() != typeof(SendTransformData))
			{
				return false;
			}
			return Equals((SendTransformData)obj);
		}

		public override int GetHashCode()
		{
			return (position.GetHashCode() * 397) ^ ((rotation != null) ? rotation.GetHashCode() : 0);
		}

		public static bool operator ==(SendTransformData std1, SendTransformData std2)
		{
			return std1.Equals(std2);
		}

		public static bool operator !=(SendTransformData std1, SendTransformData std2)
		{
			return !std1.Equals(std2);
		}
	}

	private long lastUpdateTimestamp = -1L;

	public bool suspendTransformReporting;

	private SendTransformData prevSendTransformData = new SendTransformData(Vector3.zero, new byte[3]);

	private bool stopPackageSent;

	public override void Update(MVNetworkGame game)
	{
		SyncRunTimeDataVariables(game);
		if (suspendTransformReporting || !(Mathf.Abs(game.ServerTimeInMilliSeconds - lastUpdateTimestamp) > 200f))
		{
			return;
		}
		byte[] rotation = QuaternionCompression.ToBytes(WorldObject.Rotation);
		TransformPackageType packageType = TransformPackageType.Interpolate;
		SendTransformData sendTransformData = new SendTransformData(WorldObject.Position, rotation);
		if (sendTransformData == prevSendTransformData)
		{
			if (stopPackageSent)
			{
				return;
			}
			stopPackageSent = true;
			packageType = TransformPackageType.Stop;
		}
		else
		{
			stopPackageSent = false;
		}
		prevSendTransformData = sendTransformData;
		game.UpdateWorldObject(WorldObject.Id, WorldObject.Position, rotation, packageType);
		WorldObject.State = MVWorldObjectState.Synced;
		lastUpdateTimestamp = game.ServerTimeInMilliSeconds;
	}

	public void SyncRunTimeDataVariables(MVNetworkGame game)
	{
		Dictionary<object, object> dictionary = WorldObject.RuntimeDataVariables.Send();
		if (dictionary.Count > 0)
		{
			game.UpdateWorldObjectRunTimeData(WorldObject.Id, dictionary);
		}
	}
}
