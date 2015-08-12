using UnityEngine;

public class SimpleMover : MonoBehaviour
{
	public float speed = 10f;

	private Transform _transform;

	private void Start()
	{
		_transform = transform;
	}

	private void Update()
	{
		float axis = MVInputWrapper.GetAxis("Horizontal");
		float axis2 = MVInputWrapper.GetAxis("Vertical");
		Vector3 vector = Vector3.forward * axis2 + Vector3.right * axis;
		_transform.position += speed * vector * Time.deltaTime;
	}
}
