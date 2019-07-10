using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveableController : IUpdatecontrollerSubscriberFixedUpdate, IUpdatecontrollerSubscriberBase
{
	public Dictionary<int, Vector3> Velocities = new Dictionary<int, Vector3>();

	public Dictionary<int, MVMovable> MoveControllers = new Dictionary<int, MVMovable>();

	public Dictionary<int, MVMovable> CubeModelMovableMap = new Dictionary<int, MVMovable>();

	public float time;

	public MoveableController()
	{
		SyncTimeToServer();
		UpdateController.AddFixedUpdateObject(this, UpdatePriority.PRE_UPDATEBUCKET_20);
	}

	public void AddMovable(MVMovable movable, bool isInventoryPreviewMovable)
	{
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
				Debug.LogError("[MoveableController] trying add movable " + movable.Id + " without a cube model");
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
		int serverTimeInMilliSeconds = MVGameControllerBase.Game.Peer.ServerTimeInMilliSeconds;
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
	}

	public void UpdateMoveable(int movableGameObjectID, float directionFactor, int breakid)
	{
		MoveControllers[movableGameObjectID].UpdateMoverSubTree(directionFactor, breakid);
	}

	public Quaternion GetRotationQuat(int movableGameObjectID)
	{
		if (MoveControllers[movableGameObjectID].CubeModel == null)
		{
			return Quaternion.identity;
		}
		Vector3 vector = MoveControllers[movableGameObjectID].Transform.rotation * MoveControllers[movableGameObjectID].AngularVelocity;
		Quaternion quaternion = Quaternion.AngleAxis(57.29578f * MoveControllers[movableGameObjectID].AngularVelocity.magnitude * Time.fixedDeltaTime, vector.normalized);
		if (MoveControllers[movableGameObjectID].ParentMover != null)
		{
			return GetRotationQuat(MoveControllers[movableGameObjectID].ParentMover.GameObjectID) * quaternion;
		}
		return quaternion;
	}

	public Vector3 GetVel(int movableGameObjectID, Vector3 position)
	{
		MVMovable value = null;
		MoveControllers.TryGetValue(movableGameObjectID, out value);
		if (value == null)
		{
			return Vector3.zero;
		}
		Vector3 vector = position - value.CubeModel.WorldPosition;
		Quaternion rotationQuat = GetRotationQuat(movableGameObjectID);
		Vector3 vector2 = rotationQuat * vector;
		Vector3 vector3 = Velocities[movableGameObjectID] + (vector2 - vector);
		if (value.ParentMover != null)
		{
			return vector3 + GetVel(value.ParentMover.CubeModelID, value.CubeModel.WorldPosition);
		}
		return vector3;
	}
}
