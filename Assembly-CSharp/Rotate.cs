using System;
using UnityEngine;

public class Rotate : MonoBehaviour
{
	public float rotationSpeed = 360f;

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.RotateAroundLocal(Vector3.up, rotationSpeed * ((float)Math.PI / 180f) * Time.deltaTime);
	}
}
