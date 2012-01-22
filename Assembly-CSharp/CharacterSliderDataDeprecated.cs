using System.Collections.Generic;
using UnityEngine;

public struct CharacterSliderDataDeprecated
{
	public List<VoxelHit> voxelHits;

	public bool isGrounded;

	public Vector3 position;

	public bool didCollide;

	public float friction;

	public Vector3 bounce;

	public int damage;

	public MVCollisionFlags collisionFlags;
}
