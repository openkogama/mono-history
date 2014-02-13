using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVNetworkListener : MVNetworkObject
{
	private int delayedTime;

	private HashSet<INetworkUpdateListener> updateListenerList = new HashSet<INetworkUpdateListener>();

	private Queue<NetworkTransformPackage> transformQueue;

	private Queue<NetworkInputPackage> inputQueue;

	private NetworkTransformPackage currentPackage;

	private NetworkTransformPackage nextPackage;

	private bool transformReportingHasStopped;

	public int DelayedTime => delayedTime;

	public MVNetworkListener(MVWorldObjectClient owner)
		: base(owner)
	{
		transformQueue = new Queue<NetworkTransformPackage>();
		inputQueue = new Queue<NetworkInputPackage>();
	}

	public void SetOwnerTransformToMostResentPackage()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
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

	public void ClearTransformQueue()
	{
		currentPackage = null;
		nextPackage = null;
		transformQueue.Clear();
	}

	public void AddTransformPackage(NetworkTransformPackage p)
	{
		transformQueue.Enqueue(p);
		WorldObject.State = MVWorldObjectState.Dirty;
		transformReportingHasStopped = false;
	}

	public void AddNetworkInputPackage(NetworkInputPackage p)
	{
		inputQueue.Enqueue(p);
	}

	public void AddNetorkUpdateListener(INetworkUpdateListener listener)
	{
		updateListenerList.Add(listener);
	}

	public void RmoveNetorkUpdateListener(INetworkUpdateListener listener)
	{
		updateListenerList.Remove(listener);
	}

	public override void Update(MVNetworkGame game)
	{
		delayedTime = game.ServerTimeInMilliSeconds - 100 - 100 - 100;
		UpdateTransform(game, delayedTime);
		UpdateInput(game, delayedTime);
		foreach (INetworkUpdateListener updateListener in updateListenerList)
		{
			updateListener.OnNetworkUpdated();
		}
	}

	private void UpdateTransform(MVNetworkGame game, int delayedTime)
	{
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		if (WorldObject.State == MVWorldObjectState.Destroyed)
		{
			return;
		}
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
				Debug.Log((object)"Transform reporting stopped");
			}
			if (num == 0f)
			{
				Debug.Log((object)"Interpolation INTERVAL IS ZERO!!!");
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
			float num3 = Mathf.Min(num2, 2f);
			if (transformReportingHasStopped)
			{
				WorldObject.Position = nextPackage.position;
				WorldObject.Rotation = nextPackage.rotation;
				currentPackage = null;
				nextPackage = null;
			}
			else
			{
				WorldObject.Position = ExtrapolatePosition(Mathf.Min(num2, num3));
				WorldObject.Rotation = ExtrapolateRotation(Mathf.Min(num2, num3));
			}
		}
		else if (currentPackage != null)
		{
			WorldObject.Position = currentPackage.position;
			WorldObject.Rotation = currentPackage.rotation;
		}
	}

	private void UpdateInput(MVNetworkGame game, int delayedTime)
	{
		while (inputQueue.Count > 0 && inputQueue.Peek().timestamp <= delayedTime)
		{
			NetworkInputPackage networkInputPackage = inputQueue.Dequeue();
			WorldObject.HandleInput(networkInputPackage.actionCode, networkInputPackage.keyCode);
		}
	}

	private Vector3 ExtrapolatePosition(float interpFactor)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = nextPackage.position - currentPackage.position;
		return (interpFactor - 1f) * val + nextPackage.position;
	}

	private Quaternion ExtrapolateRotation(float interpFactor)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return nextPackage.rotation;
	}
}
