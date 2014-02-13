using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class MVMovableMotorState
{
	public bool GetMovablesVelocityVector(MvCharacterController controller, float tolerance, float groundDepth, out Vector3 movableVelocityVector)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		CheckMoveables(controller, tolerance);
		movableVelocityVector = Vector3.zero;
		float num = 0f;
		bool result = false;
		if (controller.TestWithOutSliding(groundDepth, Vector3.down, out var colliderHit))
		{
			if (Mathf.Abs(Vector3.Dot(colliderHit.slopeNormal, Vector3.up)) <= 0.9f)
			{
				result = true;
			}
			MVMovable value = null;
			MVGameController.Instance.WOCM.MoveableController.CubeModelMovableMap.TryGetValue(colliderHit.hit.woId, out value);
			if (value != null)
			{
				Vector3 val = MVGameController.Instance.WOCM.MoveableController.GetVel(value.GameObjectID, ((Component)controller).gameObject.transform.position) / Time.fixedDeltaTime;
				num++;
				movableVelocityVector += val;
			}
		}
		if (num > 0.5f)
		{
			movableVelocityVector /= num;
		}
		return result;
	}

	protected void CheckMoveables(MvCharacterController controller, float tolerance)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		MVCollisionFlags mVCollisionFlags = MVCollisionFlags.None;
		List<MVOverlapResult> overlappingObjects = controller.GetOverlappingObjects();
		if (!overlappingObjects.Any())
		{
			return;
		}
		foreach (MVOverlapResult item in overlappingObjects)
		{
			MVMovable value = null;
			MVGameController.Instance.WOCM.MoveableController.CubeModelMovableMap.TryGetValue(item.woId, out value);
			if (value == null)
			{
				continue;
			}
			Vector3 position = ((Component)controller).gameObject.transform.position;
			Vector3 zero = Vector3.zero;
			Vector3 val = Vector3.zero;
			float num = 0f;
			zero = MVGameController.Instance.WOCM.MoveableController.GetVel(value.GameObjectID, position) / Time.fixedDeltaTime;
			MVGameController.Instance.WOCM.MoveableController.UpdateSingleMoveableInChain(value.GameObjectID, -1f);
			IntVector[] localCubePos = item.localCubePos;
			foreach (IntVector iVector in localCubePos)
			{
				Vector3 val2 = SharedCubeFunctions.LocalToWorld(value.GameObject, iVector);
				val += position - val2;
				num++;
			}
			if (num > 0.5f)
			{
				val /= num;
			}
			controller.Move(zero);
			MVGameController.Instance.WOCM.MoveableController.UpdateSingleMoveableInChain(value.GameObjectID, 1f);
			if (controller.CheckOverLap())
			{
				MVGameController.Instance.WOCM.MoveableController.UpdateSingleMoveableInChain(value.GameObjectID, -1f);
				mVCollisionFlags = controller.Move(position - ((Component)controller).gameObject.transform.position);
				Vector3 val3 = val;
				val3.y = 0f;
				if (val3.sqrMagnitude < tolerance * tolerance)
				{
					val3.Normalize();
					val3 *= tolerance;
				}
				mVCollisionFlags = controller.Move(val3);
				MVGameController.Instance.WOCM.MoveableController.UpdateSingleMoveableInChain(value.GameObjectID, 1f);
			}
			mVCollisionFlags = controller.Move(position - ((Component)controller).gameObject.transform.position);
		}
	}
}
