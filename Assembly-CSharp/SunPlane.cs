using System;
using UnityEngine;

public class SunPlane : MonoBehaviour
{
	public Light mainLight;

	public Transform sunPlane;

	private Camera mainCamera;

	private void Start()
	{
		mainCamera = Camera.main;
	}

	private void OnPreRender()
	{
		sunPlane.position = mainCamera.transform.position - mainLight.transform.forward * 30f;
		sunPlane.LookAt(mainCamera.transform.position);
		sunPlane.Rotate(sunPlane.right, 16200f / (float)Math.PI, Space.World);
	}
}
