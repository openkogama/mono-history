using MV.WorldObject;
using UnityEngine;

public struct CollisionState
{
	public Vector3 localOrigin;

	public Vector3 localHitPoint;

	public Vector3 localNormal;

	public Vector3 localDirection;

	public Vector3 origin;

	public Vector3 direction;

	public ICubeModelCollider cmb;

	public Matrix4x4 localToElipsoidSpace;

	public int scanAxis;

	public IntVector minBounds;

	public IntVector maxBounds;

	public int firstHitScanAxis;

	public bool firstHitDetected;

	public float scaledMaxRadius;

	public Vector3 elipsoidSpaceOrigin;

	public Vector3 elipsoidSpaceDirection;

	public float elipsoidSpaceDistance;
}
