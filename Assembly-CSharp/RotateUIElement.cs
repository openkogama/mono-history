using UnityEngine;

public class RotateUIElement : MonoBehaviour
{
	public float rotationSpeed = 360f;

	private void Update()
	{
		transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.World);
	}
}
