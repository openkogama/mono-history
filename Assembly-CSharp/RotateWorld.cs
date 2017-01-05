using UnityEngine;

public class RotateWorld : MonoBehaviour
{
	[SerializeField]
	private float rotateSpeed = 360f;

	[SerializeField]
	private Transform rotateTarget;

	private void Update()
	{
		rotateTarget.Rotate(transform.up, rotateSpeed * Time.deltaTime, Space.World);
	}
}
