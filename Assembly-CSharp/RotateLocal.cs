using UnityEngine;

public class RotateLocal : MonoBehaviour
{
	public float rotationSpeed = 360f;

	public Vector3 aroundVector = new Vector3(0f, 1f, 0f);

	private void Update()
	{
		transform.Rotate(aroundVector, rotationSpeed * Time.deltaTime, Space.Self);
	}
}
