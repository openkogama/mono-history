using MV.WorldObject;
using UnityEngine;

public struct MVControllerColliderHit
{
	public Vector3 positionTouchingHit;

	public Vector3 moveDirection;

	public Vector3 elipsoidNormal;

	public Vector3 slopeNormal;

	public Vector3 impactVelocity;

	public VoxelHit hit;

	public MVMaterial material;

	public bool testWithOutMoving;

	public MVControllerColliderHit(VoxelHit hit, Vector3 position, Vector3 elipsoidRadius, Vector3 R3Velocity, bool testWithOutMoving)
	{
		this.hit = hit;
		moveDirection = R3Velocity.normalized;
		positionTouchingHit = moveDirection * hit.distance + position;
		elipsoidNormal = (positionTouchingHit - hit.point).normalized;
		Vector3 vec = MathFunctions.DivideVector(ref elipsoidNormal, ref elipsoidRadius);
		slopeNormal = MathFunctions.DivideVector(ref vec, ref elipsoidRadius).normalized;
		material = MVGameControllerBase.Game.MaterialRepository.GetMaterial(CubeBase.GetMaterial(hit.cube, hit.face));
		impactVelocity = R3Velocity / Time.deltaTime;
		this.testWithOutMoving = testWithOutMoving;
	}
}
