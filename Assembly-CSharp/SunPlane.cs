using System;
using UnityEngine;

public class SunPlane : MonoBehaviour
{
	public Light mainLight;

	public Transform sunPlane;

	private void OnPreRender()
	{
		sunPlane.position = Camera.main.transform.position - mainLight.transform.forward * 30f;
		sunPlane.LookAt(Camera.main.transform.position);
		sunPlane.Rotate(sunPlane.right, 16200f / (float)Math.PI, Space.World);
	}
}
