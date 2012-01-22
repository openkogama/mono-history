using UnityEngine;

public struct TranslateSoundData(float moveValue, bool moveToGridPos, Vector3 worldPos)
{
	public float moveValue = moveValue;

	public bool moveToGridPos = moveToGridPos;

	public Vector3 worldPos = worldPos;
}
