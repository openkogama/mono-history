using UnityEngine;

public struct TranslateObjectData
{
	public Bounds localBounds;

	public Bounds? worldBounds;

	public float worldBoundsExtentsMagnitude;

	public Transform transform;

	public GameObject gameObject;

	public float maxDistance;
}
