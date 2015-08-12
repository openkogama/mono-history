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

	public MVCollisionFlags collisionFlags;

	public bool testWithOutMoving;

	public MVControllerColliderHit(VoxelHit hit, Vector3 position, Vector3 elipsoidRadius, Vector3 R3Velocity, bool testWithOutMoving, MVCollisionFlags collisionFlags)
	{
		this.hit = hit;
		moveDirection = R3Velocity.normalized;
		positionTouchingHit = moveDirection * hit.distance + position;
		elipsoidNormal = (positionTouchingHit - hit.point).normalized;
		slopeNormal = MathFunctions.DivideVector(MathFunctions.DivideVector(elipsoidNormal, elipsoidRadius), elipsoidRadius).normalized;
		material = MVGameController.Game.MaterialRepository.GetMaterial(CubeBase.GetMaterial(hit.cube, hit.face));
		impactVelocity = R3Velocity / Time.deltaTime;
		this.testWithOutMoving = testWithOutMoving;
		this.collisionFlags = collisionFlags;
	}
}
