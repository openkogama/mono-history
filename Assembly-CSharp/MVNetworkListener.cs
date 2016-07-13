using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVNetworkListener : MVNetworkObject
{
	private HashSet<INetworkUpdateListener> updateListenerList = new HashSet<INetworkUpdateListener>();

	private Queue<NetworkTransformPackage> transformQueue = new Queue<NetworkTransformPackage>();

	private NetworkTransformPackage currentPackage;

	private NetworkTransformPackage nextPackage;

	private bool transformReportingHasStopped;

	private bool stopListening;

	public override bool RemoveFromUpdate => stopListening;

	public MVNetworkListener(MVWorldObjectClient owner)
		: base(owner)
	{
		NetworkTransformPackage item = new NetworkTransformPackage
		{
			packageType = TransformPackageType.Interpolate,
			timestamp = MVGameControllerBase.Game.ServerTimeInMilliSeconds - 200,
			position = owner.Position,
			rotation = owner.Rotation
		};
		transformQueue.Enqueue(item);
	}

	public void SetOwnerTransformToMostResentPackage()
	{
		NetworkTransformPackage networkTransformPackage = null;
		foreach (NetworkTransformPackage item in transformQueue)
		{
			networkTransformPackage = item;
		}
		if (networkTransformPackage == null)
		{
			networkTransformPackage = nextPackage;
		}
		if (networkTransformPackage == null)
		{
			networkTransformPackage = currentPackage;
		}
		if (networkTransformPackage != null)
		{
			WorldObject.Position = networkTransformPackage.position;
			WorldObject.Rotation = networkTransformPackage.rotation;
		}
	}

	public void AddTransformPackage(NetworkTransformPackage p)
	{
		transformQueue.Enqueue(p);
		transformReportingHasStopped = false;
		stopListening = false;
	}

	public void RmoveNetorkUpdateListener(INetworkUpdateListener listener)
	{
		updateListenerList.Remove(listener);
	}

	public override void Update(MVNetworkGame game)
	{
		UpdateTransform(game, TransformNetworkManager.DelayedTime);
	}

	private void UpdateTransform(MVNetworkGame game, int delayedTime)
	{
		if (!transformReportingHasStopped)
		{
			if (currentPackage == null && transformQueue.Count > 0)
			{
				currentPackage = transformQueue.Dequeue();
			}
			if (currentPackage != null && nextPackage == null && transformQueue.Count > 0)
			{
				nextPackage = transformQueue.Dequeue();
			}
		}
		if (currentPackage != null && nextPackage != null)
		{
			if (!transformReportingHasStopped)
			{
				while (nextPackage.timestamp <= delayedTime && transformQueue.Count > 0)
				{
					currentPackage = nextPackage;
					nextPackage = transformQueue.Dequeue();
				}
			}
			float num = nextPackage.timestamp - currentPackage.timestamp;
			float num2 = 0f;
			if (nextPackage.packageType == TransformPackageType.Interpolate)
			{
				num2 = (float)(delayedTime - currentPackage.timestamp) / num;
			}
			else if (nextPackage.packageType == TransformPackageType.Teleport)
			{
				num2 = 1f;
			}
			else if (nextPackage.packageType == TransformPackageType.Stop)
			{
				num2 = (float)(delayedTime - currentPackage.timestamp) / num;
				transformReportingHasStopped = true;
			}
			if (num == 0f)
			{
				Debug.Log("Interpolation INTERVAL IS ZERO!!!");
				num2 = 1f;
			}
			if (num2 < 0f)
			{
				WorldObject.Position = currentPackage.position;
				WorldObject.Rotation = currentPackage.rotation;
				return;
			}
			if (num2 <= 1f)
			{
				WorldObject.Position = Vector3.Lerp(currentPackage.position, nextPackage.position, num2);
				WorldObject.Rotation = Quaternion.Lerp(currentPackage.rotation, nextPackage.rotation, num2);
				return;
			}
			float b = Mathf.Min(num2, 2f);
			if (transformReportingHasStopped)
			{
				WorldObject.Position = nextPackage.position;
				WorldObject.Rotation = nextPackage.rotation;
				currentPackage = null;
				nextPackage = null;
				stopListening = true;
			}
			else
			{
				WorldObject.Position = ExtrapolatePosition(Mathf.Min(num2, b));
				WorldObject.Rotation = ExtrapolateRotation(Mathf.Min(num2, b));
			}
		}
		else if (currentPackage != null)
		{
			WorldObject.Position = currentPackage.position;
			WorldObject.Rotation = currentPackage.rotation;
		}
	}

	private Vector3 ExtrapolatePosition(float interpFactor)
	{
		Vector3 vector = nextPackage.position - currentPackage.position;
		return (interpFactor - 1f) * vector + nextPackage.position;
	}

	private Quaternion ExtrapolateRotation(float interpFactor)
	{
		return nextPackage.rotation;
	}
}
