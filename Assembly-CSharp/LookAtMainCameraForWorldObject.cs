using UnityEngine;

public class LookAtMainCameraForWorldObject : MonoBehaviour
{
	private Camera mainCamera;

	private void Start()
	{
		mainCamera = Camera.main;
	}

	private void LateUpdate()
	{
		transform.up = -mainCamera.transform.forward;
	}
}
