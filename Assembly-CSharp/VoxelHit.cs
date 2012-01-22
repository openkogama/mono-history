using MV.WorldObject;
using UnityEngine;

public struct VoxelHit
{
	public Vector3 point;

	public Vector3 normal;

	public IntVector cubePos;

	public Face face;

	public bool isCubeHit;

	public int woId;

	public Cube cube;

	public float distance;

	public Collider collider;

	public Transform transform;

	public InteractionFlags interactionFlags;
}
