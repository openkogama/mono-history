using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class MVMovableMotorState
{
	public bool Move(Vector3 velocity, MvCharacterController controller, float tolerance, MVGroundState groundState, out Vector3 movableVelocityVector)
	{
		CheckMoveables(controller, tolerance);
		movableVelocityVector = Vector3.zero;
		bool result = false;
		float num = (float)MVPhysics.Gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
		if (groundState.GroundTest(out var groundHit, controller, velocity, sendCollData: false, num))
		{
			if (Mathf.Abs(Vector3.Dot(groundHit.slopeNormal, Vector3.up)) <= 0.9f)
			{
				result = true;
			}
			MVMovable value = null;
			MVGameController.WOCM.MoveableController.CubeModelMovableMap.TryGetValue(groundHit.hit.woId, out value);
			if (value != null)
			{
				movableVelocityVector = MVGameController.WOCM.MoveableController.GetVel(value.GameObjectID, controller.gameObject.transform.position);
				if (movableVelocityVector.y > 0f)
				{
					movableVelocityVector.y -= num;
				}
				movableVelocityVector /= Time.fixedDeltaTime;
				groundState.UpdateGroundStateWithHitExternal(controller, groundHit);
			}
		}
		return result;
	}

	protected void CheckMoveables(MvCharacterController controller, float tolerance)
	{
		List<MVOverlapResult> overlappingObjects = controller.GetOverlappingObjects();
		if (!overlappingObjects.Any())
		{
			return;
		}
		foreach (MVOverlapResult item in overlappingObjects)
		{
			MVMovable value = null;
			MVGameController.WOCM.MoveableController.CubeModelMovableMap.TryGetValue(item.woId, out value);
			if (value == null)
			{
				continue;
			}
			Vector3 position = controller.gameObject.transform.position;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			float num = 0f;
			zero = MVGameController.WOCM.MoveableController.GetVel(value.GameObjectID, position) / Time.fixedDeltaTime;
			MVGameController.WOCM.MoveableController.UpdateSingleMoveableInChain(value.GameObjectID, -1f);
			IntVector[] localCubePos = item.localCubePos;
			foreach (IntVector iVector in localCubePos)
			{
				Vector3 vector = SharedCubeFunctions.LocalToWorld(value.GameObject, iVector);
				zero2 += position - vector;
				num++;
			}
			if (num > 0.5f)
			{
				zero2 /= num;
			}
			controller.Move(zero, sendCollisionData: false);
			MVGameController.WOCM.MoveableController.UpdateSingleMoveableInChain(value.GameObjectID, 1f);
			if (controller.CheckOverLap())
			{
				MVGameController.WOCM.MoveableController.UpdateSingleMoveableInChain(value.GameObjectID, -1f);
				controller.Move(position - controller.gameObject.transform.position, sendCollisionData: false);
				Vector3 motion = zero2;
				motion.y = 0f;
				if (motion.sqrMagnitude < tolerance * tolerance)
				{
					motion.Normalize();
					motion *= tolerance;
				}
				controller.Move(motion, sendCollisionData: false);
				MVGameController.WOCM.MoveableController.UpdateSingleMoveableInChain(value.GameObjectID, 1f);
			}
			controller.Move(position - controller.gameObject.transform.position);
		}
	}
}
