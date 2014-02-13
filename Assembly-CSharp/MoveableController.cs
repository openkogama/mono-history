using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveableController : IUpdatecontrollerSubscriber
{
	public Dictionary<int, Vector3> Velocities = new Dictionary<int, Vector3>();

	public Dictionary<int, MVMovable> MoveControllers = new Dictionary<int, MVMovable>();

	public Dictionary<int, MVMovable> CubeModelMovableMap = new Dictionary<int, MVMovable>();

	public float time;

	public MoveableController()
	{
		SyncTimeToServer();
		MVGameController.Instance.UpdateController.AddFixedUpdateObject(this, UpdatePriority.PRE_UPDATEBUCKET_20);
	}

	public void AddMovable(MVMovable movable, bool isInventoryPreviewMovable)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Velocities.Add(movable.GameObjectID, movable.Velocity);
		MoveControllers.Add(movable.GameObjectID, movable);
		if (!isInventoryPreviewMovable)
		{
			if (movable.CubeModel != null)
			{
				CubeModelMovableMap.Add(movable.CubeModel.Id, movable);
			}
			else
			{
				Debug.LogError((object)("[MoveableController] trying add movable " + movable.Id + " without a cube model"));
			}
		}
	}

	public void RemoveMovable(MVMovable movable)
	{
		MoveControllers.Remove(movable.GameObjectID);
		Velocities.Remove(movable.GameObjectID);
		if (movable.CubeModel != null)
		{
			CubeModelMovableMap.Remove(movable.CubeModel.Id);
		}
	}

	public void ResetMoveables()
	{
		foreach (MVMovable value in MoveControllers.Values)
		{
			value.Reset();
		}
		SyncTimeToServer();
	}

	public void UpdateControllerUpdate()
	{
	}

	public void UpdateControllerFixedUpdate()
	{
		UpdateMoveables(1f);
	}

	public void SyncTimeToServer()
	{
		int serverTimeInMilliSeconds = MVGameController.Instance.Game.Peer.ServerTimeInMilliSeconds;
		uint num = 0u;
		num = (uint)((serverTimeInMilliSeconds >= 0) ? serverTimeInMilliSeconds : (int.MaxValue + serverTimeInMilliSeconds + int.MaxValue));
		time = (float)num / 1000f;
		time %= 10000f;
	}

	public void UpdateMoveables(float directionFactor)
	{
		time += Time.fixedDeltaTime;
		foreach (KeyValuePair<int, MVMovable> item in MoveControllers.Where((KeyValuePair<int, MVMovable> x) => x.Value.IsRoot))
		{
			item.Value.UpdateMoverSubTree(directionFactor, 0);
		}
	}

	public void UpdateSingleMoveableInChain(int movableGameObjectID, float directionFactor)
	{
		UpdateMoveable(movableGameObjectID, directionFactor, movableGameObjectID);
		UpdateMoveable(movableGameObjectID, 0f - directionFactor, MoveControllers[movableGameObjectID].ParentMoverID);
	}

	public void UpdateMoveable(int movableGameObjectID, float directionFactor, int breakid)
	{
		MoveControllers[movableGameObjectID].UpdateMoverSubTree(directionFactor, breakid);
	}

	public Quaternion GetRotationQuat(int movableGameObjectID)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (MoveControllers[movableGameObjectID].CubeModel == null)
		{
			return Quaternion.identity;
		}
		Vector3 val = MoveControllers[movableGameObjectID].Transform.rotation * MoveControllers[movableGameObjectID].AngularVelocity;
		Vector3 angularVelocity = MoveControllers[movableGameObjectID].AngularVelocity;
		Quaternion val2 = Quaternion.AngleAxis(57.29578f * angularVelocity.magnitude * Time.fixedDeltaTime, val.normalized);
		if (MoveControllers[movableGameObjectID].ParentMover != null)
		{
			return GetRotationQuat(MoveControllers[movableGameObjectID].ParentMover.GameObjectID) * val2;
		}
		return val2;
	}

	public Vector3 GetVel(int movableGameObjectID, Vector3 position)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		MVMovable value = null;
		MoveControllers.TryGetValue(movableGameObjectID, out value);
		if (value == null)
		{
			return Vector3.zero;
		}
		Vector3 val = position - value.CubeModel.WorldPosition;
		Quaternion rotationQuat = GetRotationQuat(movableGameObjectID);
		Vector3 val2 = rotationQuat * val;
		Vector3 val3 = Velocities[movableGameObjectID] + (val2 - val);
		if (value.ParentMover != null)
		{
			return val3 + GetVel(value.ParentMover.CubeModelID, value.CubeModel.WorldPosition);
		}
		return val3;
	}
}
