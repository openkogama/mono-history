using UnityEngine;

public class MVCharacterController3D : MvCharacterController
{
	public override void Move(Vector3 motion)
	{
		Vector3 vector = CollideAndSlide(motion, transform.position + center);
		transform.position = vector - center;
	}

	public override MvCharacterController CloneToGameObject(GameObject targetGameObject, GameObject seat)
	{
		MvCharacterController mvCharacterController = targetGameObject.AddComponent<MVCharacterController3D>();
		mvCharacterController.Init(Radius, elipsoidRadius.y * 2f, seat.transform.localPosition);
		return mvCharacterController;
	}

	protected override Vector3 CollideAndSlide(Vector3 R3Vel, Vector3 R3Position)
	{
		if (R3Vel.sqrMagnitude == 0f)
		{
			Velocity = Vector3.zero;
			return R3Position;
		}
		bool foundValidPosition = true;
		Vector3 ePos = MathFunctions.DivideVector(R3Position, elipsoidRadius);
		Vector3 eVel = MathFunctions.DivideVector(R3Vel, elipsoidRadius);
		Vector3 vec = ePos;
		Vector3 vector = default;
		collisionRecursionDepth = 0;
		vector = CollideWithWorld(ref ePos, ref eVel, ref foundValidPosition);
		Vector3 r3Position = MathFunctions.MultiplyVector(vector, elipsoidRadius);
		bool flag = OverlapCheckCollision(r3Position);
		Vector3 offset = Vector3.zero;
		if (flag && NoOverlapPosition(r3Position, R3Vel, ref offset))
		{
			flag = false;
		}
		if (foundValidPosition && !flag)
		{
			vec = vector;
		}
		Vector3 vector2 = MathFunctions.MultiplyVector(vec, elipsoidRadius);
		vector2 += offset;
		R3Position += offset;
		Vector3 velocity = vector2 - R3Position;
		Velocity = velocity;
		return vector2;
	}

	protected override Vector3 GetNextVelocity(Vector3 ePoint, Vector3 eNewBasePoint, Vector3 eDestinationPoint, ref Vector3 slidePlaneNormal)
	{
		slidePlaneNormal = eNewBasePoint - ePoint;
		slidePlaneNormal.Normalize();
		Plane plane = new Plane(slidePlaneNormal, ePoint);
		double num = MathFunctions.SignedDistanceTo(plane, ePoint, eDestinationPoint);
		Vector3 vector = eDestinationPoint - (float)num * slidePlaneNormal;
		return vector - ePoint;
	}

	protected override Vector3 RecalcDirectionMoveAway(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
	{
		eDir.Normalize();
		Vector3 vector = ePos + eDir * distance;
		Vector3 normalized = (vector - ePoint).normalized;
		Vector3 vector2 = vector + normalized * 0.005f;
		return (vector2 - ePos).normalized;
	}

	protected override bool NoOverlapPosition(Vector3 R3Position, Vector3 R3Direction, ref Vector3 offset)
	{
		R3Direction.Normalize();
		float num = Mathf.Sqrt(2f);
		float num2 = Vector3.Dot(R3Direction, Vector3.up);
		float num3 = 0.99f;
		Vector3 lhs = Vector3.up;
		if (num2 > num3 || num2 < 0f - num3)
		{
			lhs = Vector3.right;
		}
		Vector3 normalized = Vector3.Cross(lhs, R3Direction).normalized;
		Vector3 normalized2 = Vector3.Cross(normalized, R3Direction).normalized;
		normalized *= 0.005f;
		normalized2 *= 0.005f;
		offset = normalized;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = -normalized;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = normalized2;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = -normalized2;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = (normalized + normalized2) / num;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = -offset;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = (normalized - normalized2) / num;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = -offset;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = Vector3.zero;
		return false;
	}
}
