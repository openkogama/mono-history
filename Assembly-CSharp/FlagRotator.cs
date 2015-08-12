using UnityEngine;

public class FlagRotator : MonoBehaviour
{
	public MeshRenderer flag;

	public float rotationSpeed = 1f;

	private void Update()
	{
		if (flag != null)
		{
			flag.transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed * 57.29578f, Space.Self);
		}
	}
}
