using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVNetworkListener : MVNetworkObject
{
	private Queue<NetworkTransformPackage> transformQueue;

	private Queue<NetworkInputPackage> inputQueue;

	private float timeCurrentTransform;

	private NetworkTransformPackage currentPackage;

	private NetworkTransformPackage nextPackage;

	private Vector3 calculatedVelocity = Vector3.zero;

	private bool transformReportingHasStopped;

	public Vector3 CalculatedVelocity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return calculatedVelocity;
		}
	}

	public MVNetworkListener(MVWorldObjectClient owner)
		: base(owner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		transformQueue = new Queue<NetworkTransformPackage>();
		inputQueue = new Queue<NetworkInputPackage>();
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

	public override void Update(MVNetworkGame game)
	{
		int delayedTime = game.Peer.ServerTimeInMilliSeconds - 100 - 100 - 100;
		UpdateTransform(game, delayedTime);
		UpdateInput(game, delayedTime);
	}

	private void UpdateTransform(MVNetworkGame game, int delayedTime)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		if (WorldObject.State == MVWorldObjectState.Destroyed)
		{
			return;
		}
		Vector3 localPosition = WorldObject.GameObject.transform.localPosition;
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
				Debug.Log((object)"Interpolation INTERVAL IS ZERO!!!");
				num2 = 1f;
			}
			if (num2 < 0f)
			{
				Debug.Log((object)("Negative interpolation (" + delayedTime + " - " + currentPackage.timestamp + ") / (" + nextPackage.timestamp + " - " + currentPackage.timestamp + ") = (" + (delayedTime - currentPackage.timestamp) + ") / (" + (nextPackage.timestamp - currentPackage.timestamp) + ") = " + num2));
				WorldObject.GameObject.transform.localPosition = currentPackage.position;
				WorldObject.GameObject.transform.localRotation = currentPackage.rotation;
			}
			else if (num2 <= 1f)
			{
				WorldObject.GameObject.transform.localPosition = Vector3.Lerp(currentPackage.position, nextPackage.position, num2);
				WorldObject.GameObject.transform.localRotation = Quaternion.Lerp(currentPackage.rotation, nextPackage.rotation, num2);
			}
			else
			{
				float num3 = Mathf.Min(num2, 2f);
				if (transformReportingHasStopped)
				{
					WorldObject.GameObject.transform.localPosition = nextPackage.position;
					WorldObject.GameObject.transform.localRotation = nextPackage.rotation;
					currentPackage = null;
					nextPackage = null;
				}
				else
				{
					WorldObject.GameObject.transform.localPosition = ExtrapolatePosition(Mathf.Min(num2, num3));
					WorldObject.GameObject.transform.localRotation = ExtrapolateRotation(Mathf.Min(num2, num3));
				}
			}
		}
		else if (currentPackage != null)
		{
			WorldObject.GameObject.transform.localPosition = currentPackage.position;
			WorldObject.GameObject.transform.localRotation = currentPackage.rotation;
		}
		calculatedVelocity = localPosition - WorldObject.GameObject.transform.localPosition;
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
