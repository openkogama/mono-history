using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

internal class GroundState
{
	private float groundDepth = 0.01f;

	private bool grounded;

	private Vector3 groundNormal = Vector3.zero;

	private MVControllerColliderHit groundHit;

	private float gradientAngle;

	private Vector3 gradientDirection;

	private PhysicalProperties physicalProperties;

	private MvCharacterController controller;

	public PhysicalProperties PhysicalProperties => physicalProperties;

	public float GradientAngle => gradientAngle;

	public Vector3 GradientDirection
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return gradientDirection;
		}
	}

	public float GroundDepth => groundDepth;

	public bool Grounded
	{
		get
		{
			return grounded;
		}
		set
		{
			grounded = value;
		}
	}

	public Vector3 GroundNormal
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return groundNormal;
		}
	}

	public MVControllerColliderHit GroundHit => groundHit;

	public GroundState(MvCharacterController controller)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		this.controller = controller;
	}

	public void UpdateIsGrounded(Vector3 velocity, List<MVControllerColliderHit> moveHits)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		groundNormal = Vector3.zero;
		if (controller.TestWithOutSliding(groundDepth, Vector3.down, out groundHit))
		{
			groundHit.impactVelocity = Vector3.zero;
			groundNormal = groundHit.slopeNormal;
			if (moveHits.Count == 0 && velocity.magnitude > 0.01f)
			{
				groundHit.impactVelocity = velocity;
			}
			moveHits.Add(groundHit);
			gradientDirection = GetGradientDirection(controller.ElipsoidRadius, ((Component)controller).transform.position + controller.center, groundHit.hit);
			gradientAngle = GetGradientAngle(gradientDirection);
			physicalProperties = MVGameController.Instance.WOCM.MaterialRepository.GetMaterial(CubeBase.GetMaterial(groundHit.hit.cube, groundHit.hit.face)).physicalProperties;
		}
		else
		{
			physicalProperties = MVPhysics.airPhysicalProperties;
		}
	}

	public bool IsGroundedTest()
	{
		return groundNormal.y > 0.01f;
	}

	public bool IsGrounded()
	{
		return grounded;
	}

	public bool TooSteep()
	{
		return groundNormal.y <= Mathf.Cos(controller.slopeLimit * ((float)Math.PI / 180f));
	}

	private float GetGradientAngle(Vector3 slopeGradient)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Angle(Vector3.up, slopeGradient) - 90f;
	}

	private Vector3 GetGradientDirection(Vector3 elipsoidRadius, Vector3 position, VoxelHit elipsoidHit)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vec = position + Vector3.down * elipsoidHit.distance;
		Vector3 val = MathFunctions.DivideVector(elipsoidHit.point, elipsoidRadius);
		Vector3 val2 = MathFunctions.DivideVector(vec, elipsoidRadius);
		Vector3 val3 = val2 - val;
		Vector3 normalized = val3.normalized;
		if (normalized.y == 0f)
		{
			return Vector3.down;
		}
		if (normalized == Vector3.up)
		{
			return Vector3.zero;
		}
		Vector3 val4 = Vector3.Cross(Vector3.up, normalized);
		Vector3 vec2 = Vector3.Cross(normalized, val4);
		Vector3 val5 = MathFunctions.MultiplyVector(vec2, elipsoidRadius);
		return -val5.normalized;
	}
}
