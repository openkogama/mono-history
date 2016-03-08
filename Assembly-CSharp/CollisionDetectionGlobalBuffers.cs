using UnityEngine;

public static class CollisionDetectionGlobalBuffers
{
	private const int MAX_BUFFER_SIZE = 128;

	public static RaycastHit[] rayHitBuffer = new RaycastHit[128];

	public static Collider[] colliderBuffer = new Collider[128];
}
