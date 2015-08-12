using System;
using UnityEngine;

public class Rotate : MonoBehaviour
{
	public float rotationSpeed = 360f;

	private void Update()
	{
		transform.Rotate(Vector3.up, rotationSpeed * ((float)Math.PI / 180f) * Time.deltaTime, Space.World);
	}
}
