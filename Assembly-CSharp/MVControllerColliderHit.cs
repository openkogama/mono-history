using MV.WorldObject;
using UnityEngine;

public struct MVControllerColliderHit
{
	public MvCharacterController controller;

	public Vector3 positionTouchingHit;

	public Vector3 moveDirection;

	public Vector3 elipsoidNormal;

	public Vector3 slopeNormal;

	public Vector3 impactVelocity;

	public VoxelHit hit;

	public MVMaterial material;

	public MVControllerColliderHit(MvCharacterController controller, VoxelHit hit, Vector3 position, Vector3 elipsoidRadius, Vector3 R3Velocity)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		this.controller = controller;
		this.hit = hit;
		moveDirection = R3Velocity.normalized;
		positionTouchingHit = moveDirection * hit.distance + position;
		Vector3 val = positionTouchingHit - hit.point;
		elipsoidNormal = val.normalized;
		Vector3 val2 = MathFunctions.DivideVector(MathFunctions.DivideVector(elipsoidNormal, elipsoidRadius), elipsoidRadius);
		slopeNormal = val2.normalized;
		material = MVGameController.Instance.WOCM.MaterialRepository.GetMaterial(CubeBase.GetMaterial(hit.cube, hit.face));
		impactVelocity = R3Velocity / Time.deltaTime;
	}
}
